using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Statistics;

public sealed class StatisticRepository : BaseRepository<Statistic>, IStatisticRepository
{
    public StatisticRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, EntitySqlMaps.Statistic, settings)
    {
    }

    public async Task<long> CountDistinctPagesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            "SELECT COUNT(DISTINCT PageName) FROM Statistics;",
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<long> CountByPageNameAsync(string pageName, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            "SELECT COUNT(*) FROM Statistics WHERE PageName = @pageName;",
            new { pageName },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Statistic>> GetRecentAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Statistic>(DapperCommandFactory.Create(
            "SELECT * FROM Statistics ORDER BY StatisticId DESC LIMIT @limit;",
            new { limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }
}
