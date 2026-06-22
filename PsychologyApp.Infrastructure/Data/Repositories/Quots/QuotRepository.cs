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
    private readonly int _commandTimeoutSeconds;

    public QuotRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, EntitySqlMaps.Quot, settings)
    {
        _commandTimeoutSeconds = settings.Value.DbCommandTimeoutSeconds > 0
            ? settings.Value.DbCommandTimeoutSeconds
            : 30;
    }

    public async Task<IEnumerable<Quot>> GetUnreadLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE IsReaded = 0 ORDER BY QuotId DESC LIMIT @count;",
            new { count },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots ORDER BY QuotId DESC LIMIT @count;",
            new { count },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Quot>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Quot>(DapperCommandFactory.Create(
            "SELECT * FROM Quots WHERE IsFavourite = 1 ORDER BY QuotId DESC LIMIT @count;",
            new { count },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            "DELETE FROM Quots;",
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }
}
