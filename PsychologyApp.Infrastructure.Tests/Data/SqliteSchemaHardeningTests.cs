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
            DELETE FROM SchemaVersion WHERE Version >= 13;
            INSERT INTO ChatSessions (Title, CreatedAt, UpdatedAt) VALUES ('t','2020-01-01T00:00:00Z','2020-01-01T00:00:00Z');
            INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt) VALUES (1, 0, 'kept', '2020-01-01T00:00:00Z');
            INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt) VALUES (99, 0, 'orphan', '2020-01-01T00:00:00Z');
            """);

        await SqliteSchema.EnsureSchemaAsync(connection);

        IEnumerable<string> texts = await connection.QueryAsync<string>("SELECT Text FROM ChatMessages;");
        Assert.Equal(["kept"], texts);
    }

    [Fact]
    public async Task MigrationTo14_RemovesDuplicateQuotesAndKeepsTheFirstOfEach()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.EnsureSchemaAsync(connection);
        await connection.ExecuteAsync(
            """
            DROP INDEX UX_Quots_Text;
            DELETE FROM SchemaVersion WHERE Version >= 14;
            INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('a', 'same', 't', 1, 0);
            INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('b', 'same', 't', 0, 0);
            INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('c', 'other', 't', 0, 0);
            """);

        await SqliteSchema.EnsureSchemaAsync(connection);

        Assert.Equal(["a", "c"], await connection.QueryAsync<string>("SELECT Title FROM Quots ORDER BY QuotId;"));
        await Assert.ThrowsAsync<SqliteException>(() => connection.ExecuteAsync(
            "INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('d', 'same', 't', 0, 0);"));
    }

    [Fact]
    public async Task UpgradeFromVersion1_KeepsTheOldRowsAndReachesTheCurrentSchema()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        // A database exactly as the first release left it: version 1 tables only, with user data in them.
        await connection.ExecuteAsync(
            """
            CREATE TABLE SchemaVersion (Version INTEGER NOT NULL PRIMARY KEY);
            INSERT INTO SchemaVersion (Version) VALUES (1);
            CREATE TABLE Techniques (TechniqueId INTEGER PRIMARY KEY AUTOINCREMENT, Number TEXT NOT NULL, Date TEXT NOT NULL, Header TEXT NOT NULL,
                Describtion TEXT NOT NULL, Subject TEXT NOT NULL, Author TEXT NOT NULL, Algorithm TEXT NOT NULL, Image TEXT, IsCompleted INTEGER NOT NULL DEFAULT 0);
            CREATE TABLE Quots (QuotId INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT NOT NULL, Text TEXT NOT NULL, Theme TEXT NOT NULL,
                IsReaded INTEGER NOT NULL DEFAULT 0, IsFavourite INTEGER NOT NULL DEFAULT 0);
            CREATE TABLE Statistics (StatisticId INTEGER PRIMARY KEY AUTOINCREMENT, ModuleName TEXT NOT NULL, PageName TEXT NOT NULL,
                DateTime TEXT NOT NULL, SecondsDuration INTEGER NOT NULL);
            INSERT INTO Techniques (Number, Date, Header, Describtion, Subject, Author, Algorithm) VALUES ('1', 'd', 'My technique', 'desc', 's', 'a', 'steps');
            INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('t', 'kept quote', 'x', 1, 1);
            INSERT INTO Statistics (ModuleName, PageName, DateTime, SecondsDuration) VALUES ('m', 'p', '2020-01-01T00:00:00Z', 5);
            """);

        await SqliteSchema.EnsureSchemaAsync(connection);

        Assert.Equal(SqliteSchema.CurrentVersion, await connection.ExecuteScalarAsync<int>("SELECT MAX(Version) FROM SchemaVersion;"));
        Assert.Equal("desc", await connection.ExecuteScalarAsync<string>("SELECT Description FROM Techniques;"));
        Assert.Equal("kept quote", await connection.ExecuteScalarAsync<string>("SELECT Text FROM Quots WHERE IsFavourite = 1 AND IsReaded = 1;"));
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Statistics;"));
        foreach (string table in new[] { "MoodEntries", "SessionResults", "ChatSessions", "ChatMessages", "ChatMemory", "RiskAssessments", "TherapyPrograms" })
        {
            Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name = @tableName;", new { tableName = table }));
        }
    }

    [Fact]
    public async Task EnsureSchemaTwice_ChangesNothingTheSecondTime()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await SqliteSchema.EnsureSchemaAsync(connection);
        string first = string.Join("|", await connection.QueryAsync<string>("SELECT sql FROM sqlite_master WHERE sql IS NOT NULL ORDER BY name;"));

        await SqliteSchema.EnsureSchemaAsync(connection);

        Assert.Equal(first, string.Join("|", await connection.QueryAsync<string>("SELECT sql FROM sqlite_master WHERE sql IS NOT NULL ORDER BY name;")));
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
