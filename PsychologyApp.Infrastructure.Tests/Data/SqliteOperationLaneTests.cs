using Microsoft.Data.Sqlite;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public sealed class SqliteOperationLaneTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();

    [Fact]
    public async Task Timed_out_operation_never_runs_and_does_not_release_its_predecessor()
    {
        await using SqliteConnection first = await OpenAsTask();
        bool ran = false;
        await Assert.ThrowsAsync<TimeoutException>(() => SqliteOperationLane.RunAsync(
            _connectionFactory, () => { ran = true; return Task.CompletedTask; }, CancellationToken.None));
        Assert.False(ran);
        Task<SqliteConnection> third = OpenAsTask();
        await Task.Delay(200);
        Assert.False(third.IsCompleted);
        await first.DisposeAsync();
        await using SqliteConnection opened = await third.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Cancelled_wait_does_not_allow_a_later_writer_to_overtake()
    {
        await using SqliteConnection first = await OpenAsTask();
        using CancellationTokenSource cancellation = new();
        Task waiting = SqliteOperationLane.RunAsync(_connectionFactory,
            () => throw new InvalidOperationException("Must not run"), cancellation.Token);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => waiting.WaitAsync(TimeSpan.FromSeconds(2)));
        Task<SqliteConnection> third = OpenAsTask();
        await Task.Delay(200);
        Assert.False(third.IsCompleted);
        await first.DisposeAsync();
        await using SqliteConnection opened = await third.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task A_later_operation_waits_until_the_earlier_connection_is_disposed()
    {
        SqliteConnection first = await SqliteOperationLane.OpenAsync(_connectionFactory, CancellationToken.None);
        Task<SqliteConnection> second = OpenAsTask();

        await Task.Delay(200);
        Assert.False(second.IsCompleted);

        await first.DisposeAsync();
        await using SqliteConnection opened = await second.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(System.Data.ConnectionState.Open, opened.State);
    }

    [Fact]
    public async Task Operations_run_in_the_order_they_were_requested()
    {
        List<int> order = [];
        Task[] operations = Enumerable.Range(0, 5)
            .Select(i => RecordAsync(i, order))
            .ToArray();

        await Task.WhenAll(operations).WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Equal([0, 1, 2, 3, 4], order);
    }

    [Fact]
    public async Task A_failed_run_releases_its_slot()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => SqliteOperationLane.RunAsync(
            _connectionFactory,
            () => throw new InvalidOperationException(),
            CancellationToken.None));

        await using SqliteConnection next = await OpenAsTask().WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(System.Data.ConnectionState.Open, next.State);
    }

    private async Task RecordAsync(int index, List<int> order)
    {
        await using SqliteConnection connection = await SqliteOperationLane.OpenAsync(_connectionFactory, CancellationToken.None);
        // Earlier requests are held a little longer, so without the lane the later ones would overtake them.
        await Task.Delay((5 - index) * 20);
        lock (order)
        {
            order.Add(index);
        }
    }

    private async Task<SqliteConnection> OpenAsTask() =>
        await SqliteOperationLane.OpenAsync(_connectionFactory, CancellationToken.None);

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connectionFactory.DisposeAsync();
}
