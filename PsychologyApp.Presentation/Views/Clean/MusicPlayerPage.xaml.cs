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
    private bool _isSeeking;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory, IToastService toastService)
    {
        InitializeComponent();

        _toastService = toastService;
        _viewModel = musicPlayerViewModelFactory.Create();
        BindingContext = _viewModel;

        _viewModel.PlayAudioHandler = PlayUrlAsync;
        _viewModel.TogglePlaybackHandler = TogglePlaybackAsync;
        _viewModel.SeekHandler = SeekToFractionAsync;
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

    private void OnProgressDragStarted(object? sender, EventArgs e)
    {
        _isSeeking = true;
        Player.Pause();
    }

    private void OnProgressDragCompleted(object? sender, EventArgs e)
    {
        _isSeeking = false;
        SeekToFractionAsync(ProgressSlider.Value).FireAndForget();
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

    private async Task SeekToFractionAsync(double fraction)
    {
        MediaElement player = Player;
        if (player.Duration.TotalSeconds <= 0)
        {
            return;
        }

        double clamped = Math.Clamp(fraction, 0, 1);
        TimeSpan target = TimeSpan.FromSeconds(player.Duration.TotalSeconds * clamped);
        await player.SeekTo(target);

        _viewModel.UpdatePlaybackProgress(player.Position, player.Duration);
        if (_viewModel.IsPlaying)
        {
            player.Play();
        }
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
            if (_player is null || _isSeeking)
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
