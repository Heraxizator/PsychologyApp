using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Quot;

namespace PsychologyApp.Application.Startup;

public sealed class AppStartupService(
    IDatabaseInitializer databaseInitializer,
    IQuotService quotService,
    IQuoteCatalogVersionStore quoteCatalogVersionStore,
    ITechniqueCatalogService techniqueCatalogService,
    IOptions<AppSettings> settings,
    ILogger<AppStartupService> logger) : IAppStartupService
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await databaseInitializer.InitializeAsync(cancellationToken);

        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(settings.Value.MiddleTimeoutMs);

        await Task.WhenAll(
            SeedQuotesAsync(timeoutSource.Token),
            PrewarmTechniqueCatalogAsync(timeoutSource.Token));
    }

    private async Task SeedQuotesAsync(CancellationToken cancellationToken)
    {
        try
        {
            int persistedVersion = await quoteCatalogVersionStore.GetAsync(cancellationToken);
            if (persistedVersion < QuoteCatalogPolicy.CurrentVersion)
            {
                await quotService.ReseedFeedAsync(QuoteCatalogPolicy.DefaultFeedSeedCount, cancellationToken);
                await quoteCatalogVersionStore.SetAsync(QuoteCatalogPolicy.CurrentVersion, cancellationToken);
            }
            else
            {
                await quotService.LoadSingleAsync(cancellationToken);
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or OperationCanceledException or JsonException)
        {
            logger.LogError(ex, "Preload quotes failed; app can continue.");
        }
    }

    private async Task PrewarmTechniqueCatalogAsync(CancellationToken cancellationToken)
    {
        try
        {
            await techniqueCatalogService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is InvalidOperationException or OperationCanceledException or JsonException)
        {
            logger.LogError(ex, "Preload technique catalog failed; app can continue.");
        }
    }
}
