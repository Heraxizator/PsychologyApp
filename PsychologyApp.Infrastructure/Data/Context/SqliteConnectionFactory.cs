using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Infrastructure.Data.Context;
using System.Data.Common;

namespace PsychologyApp.Infrastructure.Data.Context;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    public string DatabasePath { get; } = SqlitePaths.GetDatabasePath();

    public bool AllowsParallelReads => true;

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        // No Cache=Shared: shared-cache mode swaps WAL's reader/writer concurrency for table-level locks, and Microsoft
        // advises against combining it with WAL. Pooling already makes opening a fresh connection per operation cheap.
        var connection = new SqliteConnection($"Data Source={DatabasePath}");
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken).ConfigureAwait(false);
        return connection;
    }
}
