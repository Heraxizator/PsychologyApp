using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Statistic;

public interface IStatisticService
{
    Task AddSingleAsync(StatisticDTO statisticDTO, CancellationToken cancellationToken = default);
    Task<long> CountPageCompletedAsync(CancellationToken cancellationToken = default);

    /// <summary>Drops page-visit history older than <paramref name="retention"/> (the latest visit of every page is kept).</summary>
    Task<int> PruneAsync(TimeSpan retention, CancellationToken cancellationToken = default);
}
