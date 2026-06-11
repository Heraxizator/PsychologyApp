using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Sql;
using System.Data.Common;

namespace PsychologyApp.Testing.Data;

public sealed class SharedMemoryConnectionFactory : IDbConnectionFactory, IAsyncDisposable
{
    private static readonly SemaphoreSlim SchemaInitializationGate = new(1, 1);

    static SharedMemoryConnectionFactory() => Iso8601DateTimeHandler.Register();

    private readonly string _connectionString;
    private readonly SqliteConnection _keeper;

    public SharedMemoryConnectionFactory(string? databaseName = null)
    {
        string name = databaseName ?? Guid.NewGuid().ToString("N");
        _connectionString = $"Data Source={name};Mode=Memory;Cache=Shared";
        _keeper = new SqliteConnection(_connectionString);
        _keeper.Open();
        SchemaInitializationGate.Wait();
        try
        {
            SqliteSchema.EnsureSchemaAsync(_keeper).GetAwaiter().GetResult();
        }
        finally
        {
            SchemaInitializationGate.Release();
        }
    }

    public string DatabasePath => _connectionString;

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        await _keeper.DisposeAsync();
    }
}
