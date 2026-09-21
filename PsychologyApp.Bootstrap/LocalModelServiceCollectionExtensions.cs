using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Infrastructure.LocalModel;

namespace PsychologyApp.Bootstrap;

public static class LocalModelServiceCollectionExtensions
{
    /// <summary>Registers the consent-based downloader for the on-device model. <paramref name="targetDirectory"/> is where the runtime looks for it.</summary>
    public static IServiceCollection AddPsychologyAppLocalModel(this IServiceCollection services, string targetDirectory)
    {
        services.AddSingleton<ILocalModelInstaller>(_ => new HttpLocalModelInstaller(
            // Downloads are large and slow: rely on the caller's cancellation token, not a client-wide timeout.
            new HttpClient { Timeout = Timeout.InfiniteTimeSpan },
            LocalModelCatalog.Default,
            targetDirectory));

        return services;
    }
}
