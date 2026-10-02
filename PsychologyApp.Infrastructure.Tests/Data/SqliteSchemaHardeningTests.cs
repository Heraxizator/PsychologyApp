using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Sql;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public class SqliteSchemaHardeningTests
{
    [Fact]
    public async Task DropAllTables_RemovesEveryTableTheSchemaCreates()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.EnsureSchemaAsync(connection);

        await SqliteSchema.DropAllTablesAsync(connection);

        IEnumerable<string> left = await connection.QueryAsync<string>(
            "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';");
        Assert.Empty(left);
    }

    [Fact]
    public async Task EnsureSchema_DatabaseFromANewerBuild_IsRefused()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.EnsureSchemaAsync(connection);
        await connection.ExecuteAsync("INSERT INTO SchemaVersion (Version) VALUES (@v);", new { v = SqliteSchema.CurrentVersion + 1 });

        await Assert.ThrowsAsync<InvalidOperationException>(() => SqliteSchema.EnsureSchemaAsync(connection));
    }

    [Fact]
    public async Task ChatMessages_AreRemovedWithTheirSession()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.ConfigureConnectionAsync(connection);
        await SqliteSchema.EnsureSchemaAsync(connection);
        await connection.ExecuteAsync("INSERT INTO ChatSessions (Title, CreatedAt, UpdatedAt) VALUES ('t','2020-01-01T00:00:00Z','2020-01-01T00:00:00Z');");
        await connection.ExecuteAsync("INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt) VALUES (1, 0, 'x', '2020-01-01T00:00:00Z');");

        await connection.ExecuteAsync("DELETE FROM ChatSessions WHERE SessionId = 1;");

        Assert.Equal(0, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ChatMessages;"));
    }

    [Fact]
    public async Task ChatMessages_CannotPointAtAMissingSession()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.ConfigureConnectionAsync(connection);
        await SqliteSchema.EnsureSchemaAsync(connection);

        await Assert.ThrowsAsync<SqliteException>(() => connection.ExecuteAsync(
            "INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt) VALUES (42, 0, 'x', '2020-01-01T00:00:00Z');"));
    }

    [Fact]
    public async Task MigrationTo13_KeepsMessagesAndDropsOrphans()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.EnsureSchemaAsync(connection);
        // Rewind to the pre-FK shape of version 12.
        await connection.ExecuteAsync(
            """
            DROP TABLE ChatMessages;
            CREATE TABLE ChatMessages (
                MessageId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, SessionId INTEGER NOT NULL, Role INTEGER NOT NULL,
                Text TEXT NOT NULL, CreatedAt TEXT NOT NULL, QuickRepliesJson TEXT);
            DELETE FROM SchemaVersion WHERE Version = 13;
            INSERT INTO ChatSessions (Title, CreatedAt, UpdatedAt) VALUES ('t','2020-01-01T00:00:00Z','2020-01-01T00:00:00Z');
            INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt) VALUES (1, 0, 'kept', '2020-01-01T00:00:00Z');
            INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt) VALUES (99, 0, 'orphan', '2020-01-01T00:00:00Z');
            """);

        await SqliteSchema.EnsureSchemaAsync(connection);

        IEnumerable<string> texts = await connection.QueryAsync<string>("SELECT Text FROM ChatMessages;");
        Assert.Equal(["kept"], texts);
    }
}

public class SqliteTimeTests
{
    [Fact]
    public void ToIso_LocalKind_IsWrittenAsUtcWithZ()
    {
        DateTime local = new(2026, 10, 2, 1, 30, 0, DateTimeKind.Local);

        string iso = SqliteTime.ToIso(local);

        Assert.EndsWith("Z", iso);
        Assert.Equal(local.ToUniversalTime(), SqliteTime.FromIso(iso));
    }

    [Theory]
    [InlineData("2026-10-01T22:30:00.0000000Z")]
    [InlineData("2026-10-02T01:30:00.0000000+03:00")]
    [InlineData("2026-10-01T22:30:00")]
    public void FromIso_AlwaysReturnsTheSameUtcInstant(string text)
    {
        DateTime parsed = SqliteTime.FromIso(text);

        Assert.Equal(DateTimeKind.Utc, parsed.Kind);
        Assert.Equal(new DateTime(2026, 10, 1, 22, 30, 0, DateTimeKind.Utc), parsed);
    }

    [Fact]
    public void ToLocalDays_FollowTheUsersClockNotUtc()
    {
        // 23:30 UTC on 1 Oct is already 2 Oct in Moscow (UTC+3) and still 1 Oct in New York (UTC-4 in October).
        string[] stored = ["2026-10-01T23:30"];
        TimeZoneInfo moscow = TimeZoneInfo.CreateCustomTimeZone("msk", TimeSpan.FromHours(3), "msk", "msk");
        TimeZoneInfo newYork = TimeZoneInfo.CreateCustomTimeZone("nyc", TimeSpan.FromHours(-4), "nyc", "nyc");

        Assert.Equal(new DateOnly(2026, 10, 2), Assert.Single(SqliteTime.ToLocalDays(stored, moscow)));
        Assert.Equal(new DateOnly(2026, 10, 1), Assert.Single(SqliteTime.ToLocalDays(stored, newYork)));
    }

    [Fact]
    public void ToLocalDays_AreDistinctAndNewestFirst()
    {
        TimeZoneInfo utc = TimeZoneInfo.Utc;

        IReadOnlyList<DateOnly> days = SqliteTime.ToLocalDays(
            ["2026-09-30T10:00", "2026-10-02T09:00", "2026-10-02T21:00", "2026-10-01T12:00"], utc);

        Assert.Equal([new DateOnly(2026, 10, 2), new DateOnly(2026, 10, 1), new DateOnly(2026, 9, 30)], days);
    }
}
