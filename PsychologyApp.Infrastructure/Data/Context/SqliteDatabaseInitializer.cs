using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;

namespace PsychologyApp.Infrastructure.Data.Context;

public sealed class SqliteDatabaseInitializer(IDbConnectionFactory connectionFactory) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using System.Data.Common.DbConnection connection =
            await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        await SqliteSchema.EnsureSchemaAsync(connection, cancellationToken);
    }

    public async Task RecreateDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await using (System.Data.Common.DbConnection checkpointConnection =
                     await connectionFactory.CreateOpenConnectionAsync(cancellationToken))
        {
            await SqliteSchema.ConfigureConnectionAsync(checkpointConnection, cancellationToken);
            await checkpointConnection.ExecuteScalarAsync<long>("PRAGMA wal_checkpoint(TRUNCATE);", cancellationToken);
        }

        SqliteConnection.ClearAllPools();
        SqliteSchema.DeleteDatabaseFiles();

        await using System.Data.Common.DbConnection connection =
            await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        await SqliteSchema.EnsureSchemaAsync(connection, cancellationToken);
    }

    public Task ApplyMigrationsForAppVersionAsync(string appVersion, CancellationToken cancellationToken = default)
    {
        // Schema migrations are version-based (SchemaVersion table), not tied to app display version.
        return InitializeAsync(cancellationToken);
    }
}
