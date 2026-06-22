using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.ViewModels.Motivator;
using PsychologyApp.Presentation.Services.Clean;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IQuoteViewModelFactory
{
    QuoteViewModel Create(INavigation navigation);
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
    public QuoteViewModel Create(INavigation navigation) =>
        new(
            ResolveNavigation(navigationServiceFactory, navigation),
            quotService,
            logger,
            settings,
            feedCoordinatorFactory(),
            quoteCommandsFactory,
            quoteFeedLoader,
            databaseReadySignal,
            languageContentReloader);
}

public interface IMusicPlayerViewModelFactory
{
    MusicPlayerViewModel Create(IAudioPlaybackService playbackService);
}

public sealed class MusicPlayerViewModelFactory(
    ILogger<MusicPlayerViewModel> logger,
    MusicPlaylistPresenter playlistPresenter,
    MusicPlaybackPresenter playbackPresenter) : IMusicPlayerViewModelFactory
{
    public MusicPlayerViewModel Create(IAudioPlaybackService playbackService) =>
        new(logger, playbackService, playlistPresenter, playbackPresenter);
}
