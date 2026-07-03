using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.PlayMusic.DependencyInjection;

public interface IMusicPlayerViewModelFactory
{
    MusicPlayerViewModel Create(ContentPage page, IAudioPlaybackService playbackService);
}

public sealed class MusicPlayerViewModelFactory(
    ILogger<MusicPlayerViewModel> logger,
    MusicPlaylistPresenter playlistPresenter,
    MusicPlaybackPresenter playbackPresenter,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IMusicPlayerViewModelFactory
{
    public MusicPlayerViewModel Create(ContentPage page, IAudioPlaybackService playbackService) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            logger,
            playbackService,
            playlistPresenter,
            playbackPresenter);
}
