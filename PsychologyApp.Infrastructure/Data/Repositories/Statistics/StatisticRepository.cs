using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Statistics;

public sealed class StatisticRepository : BaseRepository<Statistic>, IStatisticRepository
{
    private readonly SqliteConnection _connection;

    public StatisticRepository(SqliteConnection connection) : base(connection)
    {
        _connection = connection;
    }
}
