using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.Statistic;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application;

public sealed class StatisticService : IStatisitcService
{
    public async Task AddSingleAsync(StatisticDTO statisticDTO, int cancelTimeout)
    {
        Statistic statistic = StatisticMapper.GetStatistic(statisticDTO);

        await Database.StatisticRepository.AddAsync(statistic, cancelTimeout);
    }

    public async Task<long> CountPageCompletedAsync(int cancelTimeout)
    {
        IEnumerable<Statistic> statistics = await Database.StatisticRepository.GetAllAsync(cancelTimeout);

        return statistics.DistinctBy(x => x.PageName).Count();
    }

    public async Task<long> CountPageFactsAsync(string pageName, int cancelTimeout)
    {
        IEnumerable<Statistic> statistics = await Database.StatisticRepository.GetAllAsync(cancelTimeout);

        return statistics.Where(x => x.PageName == pageName).Count();
    }

    public async Task<IEnumerable<StatisticDTO>> GetAllAsync(int cancelTimeout)
    {
        IEnumerable<Statistic> statistics = await Database.StatisticRepository.GetAllAsync(cancelTimeout);

        return statistics.Select(StatisticMapper.GetStatisticDTO);
    }
}
