using Dapper;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace PsychologyApp.Infrastructure.Data.Context;

public static class SqliteSchema
{
    public const int CurrentVersion = 14;

    private static readonly string[] DropTablesSql =
    [
        "DROP TABLE IF EXISTS ChatMemory;",
        "DROP TABLE IF EXISTS ChatMessages;",
        "DROP TABLE IF EXISTS ChatSessions;",
        "DROP TABLE IF EXISTS SessionResults;",
        "DROP TABLE IF EXISTS EscalationEvents;",
        "DROP TABLE IF EXISTS TherapyPrograms;",
        "DROP TABLE IF EXISTS RiskAssessments;",
        "DROP TABLE IF EXISTS MoodEntries;",
        "DROP TABLE IF EXISTS SessionDrafts;",
        "DROP TABLE IF EXISTS Completions;",
        "DROP TABLE IF EXISTS TestResults;",
        "DROP TABLE IF EXISTS SchemaVersion;",
        "DROP TABLE IF EXISTS Techniques;",
        "DROP TABLE IF EXISTS Quots;",
        "DROP TABLE IF EXISTS Statistics;",
        "DROP TABLE IF EXISTS AppMetadata;",
    ];

    public static async Task ConfigureConnectionAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        if (SupportsWal(connection))
        {
            await ExecuteAsync(connection, "PRAGMA journal_mode=WAL;", cancellationToken).ConfigureAwait(false);
            await ExecuteAsync(connection, "PRAGMA synchronous=NORMAL;", cancellationToken).ConfigureAwait(false);
        }

