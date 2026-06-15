using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Clean;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Views.Clean;

public partial class MusicPlayerPage : ContentPage
{
    private readonly MusicPlayerViewModel _viewModel;
    private readonly IToastService _toastService;
    private readonly MediaElementAudioPlaybackService _playbackService;
    private PageAnimationHelper? _animationHelper;
    private IDispatcherTimer? _positionTimer;
    private MediaElement? _player;
    private bool _isSeeking;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory, IToastService toastService)
    {
        InitializeComponent();

        _toastService = toastService;
        _playbackService = new MediaElementAudioPlaybackService();
        _playbackService.PlaybackFailed += (_, _) =>
            _toastService.LongToast(AppStrings.CleanerPlaybackError);

        _viewModel = musicPlayerViewModelFactory.Create(_playbackService);
        BindingContext = _viewModel;

        EnsurePlayer();
        _animationHelper = new PageAnimationHelper(_viewModel, contentView: Musics);
    }

    private MediaElement EnsurePlayer()
    {
        if (_player is null)
        {
            _player = new MediaElement
            {
                IsVisible = false,
                ShouldAutoPlay = true
            };
            MainContentGrid.Children.Add(_player);
        }

        _playbackService.Attach(_player);
        return _player;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EnsurePlayer();
        _animationHelper?.TryRevealAsync();
        PrefetchPlaylistAsync().FireAndForget();
        StartPositionTimer();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
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

    private async Task PrefetchPlaylistAsync()
    {
        IEnumerable<string> urls = _viewModel.AllItems
            .Select(item => item.URL)
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Cast<string>();

        await MusicAudioCache.PrefetchAsync(urls);
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
