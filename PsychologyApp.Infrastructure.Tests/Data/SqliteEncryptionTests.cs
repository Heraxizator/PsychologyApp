using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Infrastructure.Data.Context;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public sealed class SqliteEncryptionTests : IDisposable
{
    private static readonly string Key = new('a', 64);
    private static readonly string OtherKey = new('b', 64);

    private readonly string _path = Path.Combine(Path.GetTempPath(), $"enc-{Guid.NewGuid():N}.db");

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        foreach (string suffix in new[] { string.Empty, "-wal", "-shm", ".plain.bak" })
        {
            try
            {
                File.Delete(_path + suffix);
            }
            catch (IOException)
            {
            }
        }
    }

    private async Task CreatePlainDatabaseAsync(bool wal)
    {
        await using SqliteConnection connection = new($"Data Source={_path};Pooling=False");
        await connection.OpenAsync();
        if (wal)
        {
            await connection.ExecuteAsync("PRAGMA journal_mode=WAL;");
        }

        await SqliteSchema.EnsureSchemaAsync(connection);
        await connection.ExecuteAsync("INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('t', 'a secret thought', 'x', 0, 0);");
        await connection.ExecuteAsync("INSERT INTO ChatSessions (Title, CreatedAt, UpdatedAt) VALUES ('chat','2020-01-01T00:00:00Z','2020-01-01T00:00:00Z');");
    }

    private async Task<SqliteConnection> OpenWithKeyAsync(string key)
    {
        SqliteConnection connection = new($"Data Source={_path};Pooling=False");
        await connection.OpenAsync();
        await SqliteEncryption.ApplyKeyAsync(connection, key);
        return connection;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task APlainDatabaseIsMovedToTheKeyAndKeepsEveryRow(bool wal)
    {
        await CreatePlainDatabaseAsync(wal);

        bool encrypted = await SqliteEncryption.EnsureEncryptedAsync(_path, Key);

        Assert.True(encrypted);
        await using SqliteConnection connection = await OpenWithKeyAsync(Key);
        Assert.Equal("a secret thought", await connection.ExecuteScalarAsync<string>("SELECT Text FROM Quots;"));
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ChatSessions;"));
        Assert.Equal(SqliteSchema.CurrentVersion, await connection.ExecuteScalarAsync<int>("SELECT MAX(Version) FROM SchemaVersion;"));
        Assert.False(File.Exists(_path + ".plain.bak"));
    }

    [Fact]
    public async Task TheEncryptedFileHoldsNoReadableText()
    {
        await CreatePlainDatabaseAsync(wal: true);
        await SqliteEncryption.EnsureEncryptedAsync(_path, Key);
        SqliteConnection.ClearAllPools();

        byte[] bytes = await File.ReadAllBytesAsync(_path);

        Assert.DoesNotContain("a secret thought", System.Text.Encoding.UTF8.GetString(bytes));
        Assert.DoesNotContain("SQLite format 3", System.Text.Encoding.ASCII.GetString(bytes, 0, 16));
    }

    [Fact]
    public async Task WithoutTheKeyOrWithAnotherKeyTheFileCannotBeRead()
    {
        await CreatePlainDatabaseAsync(wal: false);
        await SqliteEncryption.EnsureEncryptedAsync(_path, Key);

        await using (SqliteConnection noKey = new($"Data Source={_path};Pooling=False"))
        {
            await noKey.OpenAsync();
            await Assert.ThrowsAsync<SqliteException>(() => noKey.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sqlite_master;"));
        }

        await using SqliteConnection wrong = await OpenWithKeyAsync(OtherKey);
        await Assert.ThrowsAsync<SqliteException>(() => wrong.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sqlite_master;"));
    }

    [Fact]
    public async Task AnEncryptedDatabaseIsLeftAloneTheSecondTime()
    {
        await CreatePlainDatabaseAsync(wal: false);
        await SqliteEncryption.EnsureEncryptedAsync(_path, Key);

        Assert.True(await SqliteEncryption.EnsureEncryptedAsync(_path, Key));

        await using SqliteConnection connection = await OpenWithKeyAsync(Key);
        Assert.Equal(1, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Quots;"));
    }

    [Fact]
    public async Task ACopyLeftByAnInterruptedMoveIsRemovedOnceTheKeyWorks()
    {
        await CreatePlainDatabaseAsync(wal: false);
        await SqliteEncryption.EnsureEncryptedAsync(_path, Key);
        await File.WriteAllTextAsync(_path + ".plain.bak", "leftover");

        await SqliteEncryption.EnsureEncryptedAsync(_path, Key);

        Assert.False(File.Exists(_path + ".plain.bak"));
    }

    [Fact]
    public async Task ANewDatabaseCreatedWithTheKeyThroughTheFactoryPathIsEncryptedFromTheStart()
    {
        await using (SqliteConnection connection = await OpenWithKeyAsync(Key))
        {
            await SqliteSchema.ConfigureConnectionAsync(connection);
            await SqliteSchema.EnsureSchemaAsync(connection);
            await connection.ExecuteAsync("INSERT INTO Quots (Title, Text, Theme, IsReaded, IsFavourite) VALUES ('t', 'second secret', 'x', 0, 0);");
        }

        SqliteConnection.ClearAllPools();
        Assert.DoesNotContain("second secret", System.Text.Encoding.UTF8.GetString(await File.ReadAllBytesAsync(_path)));
    }

    [Fact]
    public async Task APooledConnectionCanBeKeyedAgainWhenReused()
    {
        await CreatePlainDatabaseAsync(wal: false);
        await SqliteEncryption.EnsureEncryptedAsync(_path, Key);

        for (int i = 0; i < 3; i++)
        {
            await using SqliteConnection pooled = new($"Data Source={_path}");
            await pooled.OpenAsync();
            await SqliteEncryption.ApplyKeyAsync(pooled, Key);
            await SqliteSchema.ConfigureConnectionAsync(pooled);
            Assert.Equal(1, await pooled.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Quots;"));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("zz")]
    [InlineData("'; DROP TABLE Quots; --")]
    public async Task AMalformedKeyIsRefusedBeforeAnythingIsSent(string key)
    {
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();

        await Assert.ThrowsAsync<ArgumentException>(() => SqliteEncryption.ApplyKeyAsync(connection, key));
    }
}
