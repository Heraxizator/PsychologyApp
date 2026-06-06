using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.Statistic;
using DomainStatistic = PsychologyApp.Domain.Entities.Statistic;

namespace PsychologyApp.Application.Services.Statistic;

public sealed class StatisticService(IStatisticRepository statisticRepository) : IStatisticService
{
    public async Task AddSingleAsync(StatisticDTO statisticDTO, CancellationToken cancellationToken = default)
    {
        DomainStatistic statistic = StatisticMapper.GetStatistic(statisticDTO);
        await statisticRepository.AddAsync(statistic, cancellationToken);
    }

    public async Task<long> CountPageCompletedAsync(CancellationToken cancellationToken = default) =>
        await statisticRepository.CountDistinctPagesAsync(cancellationToken);
}
