using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Services.QuotService;

namespace PsychologyApp.Application.Startup;

public sealed class AppStartupService(
    IDatabaseInitializer databaseInitializer,
    IQuotService quotService,
    IOptions<AppSettings> settings,
    ILogger<AppStartupService> logger) : IAppStartupService
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await databaseInitializer.InitializeAsync(cancellationToken);

        try
        {
            using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(settings.Value.MiddleTimeoutMs);
            await quotService.LoadSingleAsync(timeoutSource.Token);
        }
        catch (Exception ex) when (ex is QuotApiLoadException or HttpRequestException or OperationCanceledException)
        {
            logger.LogWarning(ex, "Preload quotes failed; app can continue.");
        }
    }
}
