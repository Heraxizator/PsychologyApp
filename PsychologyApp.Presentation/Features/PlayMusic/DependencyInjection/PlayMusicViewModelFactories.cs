using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;

namespace PsychologyApp.Presentation.Features.PlayMusic.DependencyInjection;

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
