using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Views.Clean;

public partial class MusicPlayerPage : ContentPage
{
    private readonly MusicPlayerViewModel ViewModel;
    private PageAnimationHelper? _animationHelper;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory)
    {
        InitializeComponent();

        ViewModel = musicPlayerViewModelFactory.Create();
        BindingContext = ViewModel;

        ViewModel.PlayAudioHandler = PlayUrlAsync;
        ViewModel.TogglePlaybackHandler = TogglePlaybackAsync;

        _animationHelper = new PageAnimationHelper(ViewModel, Progress, Musics, introView: IntroPrompt);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }

    private async Task PlayUrlAsync(string url)
    {
        string playbackUri = await MusicAudioCache.ResolvePlaybackUriAsync(url);
        Player.Source = MediaSource.FromUri(playbackUri);
        Player.Play();
        ViewModel.SetPlaybackState(true);
    }

    private Task TogglePlaybackAsync()
    {
        if (ViewModel.IsPlaying)
        {
            Player.Pause();
            ViewModel.SetPlaybackState(false);
        }
        else
        {
            Player.Play();
            ViewModel.SetPlaybackState(true);
        }

        return Task.CompletedTask;
    }

    private void OnMediaEnded(object? sender, EventArgs e) =>
        ViewModel.SetPlaybackState(false);
}
