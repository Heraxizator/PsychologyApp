using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PsychologyApp.Domain.Common;
using PsychologyApp.Infrastructure.Data.Repositories.Quots;
using PsychologyApp.Infrastructure.Data.Repositories.Reasons;
using PsychologyApp.Infrastructure.Data.Repositories.Statistics;
using PsychologyApp.Infrastructure.Data.Repositories.Techniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Context;

public static class Database
{
    private static readonly SqliteConnection _connection = new($"Data Source={ApplicationDbContext.GetPathDB()}");

    #region Repositories
    public static readonly QuotRepository QuotRepository = new(_connection);

    public static readonly ReasonRepository ReasonRepository = new(_connection);

    public static readonly TechniqueRepository TechniqueRepository = new(_connection);

    public static readonly StatisticRepository StatisticRepository = new(_connection);
    #endregion

    static Database()
    {

    }

    public static void CreateTables()
    {
        ApplicationDbContext context = new();

        context.Database.EnsureCreated();

        context.Database.CloseConnection();
    }

    public static void DeleteTables()
    {
        ApplicationDbContext context = new();

        context.Database.EnsureDeleted();

        context.Database.CloseConnection();
    }

    public static void ReCreateTables()
    {
        ApplicationDbContext context = new();

        context.Database.EnsureDeleted();

        context.Database.EnsureCreated();

        context.Database.CloseConnection();
    }

    public static void ConfigureSQLite()
    {
        _connection.Open();

        _connection.Execute("PRAGMA synchronous=OFF;");

        _connection.Execute("PRAGMA journal_mode=OFF;");
    }
}
