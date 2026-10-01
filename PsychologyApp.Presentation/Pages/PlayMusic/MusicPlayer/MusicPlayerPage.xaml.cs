using PsychologyApp.Presentation.Shared.Common.Diagnostics;
using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Features.PlayMusic.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;
using System.ComponentModel;

namespace PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;

public partial class MusicPlayerPage : ContentPage
{
    private readonly MusicPlayerViewModel _viewModel;
    private readonly IToastService _toastService;
    private readonly MediaElementAudioPlaybackService _playbackService;
    private PageAnimationHelper? _animationHelper;
    private IDispatcherTimer? _positionTimer;
    private MediaElement? _player;
    private CancellationTokenSource? _prefetchCts;
    private bool _isSeeking;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory, IToastService toastService)
    {
        InitializeComponent();

        _toastService = toastService;
        _playbackService = new MediaElementAudioPlaybackService();
        _playbackService.PlaybackFailed += (_, _) =>
            _toastService.LongToast(AppStrings.CleanerPlaybackError);

        _viewModel = musicPlayerViewModelFactory.Create(this, _playbackService);
        BindingContext = _viewModel;

        _playbackService.AttachLazily(CreatePlayer);
        _animationHelper = new PageAnimationHelper(_viewModel, contentView: Musics);
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MusicPlayerViewModel.PlaylistContentVersion))
        {
            UiStateAnimator.CrossfadeContentRefreshAsync(Musics).FireAndForget();
        }
    }

    private MediaElement CreatePlayer()
    {
        _player = new MediaElement
        {
            IsVisible = false,
            ShouldAutoPlay = true
        };
        MainContentGrid.Children.Add(_player);
        return _player;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _playbackService.AttachLazily(CreatePlayer);
        _animationHelper?.TryRevealAsync();
        _prefetchCts = new CancellationTokenSource();
        PrefetchPlaylistAsync(_prefetchCts.Token).FireAndForget();
        StartPositionTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Cancel() runs the token callbacks on this (UI) thread, and those tear down an in-flight HTTPS download
        // through SslStream/JNI: leaving the tab froze the app for ~2 s. CancelAsync runs them on the pool.
        if (_prefetchCts is { } prefetch)
        {
            _prefetchCts = null;
            prefetch.CancelAsync().ContinueWith(_ => prefetch.Dispose(), TaskScheduler.Default);
        }
        StopPositionTimer();
        _playbackService.PauseAsync().FireAndForget();
        _viewModel.SetPlaybackState(false);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            StopPositionTimer();
            _playbackService.Detach();
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }

    private void OnProgressDragStarted(object? sender, EventArgs e)
    {
        _isSeeking = true;
        _playbackService.PauseAsync().FireAndForget();
    }

    private void OnProgressDragCompleted(object? sender, EventArgs e)
    {
        _isSeeking = false;
        _viewModel.SeekToFractionAsync(ProgressSlider.Value).FireAndForget();
    }

    // Only someone who stays on the music tab gets the playlist downloaded. Starting it on every visit meant that
    // just passing through the tab kicked off DNS, TLS and multi-MB downloads, and the device log showed 1-2 s UI
    // stalls on the next tab every time.
    private const int PrefetchDelayMs = 3000;

    private async Task PrefetchPlaylistAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(PrefetchDelayMs, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        PerfTrace.Mark("Music prefetch started");
        string[] urls = _viewModel.AllItems
            .Select(item => item.URL)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>()
            .ToArray();

        // Downloading and hashing a whole playlist is background work; only the flag refresh needs the UI thread.
        try
        {
            await Task.Run(() => MusicAudioCache.PrefetchAsync(urls, cancellationToken), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            PerfTrace.Mark("Music prefetch cancelled");
            return;
        }

        PerfTrace.Mark("Music prefetch finished");
        _viewModel.RefreshCacheFlags();
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

            _viewModel.UpdatePlaybackProgressFromService();
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
