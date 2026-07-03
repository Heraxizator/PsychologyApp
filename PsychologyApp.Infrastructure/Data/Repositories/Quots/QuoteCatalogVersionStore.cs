using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Quots;

public sealed class QuoteCatalogVersionStore(
    IDbConnectionFactory connectionFactory,
    IOptions<AppSettings> settings) : IQuoteCatalogVersionStore
{
    private const string Key = "QuoteCatalogVersion";
    private int CommandTimeoutSeconds => settings.Value.DbCommandTimeoutSeconds;

    public async Task<int> GetAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.ExecuteScalarAsync<string?>(DapperCommandFactory.Create(
            "SELECT Value FROM AppMetadata WHERE Key = @key;",
            new { key = Key },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return int.TryParse(value, out int version) ? version : 0;
    }

    public async Task SetAsync(int version, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            """
            INSERT INTO AppMetadata (Key, Value)
            VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
            """,
            new { key = Key, value = version.ToString() },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        SqliteConnection connection = (SqliteConnection)await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }
}
