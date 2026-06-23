using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.MusicPlayer;

public partial class MusicPlayerViewModel : BaseViewModel
{
    private readonly ILogger<MusicPlayerViewModel> _logger;
    private readonly IAudioPlaybackService _playbackService;
    private readonly MusicPlaylistPresenter _playlistPresenter;
    private readonly MusicPlaybackPresenter _playbackPresenter;
    private Audio? _activeTrack;
    private string search_text = string.Empty;
    private string _selectedCategoryKey = string.Empty;
    private bool _isPlaying;
    private bool _isBuffering;
    private string _positionDisplay = "0:00";
    private string _durationDisplay = "0:00";
    private double _progressFraction;

    public ObservableCollection<Audio> AllItems { get; } = [];
    public ObservableCollection<Audio> FilteredItems { get; } = [];
    public ObservableCollection<FilterChipTabItem> CategoryFilters { get; } = [];

    public ICommand TogglePlayPauseCommand { get; }
    public ICommand PlayNextCommand { get; }
    public ICommand PlayPreviousCommand { get; }
    public ICommand SelectCategoryCommand { get; }

    public MusicPlayerViewModel(
        ILogger<MusicPlayerViewModel> logger,
        IAudioPlaybackService playbackService,
        MusicPlaylistPresenter playlistPresenter,
        MusicPlaybackPresenter playbackPresenter)
    {
        _logger = logger;
        _playbackService = playbackService;
        _playlistPresenter = playlistPresenter;
        _playbackPresenter = playbackPresenter;
        _playbackService.PlaybackEnded += (_, _) => SetPlaybackState(false);
        _playbackService.PlaybackFailed += (_, _) =>
        {
            SetPlaybackState(false);
            IsBuffering = false;
        };
        ModuleName = AppStrings.ShellTabCleanerShort;
        PageName = AppStrings.CleanerPrayersPage;

        TogglePlayPauseCommand = new AsyncCommand(async () =>
        {
            bool isPlaying = await _playbackPresenter.TogglePlayPauseAsync(_playbackService, IsPlaying);
            SetPlaybackState(isPlaying);
        });
        PlayNextCommand = new AsyncCommand(PlayNextAsync);
        PlayPreviousCommand = new AsyncCommand(PlayPreviousAsync);
        SelectCategoryCommand = new Command<string?>(SelectCategory);
        Reload = new AsyncCommand(() =>
        {
            InitializePlaylist();
            return Task.CompletedTask;
        });

        InitializePlaylist();
    }
}
