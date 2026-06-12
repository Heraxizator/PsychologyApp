using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Views.Clean;

public partial class MusicPlayerPage : ContentPage
{
    private readonly MusicPlayerViewModel _viewModel;
    private readonly IToastService _toastService;
    private PageAnimationHelper? _animationHelper;
    private IDispatcherTimer? _positionTimer;
    private MediaElement? _player;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory, IToastService toastService)
    {
        InitializeComponent();

        _toastService = toastService;
        _viewModel = musicPlayerViewModelFactory.Create();
        BindingContext = _viewModel;

        _viewModel.PlayAudioHandler = PlayUrlAsync;
        _viewModel.TogglePlaybackHandler = TogglePlaybackAsync;
        _viewModel.PrefetchHandler = PrefetchPlaylistAsync;

        _animationHelper = new PageAnimationHelper(_viewModel, contentView: Musics);
    }

    private MediaElement Player => EnsurePlayer();

    private MediaElement EnsurePlayer()
    {
        if (_player is not null)
        {
            return _player;
        }

        _player = new MediaElement
        {
            IsVisible = false,
            ShouldAutoPlay = true
        };
        _player.MediaEnded += OnMediaEnded;
        _player.MediaFailed += OnMediaFailed;
        MainContentGrid.Children.Add(_player);
        return _player;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
        _viewModel.PrefetchHandler?.Invoke().FireAndForget();
        StartPositionTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopPositionTimer();
        if (_player is not null)
        {
            _player.Pause();
        }

        _viewModel.SetPlaybackState(false);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            StopPositionTimer();
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }

    private async Task PrefetchPlaylistAsync()
    {
        IEnumerable<string> urls = _viewModel.AllItems
            .Select(item => item.URL)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>();

        await MusicAudioCache.PrefetchAsync(urls);
        _viewModel.RefreshCacheFlags();
    }

    private async Task PlayUrlAsync(string url)
    {
        MediaElement player = Player;
        _viewModel.IsBuffering = true;

        try
        {
            AudioCacheResult result = await MusicAudioCache.ResolvePlaybackUriAsync(url);
            if (result.DownloadFailed)
            {
                _toastService.LongToast(AppStrings.CleanerPlaybackError);
                _viewModel.SetPlaybackState(false);
                return;
            }

            player.Source = MediaSource.FromUri(result.Uri);
            player.Play();
            _viewModel.SetPlaybackState(true);
            _viewModel.RefreshCacheFlags();
        }
        catch (Exception)
        {
            _toastService.LongToast(AppStrings.CleanerPlaybackError);
            _viewModel.SetPlaybackState(false);
        }
        finally
        {
            _viewModel.IsBuffering = false;
        }
    }

    private Task TogglePlaybackAsync()
    {
        MediaElement player = Player;

        if (_viewModel.IsPlaying)
        {
            player.Pause();
            _viewModel.SetPlaybackState(false);
        }
        else
        {
            player.Play();
            _viewModel.SetPlaybackState(true);
        }

        return Task.CompletedTask;
    }

    private void OnMediaEnded(object? sender, EventArgs e) =>
        _viewModel.SetPlaybackState(false);

    private void OnMediaFailed(object? sender, EventArgs e)
    {
        _viewModel.SetPlaybackState(false);
        _viewModel.IsBuffering = false;
        _toastService.LongToast(AppStrings.CleanerPlaybackError);
    }

    private void StartPositionTimer()
    {
        StopPositionTimer();
        _positionTimer = Dispatcher.CreateTimer();
        _positionTimer.Interval = TimeSpan.FromMilliseconds(500);
        _positionTimer.Tick += (_, _) =>
        {
            if (_player is null)
            {
                return;
            }

            _viewModel.UpdatePlaybackProgress(_player.Position, _player.Duration);
        };
        _positionTimer.Start();
    }

    private void StopPositionTimer()
    {
        if (_positionTimer is null)
        {
            return;
        }

        _positionTimer.Stop();
        _positionTimer = null;
    }
}
