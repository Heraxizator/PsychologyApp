using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Quots;

public sealed class QuotRepository : BaseRepository<Quot>, IQuotRepository
{
    public QuotRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, EntitySqlMaps.Quot, settings)
    {
    }

    public async Task<IEnumerable<Quot>> GetUnreadLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE IsReaded = 0 ORDER BY QuotId DESC LIMIT @count;",
            new { count },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots ORDER BY QuotId DESC LIMIT @count;",
            new { count },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE IsFavourite = 1 ORDER BY QuotId DESC LIMIT @count;",
            new { count },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetUnreadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default)
    {
        if (themes.Count == 0)
        {
            return [];
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE IsReaded = 0 AND Theme IN @themes ORDER BY QuotId DESC LIMIT @count;",
            new { themes, count },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetReadByThemesAsync(
        IReadOnlyList<string> themes,
        int count,
        CancellationToken cancellationToken = default)
    {
        if (themes.Count == 0)
        {
            return [];
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE IsReaded = 1 AND Theme IN @themes ORDER BY QuotId DESC LIMIT @count;",
            new { themes, count },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetByThemeAsync(
        string theme,
        int count,
        CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE Theme = @theme ORDER BY QuotId DESC LIMIT @count;",
            new { theme, count },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<Quot?> GetByTextAsync(string text, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE Text = @text ORDER BY QuotId DESC LIMIT 1;",
            new { text },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(DapperCommandFactory.Create(
            "SELECT COUNT(*) FROM Quots;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<int> CountUnreadAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(DapperCommandFactory.Create(
            "SELECT COUNT(*) FROM Quots WHERE IsReaded = 0;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task ResetReadStateAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            "UPDATE Quots SET IsReaded = 0;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            "DELETE FROM Quots;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<string>> GetExistingTextsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<string> rows = await connection.QueryAsync<string>(DapperCommandFactory.Create(
            "SELECT DISTINCT Text FROM Quots;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<string>> GetFavoriteTextsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<string> rows = await connection.QueryAsync<string>(DapperCommandFactory.Create(
            "SELECT DISTINCT Text FROM Quots WHERE IsFavourite = 1;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
