using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Pages.QuoteFeed;

namespace PsychologyApp.Presentation.App.Providers;

public interface IQuoteViewModelFactory
{
    QuoteViewModel Create(ContentPage page);
}

public sealed class QuoteViewModelFactory(
    IQuotService quotService,
    ILogger<QuoteViewModel> logger,
    IOptions<AppSettings> settings,
    IDatabaseReadySignal databaseReadySignal,
    Func<QuoteFeedCoordinator> feedCoordinatorFactory,
    QuoteItemCommandsFactory quoteCommandsFactory,
    QuoteFeedLoader quoteFeedLoader,
    LanguageContentReloader languageContentReloader,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IQuoteViewModelFactory
{
    public QuoteViewModel Create(ContentPage page) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            quotService,
            logger,
            settings,
            feedCoordinatorFactory(),
            quoteCommandsFactory,
            quoteFeedLoader,
            databaseReadySignal,
            languageContentReloader);
}
