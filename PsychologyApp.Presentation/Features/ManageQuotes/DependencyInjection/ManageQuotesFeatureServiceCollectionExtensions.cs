using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;

namespace PsychologyApp.Presentation.Features.ManageQuotes.DependencyInjection;

public static class ManageQuotesFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddManageQuotesFeature(this IServiceCollection services)
    {
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<QuoteFeedCoordinator>(services);
        services.AddSingleton<QuoteFeedLoader>();
        services.AddSingleton<QuoteItemCommandsFactory>();
        services.AddSingleton<IQuotesChangeNotifier, QuotesChangeNotifier>();
        services.AddSingleton<IQuoteViewModelFactory, QuoteViewModelFactory>();
        return services;
    }
}
