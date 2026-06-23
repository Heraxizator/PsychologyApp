using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Statistic;

public sealed class StatisticService(IStatisticRepository statisticRepository) : IStatisticService
{
    public async Task AddSingleAsync(StatisticDTO statisticDTO, CancellationToken cancellationToken = default)
    {
        global::PsychologyApp.Domain.Entities.Statistic statistic = StatisticMapper.GetStatistic(statisticDTO);
        await statisticRepository.AddAsync(statistic, cancellationToken);
    }

    public async Task<long> CountPageCompletedAsync(CancellationToken cancellationToken = default) =>
        await statisticRepository.CountDistinctPagesAsync(cancellationToken);
}
