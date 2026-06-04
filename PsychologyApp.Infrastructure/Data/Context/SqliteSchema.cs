using Dapper;
using Microsoft.Data.Sqlite;

namespace PsychologyApp.Infrastructure.Data.Context;

public static class SqliteSchema
{
    private static readonly string[] DropTablesSql =
    [
        "DROP TABLE IF EXISTS Techniques;",
        "DROP TABLE IF EXISTS Quots;",
        "DROP TABLE IF EXISTS Reasons;",
        "DROP TABLE IF EXISTS Statistics;",
    ];

    private static readonly string[] CreateTablesSql =
    [
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
        """
        CREATE TABLE IF NOT EXISTS Reasons (
            ReasonId INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            Subtitle TEXT NOT NULL,
            Solution TEXT NOT NULL
        );
        """,
        """
        CREATE TABLE IF NOT EXISTS Statistics (
            StatisticId INTEGER PRIMARY KEY AUTOINCREMENT,
            ModuleName TEXT NOT NULL,
            PageName TEXT NOT NULL,
            DateTime TEXT NOT NULL,
            SecondsDuration INTEGER NOT NULL
        );
        """,
    ];

    public static async Task CreateTablesAsync(SqliteConnection connection, int commandTimeout = 0)
    {
        foreach (string sql in CreateTablesSql)
        {
            await connection.ExecuteAsync(sql, commandTimeout: commandTimeout);
        }
    }

    public static async Task DropTablesAsync(SqliteConnection connection, int commandTimeout = 0)
    {
        foreach (string sql in DropTablesSql)
        {
            await connection.ExecuteAsync(sql, commandTimeout: commandTimeout);
        }
    }

    public static void DeleteDatabaseFile()
    {
        string path = SqlitePaths.GetDatabasePath();
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
