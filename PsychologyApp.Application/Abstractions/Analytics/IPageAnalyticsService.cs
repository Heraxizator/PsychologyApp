namespace PsychologyApp.Application.Abstractions.Analytics;

public interface IPageAnalyticsService
{
    Task TrackPageVisitAsync(string moduleName, string pageName, DateTime startedAt, CancellationToken cancellationToken = default);
}
