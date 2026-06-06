using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Sql;
using System.Data.Common;

namespace PsychologyApp.Infrastructure.Tests.Data;

internal sealed class SharedMemoryConnectionFactory : IDbConnectionFactory, IAsyncDisposable
{
    private const string ConnectionString = "Data Source=PsychologyAppTests;Mode=Memory;Cache=Shared";
    private static readonly SemaphoreSlim SchemaInitializationGate = new(1, 1);

    static SharedMemoryConnectionFactory() => Iso8601DateTimeHandler.Register();
    private readonly SqliteConnection _keeper;

    public SharedMemoryConnectionFactory()
    {
        _keeper = new SqliteConnection(ConnectionString);
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

    public string DatabasePath => ConnectionString;

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        await _keeper.DisposeAsync();
    }
}

internal static class RepositoryTestContext
{
    public static IOptions<AppSettings> Settings { get; } =
        Options.Create(new AppSettings { DbCommandTimeoutSeconds = 30 });
}
