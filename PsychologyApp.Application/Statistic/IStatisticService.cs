using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.Statistic;

public interface IStatisticService
{
    Task AddSingleAsync(StatisticDTO statisticDTO, CancellationToken cancellationToken = default);
    Task<long> CountPageCompletedAsync(CancellationToken cancellationToken = default);
}
