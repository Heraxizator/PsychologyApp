using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;

namespace PsychologyApp.Infrastructure.Data.Context;

public sealed class SqliteDatabaseInitializer(IDbConnectionFactory connectionFactory) : IDatabaseInitializer
{
    // The factory already applies the connection PRAGMAs, so neither path repeats them.
    public Task InitializeAsync(CancellationToken cancellationToken = default) =>
        SqliteOperationLane.RunAsync(connectionFactory, () => EnsureSchemaAsync(cancellationToken), cancellationToken);

    // One lane slot for the whole sequence: no other operation may run against the database while its files are deleted.
    public Task RecreateDatabaseAsync(CancellationToken cancellationToken = default) =>
        SqliteOperationLane.RunAsync(connectionFactory, async () =>
        {
            await using (System.Data.Common.DbConnection checkpointConnection =
                         await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false))
            {
                await checkpointConnection.ExecuteScalarAsync<long>("PRAGMA wal_checkpoint(TRUNCATE);", cancellationToken).ConfigureAwait(false);
            }

            SqliteConnection.ClearAllPools();
            SqliteSchema.DeleteDatabaseFiles();

            await EnsureSchemaAsync(cancellationToken).ConfigureAwait(false);
        }, cancellationToken);

    public Task ApplyMigrationsForAppVersionAsync(string appVersion, CancellationToken cancellationToken = default)
    {
        // Schema migrations are version-based (SchemaVersion table), not tied to app display version.
        return InitializeAsync(cancellationToken);
    }

    private async Task EnsureSchemaAsync(CancellationToken cancellationToken)
    {
        await using System.Data.Common.DbConnection connection =
            await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        await SqliteSchema.EnsureSchemaAsync(connection, cancellationToken).ConfigureAwait(false);
    }
}
