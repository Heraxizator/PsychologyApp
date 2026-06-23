using PsychologyApp.Presentation.Features.PlayMusic;

namespace PsychologyApp.Presentation.App.Providers;

public static class PlayMusicFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddPlayMusicFeature(this IServiceCollection services)
    {
        services.AddSingleton<MusicPlaylistPresenter>();
        services.AddSingleton<MusicPlaybackPresenter>();
        services.AddSingleton<IMusicPlayerViewModelFactory, MusicPlayerViewModelFactory>();
        return services;
    }
}