        await ExecuteAsync(connection, "PRAGMA busy_timeout=5000;", cancellationToken).ConfigureAwait(false);
        // Off by default in SQLite and per connection: without it the ChatMessages -> ChatSessions key is only decoration.
        await ExecuteAsync(connection, "PRAGMA foreign_keys=ON;", cancellationToken).ConfigureAwait(false);
    }

    // Dapper's ExecuteAsync(sql, object param) would take a CancellationToken as the parameter bag and ignore it.
    private static Task<int> ExecuteAsync(DbConnection connection, string sql, CancellationToken cancellationToken) =>
        connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

    private static bool SupportsWal(DbConnection connection) =>
        connection is not SqliteConnection sqlite
        || new SqliteConnectionStringBuilder(sqlite.ConnectionString).Mode != SqliteOpenMode.Memory;

    public static async Task EnsureSchemaAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(
            connection,
            """
            CREATE TABLE IF NOT EXISTS SchemaVersion (
                Version INTEGER NOT NULL PRIMARY KEY
            );
            """,
            cancellationToken).ConfigureAwait(false);

        int version = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("SELECT IFNULL(MAX(Version), 0) FROM SchemaVersion;", cancellationToken: cancellationToken)).ConfigureAwait(false);

        if (version > CurrentVersion)
        {
            // Written by a newer build: running older code against it could silently misread or drop what it does not know.
            throw new InvalidOperationException(
                $"The database schema version {version} is newer than this app supports ({CurrentVersion}).");
        }

        if (version < 1)
        {
            await ApplyMigrationAsync(connection, 1, CreateVersion1Async, cancellationToken).ConfigureAwait(false);
            version = 1;
        }

        if (version < 2)
        {
            await ApplyMigrationAsync(connection, 2, MigrateToVersion2Async, cancellationToken).ConfigureAwait(false);
            version = 2;
        }

        if (version < 3)
        {
            await ApplyMigrationAsync(connection, 3, MigrateToVersion3Async, cancellationToken).ConfigureAwait(false);
            version = 3;
        }

        if (version < 4)
        {
            await ApplyMigrationAsync(connection, 4, MigrateToVersion4Async, cancellationToken).ConfigureAwait(false);
            version = 4;
        }

        if (version < 5)
        {
            await ApplyMigrationAsync(connection, 5, MigrateToVersion5Async, cancellationToken).ConfigureAwait(false);
            version = 5;
        }

        if (version < 6)
        {
            await ApplyMigrationAsync(connection, 6, MigrateToVersion6Async, cancellationToken).ConfigureAwait(false);
            version = 6;
        }

        if (version < 7)
        {
            await ApplyMigrationAsync(connection, 7, MigrateToVersion7Async, cancellationToken).ConfigureAwait(false);
            version = 7;
        }

        if (version < 8)
        {
            await ApplyMigrationAsync(connection, 8, MigrateToVersion8Async, cancellationToken).ConfigureAwait(false);
            version = 8;
        }

        if (version < 9)
        {
            await ApplyMigrationAsync(connection, 9, MigrateToVersion9Async, cancellationToken).ConfigureAwait(false);
            version = 9;
        }

        if (version < 10)
        {
            await ApplyMigrationAsync(connection, 10, MigrateToVersion10Async, cancellationToken).ConfigureAwait(false);
            version = 10;
        }

        if (version < 11)
        {
            await ApplyMigrationAsync(connection, 11, MigrateToVersion11Async, cancellationToken).ConfigureAwait(false);
            version = 11;
        }

        if (version < 12)
        {
            await ApplyMigrationAsync(connection, 12, MigrateToVersion12Async, cancellationToken).ConfigureAwait(false);
            version = 12;
        }

        if (version < 13)
        {
            await ApplyMigrationAsync(connection, 13, MigrateToVersion13Async, cancellationToken).ConfigureAwait(false);
            version = 13;
        }

        if (version < 14)
        {
            await ApplyMigrationAsync(connection, 14, MigrateToVersion14Async, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>A quote is unique by its text: two concurrent seedings (the startup one and "more quotes") used to insert the same text twice.
    /// Existing duplicates are removed first (the lowest id, usually the one already read or favourited first, stays).</summary>
    private static async Task MigrateToVersion14Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            DELETE FROM Quots WHERE QuotId NOT IN (SELECT MIN(QuotId) FROM Quots GROUP BY Text);
            DROP INDEX IF EXISTS IX_Quots_Text;
            CREATE UNIQUE INDEX IF NOT EXISTS UX_Quots_Text ON Quots (Text);
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    /// <summary>
    /// Chat messages now belong to their session by a real foreign key (deleting a session removes its messages even if a caller
    /// forgets to). SQLite cannot add a key to an existing table, so the table is rebuilt; orphans are dropped on the way.
    /// </summary>
    private static async Task MigrateToVersion13Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE ChatMessages_v13 (
                MessageId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                SessionId INTEGER NOT NULL REFERENCES ChatSessions(SessionId) ON DELETE CASCADE,
                Role INTEGER NOT NULL,
                Text TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                QuickRepliesJson TEXT
            );

            INSERT INTO ChatMessages_v13 (MessageId, SessionId, Role, Text, CreatedAt, QuickRepliesJson)
            SELECT MessageId, SessionId, Role, Text, CreatedAt, QuickRepliesJson
            FROM ChatMessages
            WHERE SessionId IN (SELECT SessionId FROM ChatSessions);

            DROP TABLE ChatMessages;
            ALTER TABLE ChatMessages_v13 RENAME TO ChatMessages;
            CREATE INDEX IF NOT EXISTS IX_ChatMessages_Session ON ChatMessages(SessionId, MessageId);
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    /// <summary>The journal reads moods by date range and favourites look quotes up by text; both scanned the whole table.</summary>
    private static async Task MigrateToVersion12Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS IX_MoodEntries_RecordedAt ON MoodEntries (RecordedAt);",
            transaction: transaction).ConfigureAwait(false);
        await connection.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS IX_Quots_Text ON Quots (Text);",
            transaction: transaction).ConfigureAwait(false);
    }

    /// <summary>Lets a session's own reflection note ("what did you notice") be looked back up next time
    /// the same technique is opened, instead of only living in the general mood/journal table.</summary>
    private static async Task MigrateToVersion11Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            "ALTER TABLE SessionResults ADD COLUMN Note TEXT;",
            transaction: transaction).ConfigureAwait(false);
    }

    /// <summary>What the chat companion remembers between conversations: the name, and which practices helped.</summary>
    private static async Task MigrateToVersion10Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS ChatMemory (
                MemoryKey TEXT NOT NULL PRIMARY KEY,
                MemoryValue TEXT NOT NULL
            );
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    private static async Task MigrateToVersion9Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS ChatSessions (
                SessionId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                Emotion TEXT,
                Theme TEXT,
                FirstIntensity INTEGER,
                LastIntensity INTEGER,
                StateJson TEXT
            );

            CREATE TABLE IF NOT EXISTS ChatMessages (
                MessageId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                SessionId INTEGER NOT NULL,
                Role INTEGER NOT NULL,
                Text TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                QuickRepliesJson TEXT
            );

            CREATE INDEX IF NOT EXISTS IX_ChatMessages_Session ON ChatMessages(SessionId, MessageId);
            CREATE INDEX IF NOT EXISTS IX_ChatSessions_UpdatedAt ON ChatSessions(UpdatedAt);
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    private static async Task MigrateToVersion8Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS SessionResults (
                SessionResultId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                ItemKey TEXT NOT NULL,
                CompletedAt TEXT NOT NULL,
                DurationSeconds INTEGER NOT NULL,
                PayloadJson TEXT,
                PreIntensity INTEGER,
                PostIntensity INTEGER,
                ProgramType TEXT,
                ProgramWeek INTEGER
            );

            CREATE INDEX IF NOT EXISTS IX_SessionResults_CompletedAt ON SessionResults(CompletedAt);
            CREATE INDEX IF NOT EXISTS IX_SessionResults_ItemKey ON SessionResults(ItemKey);
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    private static async Task MigrateToVersion7Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS RiskAssessments (
                RiskAssessmentId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                AssessedAt TEXT NOT NULL,
                Source TEXT NOT NULL,
                Notes TEXT NOT NULL,
                HasSelfHarmThoughts INTEGER NOT NULL,
                HasSevereDisorientation INTEGER NOT NULL,
                HasSubstanceRisk INTEGER NOT NULL,
                HasSevereInsomnia INTEGER NOT NULL,
                RiskLevel TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS TherapyPrograms (
                ProgramKey TEXT NOT NULL PRIMARY KEY,
                StartedAt TEXT NOT NULL,
                CurrentWeek INTEGER NOT NULL,
                IsActive INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS EscalationEvents (
                EscalationEventId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                CreatedAt TEXT NOT NULL,
                RiskLevel TEXT NOT NULL,
                TriggerSource TEXT NOT NULL,
                Action TEXT NOT NULL,
                Notes TEXT NOT NULL
            );
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    private static async Task MigrateToVersion6Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS AppMetadata (
                Key TEXT NOT NULL PRIMARY KEY,
                Value TEXT NOT NULL
            );
            """,
            transaction: transaction).ConfigureAwait(false);
    }

    private static async Task MigrateToVersion5Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        int legacyColumnExists = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM pragma_table_info('Techniques') WHERE name = 'Describtion';",
            transaction: transaction).ConfigureAwait(false);

        if (legacyColumnExists > 0)
        {
            await connection.ExecuteAsync(
                "ALTER TABLE Techniques RENAME COLUMN Describtion TO Description;",
                transaction: transaction).ConfigureAwait(false);
        }
    }

    private static async Task MigrateToVersion4Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS TestResults (
                TestResultId INTEGER PRIMARY KEY AUTOINCREMENT,
                TestId TEXT NOT NULL,
                Score INTEGER,
                Summary TEXT NOT NULL,
                DetailJson TEXT,
                CompletedAt TEXT NOT NULL
            );
            """,
            transaction: transaction).ConfigureAwait(false);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Completions (
                CompletionId INTEGER PRIMARY KEY AUTOINCREMENT,
                CompletionKind TEXT NOT NULL,
                ItemKey TEXT NOT NULL,
                ModuleName TEXT NOT NULL,
                PageName TEXT NOT NULL,
                CompletedAt TEXT NOT NULL,
                DurationSeconds INTEGER NOT NULL DEFAULT 0
            );
            """,
            transaction: transaction).ConfigureAwait(false);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS SessionDrafts (
                TechniqueKey TEXT NOT NULL PRIMARY KEY,
                PayloadJson TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );
            """,
            transaction: transaction).ConfigureAwait(false);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS MoodEntries (
                MoodEntryId INTEGER PRIMARY KEY AUTOINCREMENT,
                MoodLevel INTEGER NOT NULL,
                Note TEXT,
                RecordedAt TEXT NOT NULL
            );
            """,
            transaction: transaction).ConfigureAwait(false);

        await connection.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS IX_TestResults_TestId ON TestResults (TestId);",
            transaction: transaction).ConfigureAwait(false);

        await connection.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS IX_Completions_CompletedAt ON Completions (CompletedAt);",
            transaction: transaction).ConfigureAwait(false);
    }

    private static async Task ApplyMigrationAsync(
        DbConnection connection,
        int version,
        Func<DbConnection, DbTransaction, CancellationToken, Task> migrate,
        CancellationToken cancellationToken)
    {
        await using DbTransaction transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await migrate(connection, transaction, cancellationToken).ConfigureAwait(false);
            await connection.ExecuteAsync(new CommandDefinition(
                "INSERT OR IGNORE INTO SchemaVersion (Version) VALUES (@version);",
                new { version },
                transaction,
                cancellationToken: cancellationToken)).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    private static async Task MigrateToVersion3Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync("DROP TABLE IF EXISTS Reasons;", transaction: transaction).ConfigureAwait(false);
    }

    private static async Task MigrateToVersion2Async(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        await connection.ExecuteAsync(
            "CREATE INDEX IF NOT EXISTS IX_Statistics_PageName ON Statistics (PageName);",
            transaction: transaction).ConfigureAwait(false);
    }

    public static async Task DropAllTablesAsync(DbConnection connection, CancellationToken cancellationToken = default)
    {
        foreach (string sql in DropTablesSql)
        {
            await ExecuteAsync(connection, sql, cancellationToken).ConfigureAwait(false);
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
                Description TEXT NOT NULL,
                Subject TEXT NOT NULL,
                Author TEXT NOT NULL,
                Algorithm TEXT NOT NULL,
                Image TEXT,
                IsCompleted INTEGER NOT NULL DEFAULT 0
            );
            """,
            transaction: transaction).ConfigureAwait(false);

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
            transaction: transaction).ConfigureAwait(false);

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
            transaction: transaction).ConfigureAwait(false);
    }
}
