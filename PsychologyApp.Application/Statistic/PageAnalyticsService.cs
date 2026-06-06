using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.Statistic;

namespace PsychologyApp.Application.Analytics;

public sealed class PageAnalyticsService(IStatisticService statisticService, TimeProvider timeProvider) : IPageAnalyticsService
{
    public async Task TrackPageVisitAsync(string moduleName, string pageName, DateTime startedAt, CancellationToken cancellationToken = default)
    {
        int seconds = Math.Max(0, (int)timeProvider.GetUtcNow().UtcDateTime.Subtract(startedAt.ToUniversalTime()).TotalSeconds);
        var statisticDto = new StatisticDTO
        {
            ModuleName = moduleName,
            PageName = pageName,
            DateTime = startedAt,
            SecondsDuration = seconds
        };

        await statisticService.AddSingleAsync(statisticDto, cancellationToken);
    }
}
