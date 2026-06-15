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

        Assert.Equal(5, version);
        Assert.Equal(1, indexCount);
    }

    [Fact]
    public async Task EnsureSchema_CreatesProgressTablesForVersion4()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        await SqliteSchema.EnsureSchemaAsync(connection);

        Assert.Equal(1, await TableExistsAsync(connection, "TestResults"));
        Assert.Equal(1, await TableExistsAsync(connection, "Completions"));
        Assert.Equal(1, await TableExistsAsync(connection, "SessionDrafts"));
        Assert.Equal(1, await TableExistsAsync(connection, "MoodEntries"));
    }

    [Fact]
    public async Task EnsureSchema_UsesDescriptionColumnAtVersion5()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        await SqliteSchema.EnsureSchemaAsync(connection);

        int descriptionColumn = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM pragma_table_info('Techniques') WHERE name = 'Description';");
        int legacyColumn = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM pragma_table_info('Techniques') WHERE name = 'Describtion';");

        Assert.Equal(1, descriptionColumn);
        Assert.Equal(0, legacyColumn);
    }

    private static Task<int> TableExistsAsync(SqliteConnection connection, string tableName) =>
        connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@tableName;",
            new { tableName });
}
