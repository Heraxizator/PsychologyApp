using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

public abstract class SqliteRepositoryBase
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly int _commandTimeoutSeconds;

    protected SqliteRepositoryBase(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    {
        _connectionFactory = connectionFactory;
        _commandTimeoutSeconds = settings.Value.DbCommandTimeoutSeconds > 0
            ? settings.Value.DbCommandTimeoutSeconds
            : 30;
    }

    protected int CommandTimeoutSeconds => _commandTimeoutSeconds;

    // Every IDbConnectionFactory implementation already configures (PRAGMA busy_timeout, WAL) the connection it opens
    // (see SqliteConnectionFactory / SharedMemoryConnectionFactory), so this used to run the same three PRAGMA statements
    // a second time on every single repository call. Just hand the already-configured connection back.
    // Returned through the operation lane so the awaiting repository method continues — and queries — off the UI thread.
    protected ConfiguredTaskAwaitable<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) =>
        SqliteOperationLane.OpenAsync(_connectionFactory, cancellationToken);

    /// <summary>For methods that only read: runs in parallel with other readers, after the writes requested before it.</summary>
    protected ConfiguredTaskAwaitable<SqliteConnection> OpenReadConnectionAsync(CancellationToken cancellationToken = default) =>
        SqliteOperationLane.OpenAsync(_connectionFactory, cancellationToken, readOnly: true);
}
