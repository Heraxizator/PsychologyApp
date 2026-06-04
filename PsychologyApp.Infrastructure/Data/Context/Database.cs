using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Infrastructure.Data.Repositories.Quots;
using PsychologyApp.Infrastructure.Data.Repositories.Reasons;
using PsychologyApp.Infrastructure.Data.Repositories.Statistics;
using PsychologyApp.Infrastructure.Data.Repositories.Techniques;

namespace PsychologyApp.Infrastructure.Data.Context;

public static class Database
{
    private static readonly SqliteConnection _connection = new($"Data Source={SqlitePaths.GetDatabasePath()}");

    public static readonly QuotRepository QuotRepository = new(_connection);
    public static readonly ReasonRepository ReasonRepository = new(_connection);
    public static readonly TechniqueRepository TechniqueRepository = new(_connection);
    public static readonly StatisticRepository StatisticRepository = new(_connection);

    public static async Task CreateTablesAsync(int commandTimeout = 0)
    {
        await SqliteSchema.CreateTablesAsync(_connection, commandTimeout);
    }

    public static async Task DeleteTablesAsync(int commandTimeout = 0)
    {
        await SqliteSchema.DropTablesAsync(_connection, commandTimeout);
    }

    public static async Task ReCreateTablesAsync(int commandTimeout = 0)
    {
        await _connection.CloseAsync();

        SqliteSchema.DeleteDatabaseFile();

        await _connection.OpenAsync();
        await SqliteSchema.CreateTablesAsync(_connection, commandTimeout);
    }

    public static void ConfigureSQLite()
    {
        _connection.Open();

        _connection.Execute("PRAGMA synchronous=OFF;");
        _connection.Execute("PRAGMA journal_mode=OFF;");

        SqliteSchema.CreateTablesAsync(_connection).GetAwaiter().GetResult();
    }

    public static void CreateTables() => CreateTablesAsync().GetAwaiter().GetResult();

    public static void DeleteTables() => DeleteTablesAsync().GetAwaiter().GetResult();

    public static void ReCreateTables() => ReCreateTablesAsync().GetAwaiter().GetResult();
}
