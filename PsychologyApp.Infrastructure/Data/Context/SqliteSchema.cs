using Dapper;
using System.Data.Common;

namespace PsychologyApp.Infrastructure.Data.Context;

public static class SqliteSchema
{
    public const int CurrentVersion = 3;

    private static readonly string[] DropTablesSql =
    [
        "DROP TABLE IF EXISTS SchemaVersion;",
        "DROP TABLE IF EXISTS Techniques;",
        "DROP TABLE IF EXISTS Quots;",
        "DROP TABLE IF EXISTS Statistics;",
    ];

    public static async Task ConfigureConnectionAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        await connection.ExecuteAsync("PRAGMA journal_mode=WAL;", cancellationToken);
        await connection.ExecuteAsync("PRAGMA synchronous=NORMAL;", cancellationToken);
        await connection.ExecuteAsync("PRAGMA busy_timeout=5000;", cancellationToken);
    }

    public static async Task EnsureSchemaAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS SchemaVersion (
                Version INTEGER NOT NULL PRIMARY KEY
            );
            """,
            cancellationToken);

        int version = await connection.ExecuteScalarAsync<int>(
            "SELECT IFNULL(MAX(Version), 0) FROM SchemaVersion;",
            cancellationToken);

        if (version < 1)
        {
            await ApplyMigrationAsync(connection, 1, CreateVersion1Async, cancellationToken);
            version = 1;
        }

        if (version < 2)
        {
            await ApplyMigrationAsync(connection, 2, MigrateToVersion2Async, cancellationToken);
            version = 2;
        }

        if (version < 3)
        {
            await ApplyMigrationAsync(connection, 3, MigrateToVersion3Async, cancellationToken);
        }
    }

    private static async Task ApplyMigrationAsync(
        DbConnection connection,
        int version,
        Func<DbConnection, DbTransaction, CancellationToken, Task> migrate,
        CancellationToken cancellationToken)
    {
        await using DbTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await migrate(connection, transaction, cancellationToken);
            await connection.ExecuteAsync(
                "INSERT OR IGNORE INTO SchemaVersion (Version) VALUES (@version);",
                new { version },
                transaction);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task MigrateToVersion3Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync("DROP TABLE IF EXISTS Reasons;", transaction: transaction);
    }

    private static async Task MigrateToVersion2Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS IX_Statistics_PageName ON Statistics (PageName);",
            transaction: transaction);
    }

    public static async Task DropAllTablesAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        foreach (string sql in DropTablesSql)
        {
            await connection.ExecuteAsync(sql, cancellationToken);
        }
    }

    public static void DeleteDatabaseFiles()
    {
        string path = SqlitePaths.GetDatabasePath();
        foreach (string file in new[] { path, $"{path}-wal", $"{path}-shm" })
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    private static async Task CreateVersion1Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Techniques (
                TechniqueId INTEGER PRIMARY KEY AUTOINCREMENT,
                Number TEXT NOT NULL,
                Date TEXT NOT NULL,
                Header TEXT NOT NULL,
                Describtion TEXT NOT NULL,
                Subject TEXT NOT NULL,
                Author TEXT NOT NULL,
                Algorithm TEXT NOT NULL,
                Image TEXT,
                IsCompleted INTEGER NOT NULL DEFAULT 0
            );
            """,
            transaction: transaction);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Quots (
                QuotId INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Text TEXT NOT NULL,
                Theme TEXT NOT NULL,
                IsReaded INTEGER NOT NULL DEFAULT 0,
                IsFavourite INTEGER NOT NULL DEFAULT 0
            );
            """,
            transaction: transaction);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Statistics (
                StatisticId INTEGER PRIMARY KEY AUTOINCREMENT,
                ModuleName TEXT NOT NULL,
                PageName TEXT NOT NULL,
                DateTime TEXT NOT NULL,
                SecondsDuration INTEGER NOT NULL
            );
            """,
            transaction: transaction);
    }
}
