using Dapper;
using Xunit;
using Microsoft.Data.Sqlite;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Infrastructure.Tests.Data;

public class SqliteSchemaTests
{
    [Fact]
    public async Task EnsureSchema_CreatesAllTables()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        await SqliteSchema.EnsureSchemaAsync(connection);

        int techniques = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Techniques';");
        int schemaVersion = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='SchemaVersion';");

        Assert.Equal(1, techniques);
        Assert.Equal(1, schemaVersion);
    }

    [Fact]
    public async Task EnsureSchema_AppliesVersion2Index()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        await SqliteSchema.EnsureSchemaAsync(connection);

        int version = await connection.ExecuteScalarAsync<int>("SELECT MAX(Version) FROM SchemaVersion;");
        int indexCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='index' AND name='IX_Statistics_PageName';");

        Assert.Equal(3, version);
        Assert.Equal(1, indexCount);
    }
}
