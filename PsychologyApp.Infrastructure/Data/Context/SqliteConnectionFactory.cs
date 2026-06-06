using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Infrastructure.Data.Context;
using System.Data.Common;

namespace PsychologyApp.Infrastructure.Data.Context;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    public string DatabasePath { get; } = SqlitePaths.GetDatabasePath();

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection($"Data Source={DatabasePath};Cache=Shared");
        await connection.OpenAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }
}
