using Dapper;
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

    protected async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }
}
