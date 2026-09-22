using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;

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
    protected async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken = default) =>
        (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
}
