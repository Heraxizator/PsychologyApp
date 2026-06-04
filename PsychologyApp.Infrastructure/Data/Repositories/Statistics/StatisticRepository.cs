using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Statistics;

public sealed class StatisticRepository : BaseRepository<Statistic>, IStatisticRepository
{
    public StatisticRepository(SqliteConnection connection)
        : base(connection, EntitySqlMaps.Statistic)
    {
    }
}
