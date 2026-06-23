using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;

namespace PsychologyApp.Presentation.Shared.Common;

public static class OperationCancellation
{
    public static CancellationTokenSource CreateTimeoutSource(IOptions<AppSettings> settings, int timeoutMs)
    {
        int effectiveTimeout = timeoutMs > 0 ? timeoutMs : settings.Value.MiddleTimeoutMs;
        return new CancellationTokenSource(effectiveTimeout);
    }

    public static CancellationTokenSource CreateSmallTimeoutSource(IOptions<AppSettings> settings) =>
        CreateTimeoutSource(settings, settings.Value.SmallTimeoutMs);

    public static CancellationTokenSource CreateMiddleTimeoutSource(IOptions<AppSettings> settings) =>
        CreateTimeoutSource(settings, settings.Value.MiddleTimeoutMs);

    public static CancellationTokenSource CreateLargeTimeoutSource(IOptions<AppSettings> settings) =>
        CreateTimeoutSource(settings, settings.Value.LargeTimeoutMs);
}
