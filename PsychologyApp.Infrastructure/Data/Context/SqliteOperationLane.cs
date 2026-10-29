using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;

namespace PsychologyApp.Infrastructure.Data.Context;

/// <summary>
/// Runs database work on the thread pool in the order it was requested: a writer waits for everything requested before it, a reader waits only for the writers requested before it, so readers run in parallel (WAL) while every pair that involves a write keeps its order.
/// Microsoft.Data.Sqlite executes its async APIs synchronously, so a repository method awaited from the UI thread
/// used to open the connection and run its whole query right there, stalling the UI. The single ordered lane keeps
/// the ordering the app implicitly relied on while everything ran inline — e.g. a debounced draft save must not
/// land after the draft is deleted on session completion.
/// </summary>
internal static class SqliteOperationLane
{
    // Bound the caller wait, but never bypass an unfinished database operation.
    private static readonly TimeSpan MaxWaitForPrevious = TimeSpan.FromSeconds(10);

    private static readonly ConditionalWeakTable<IDbConnectionFactory, Lane> Lanes = new();

    /// <summary>
    /// Opens a connection off the UI thread once earlier operations finish; the slot is released when the connection
    /// closes. Awaiting the result resumes on the thread pool, so the caller's query runs there as well.
    /// </summary>
    public static ConfiguredTaskAwaitable<SqliteConnection> OpenAsync(IDbConnectionFactory factory, CancellationToken cancellationToken, bool readOnly = false) =>
        OpenCoreAsync(factory, cancellationToken, readOnly).ConfigureAwait(false);

    /// <summary>Holds a single slot for an operation that opens several connections itself (schema setup, recreation).</summary>
    public static async Task RunAsync(IDbConnectionFactory factory, Func<Task> operation, CancellationToken cancellationToken)
    {
        (Task previous, TaskCompletionSource done) = LaneFor(factory).Reserve(read: false);
        try
        {
            await previous.WaitAsync(MaxWaitForPrevious, cancellationToken).ConfigureAwait(false);
            await Task.Run(operation, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            done.TrySetResult();
        }
    }

    private static async Task<SqliteConnection> OpenCoreAsync(IDbConnectionFactory factory, CancellationToken cancellationToken, bool readOnly)
    {
        // Reserved synchronously, before the first await, so the lane order is the order callers asked in.
        (Task previous, TaskCompletionSource done) = LaneFor(factory).Reserve(read: readOnly && factory.AllowsParallelReads);
        try
        {
            await previous.WaitAsync(MaxWaitForPrevious, cancellationToken).ConfigureAwait(false);
            SqliteConnection connection = (SqliteConnection)await Task
                .Run(() => factory.CreateOpenConnectionAsync(cancellationToken), cancellationToken)
                .ConfigureAwait(false);

            connection.StateChange += (_, e) =>
            {
                if (e.CurrentState == ConnectionState.Closed)
                {
                    done.TrySetResult();
                }
            };
            connection.Disposed += (_, _) => done.TrySetResult();
            return connection;
        }
        catch
        {
            done.TrySetResult();
            throw;
        }
    }

    private static Lane LaneFor(IDbConnectionFactory factory) => Lanes.GetValue(factory, static _ => new Lane());

    private sealed class Lane
    {
        private readonly Lock _sync = new();
        private Task _lastWriter = Task.CompletedTask;
        private readonly List<Task> _readersSinceWriter = [];

        /// <summary>
        /// A reader waits for the writers requested before it; a writer waits for the writers and the readers requested before it.
        /// Read/read is the only pair left unordered.
        /// </summary>
        public (Task Previous, TaskCompletionSource Done) Reserve(bool read)
        {
            TaskCompletionSource done = new(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_sync)
            {
                if (read)
                {
                    _readersSinceWriter.RemoveAll(static t => t.IsCompleted);
                    _readersSinceWriter.Add(Task.WhenAll(_lastWriter, done.Task));
                    return (_lastWriter, done);
                }

                Task previous = _readersSinceWriter.Count == 0
                    ? _lastWriter
                    : Task.WhenAll(_readersSinceWriter.Append(_lastWriter));
                _readersSinceWriter.Clear();
                // A cancelled or timed-out slot still carries its unfinished predecessors.
                _lastWriter = Task.WhenAll(previous, done.Task);
                return (previous, done);
            }
        }
    }
}
