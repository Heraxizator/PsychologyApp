using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Clean;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Clean;

public class MusicPlayerViewModel : BaseViewModel
{
    private readonly ILogger<MusicPlayerViewModel> _logger;
    private Audio? _activeTrack;
    private string search_text = string.Empty;
    private bool _isPlaying;
    private bool _isBuffering;
    private string _positionDisplay = "0:00";
    private string _durationDisplay = "0:00";

    public ObservableCollection<Audio> AllItems { get; } = [];
    public ObservableCollection<Audio> FilteredItems { get; } = [];

    public Func<string, Task>? PlayAudioHandler { get; set; }
    public Func<Task>? TogglePlaybackHandler { get; set; }
    public Func<Task>? PrefetchHandler { get; set; }
    public ICommand TogglePlayPauseCommand { get; }
    public ICommand PlayNextCommand { get; }

    public string PageTitle => AppStrings.ShellTabCleanerShort;
    public string SearchPlaceholder => AppStrings.CleanerSearchPlaceholder;
    public string MoreInfoHeader => AppStrings.TestsMoreInfo;
    public string MoreInfoBody => AppStrings.CleanerMoreInfoBody;
    public string NoPrayersFoundText => AppStrings.CleanerNoPrayersFound;
    public string NowPlayingLabel => AppStrings.CleanerNowPlaying;
    public string BufferingText => AppStrings.CleanerPreparingAudio;
    public string OfflineBadgeText => AppStrings.CleanerOfflineBadge;
    public string PlayNextButtonText => AppStrings.CleanerPlayNext;
    public string ReplayButtonText => AppStrings.CleanerReplay;
    public string LoadFailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    public Audio? ActiveTrack
    {
        get => _activeTrack;
        private set
        {
            if (_activeTrack == value)
            {
                return;
            }

            _activeTrack = value;
            OnPropertyChanged(nameof(ActiveTrack));
            OnPropertyChanged(nameof(HasActiveTrack));
            OnPropertyChanged(nameof(ActiveTrackTitle));
            SyncActiveTrackFlags();
        }
    }

    public bool HasActiveTrack => ActiveTrack is not null;
    public string ActiveTrackTitle => ActiveTrack?.Name ?? string.Empty;
    public bool ShowBufferingOverlay => IsBuffering && HasActiveTrack;

    public bool IsPlaying
    {
        get => _isPlaying;
        private set
        {
            if (_isPlaying == value)
            {
                return;
            }

            _isPlaying = value;
            OnPropertyChanged(nameof(IsPlaying));
            OnPropertyChanged(nameof(PlayPauseGlyph));
            SyncActiveTrackFlags();
        }
    }

    public bool IsBuffering
    {
        get => _isBuffering;
        set
        {
            if (SetProperty(ref _isBuffering, value))
            {
                OnPropertyChanged(nameof(ShowBufferingOverlay));
            }
        }
    }

    public string PlayPauseGlyph => IsPlaying ? "⏸" : "▶";

    public string PositionDisplay
    {
        get => _positionDisplay;
        set => SetProperty(ref _positionDisplay, value);
    }

    public string DurationDisplay
    {
        get => _durationDisplay;
        set => SetProperty(ref _durationDisplay, value);
    }

    public string ProgressDisplay => $"{PositionDisplay} / {DurationDisplay}";

    public bool HasFilteredItems => FilteredItems.Count > 0;
    public bool IsSearchEmptyVisible => IsDone && !string.IsNullOrWhiteSpace(SearchText) && FilteredItems.Count == 0;

    public MusicPlayerViewModel(ILogger<MusicPlayerViewModel> logger)
    {
        _logger = logger;
        ModuleName = AppStrings.ShellTabCleanerShort;
        PageName = AppStrings.CleanerPrayersPage;

        TogglePlayPauseCommand = new AsyncCommand(TogglePlayPauseAsync);
        PlayNextCommand = new AsyncCommand(PlayNextAsync);
        Reload = new AsyncCommand(() =>
        {
            InitializePlaylist();
            return Task.CompletedTask;
        });

        InitializePlaylist();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SearchPlaceholder),
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(NoPrayersFoundText),
            nameof(NowPlayingLabel),
            nameof(BufferingText),
            nameof(OfflineBadgeText),
            nameof(PlayNextButtonText),
            nameof(ReplayButtonText),
            nameof(PlayPauseGlyph),
            nameof(ProgressDisplay));

        if (IsDone && AllItems.Count > 0)
        {
            ObservableCollection<Audio> localized = MusicPlaylist.CreateDefault();
            for (int i = 0; i < AllItems.Count && i < localized.Count; i++)
            {
                AllItems[i].Name = localized[i].Name;
                AllItems[i].Description = localized[i].Description;
            }

            ApplyFilter();
        }
    }

    public void InitializePlaylist()
    {
        try
        {
            InitItems(MusicPlaylist.CreateDefault());
            SetDone();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize music playlist.");
            SetFail();
        }
    }

    public void RefreshCacheFlags()
    {
        foreach (Audio item in AllItems)
        {
            item.IsCached = !string.IsNullOrWhiteSpace(item.URL) && MusicAudioCache.IsCached(item.URL);
        }
    }

    public void UpdatePlaybackProgress(TimeSpan position, TimeSpan duration)
    {
        PositionDisplay = FormatTime(position);
        DurationDisplay = FormatTime(duration);
        OnPropertyChanged(nameof(ProgressDisplay));
    }

    private void InitItems(ObservableCollection<Audio> collection)
    {
        AllItems.Clear();

        foreach (Audio item in collection)
        {
            item.ClickCommand = new AsyncCommand(() => PlayTrackAsync(item));
            item.IsCached = !string.IsNullOrWhiteSpace(item.URL) && MusicAudioCache.IsCached(item.URL);
            AllItems.Add(item);
        }

        ApplyFilter();
    }

    private async Task PlayTrackAsync(Audio item)
    {
        if (string.IsNullOrWhiteSpace(item.URL))
        {
            return;
        }

        ActiveTrack = item;

        if (PlayAudioHandler is not null)
        {
            await PlayAudioHandler(item.URL);
        }
    }

    private async Task PlayNextAsync()
    {
        if (ActiveTrack is null || AllItems.Count == 0)
        {
            return;
        }

        int index = AllItems.IndexOf(ActiveTrack);
        int nextIndex = index < 0 ? 0 : (index + 1) % AllItems.Count;
        await PlayTrackAsync(AllItems[nextIndex]);
    }

    private async Task TogglePlayPauseAsync()
    {
        if (TogglePlaybackHandler is not null)
        {
            await TogglePlaybackHandler();
        }
    }

    public void SetPlaybackState(bool isPlaying) => IsPlaying = isPlaying;

    public void SyncActiveTrackFlags()
    {
        foreach (Audio item in AllItems)
        {
            bool isActive = ReferenceEquals(item, ActiveTrack);
            item.IsActive = isActive;
            item.IsPlayingThis = isActive && IsPlaying;
        }
    }

    public string SearchText
    {
        get => search_text;
        set
        {
            if (SetProperty(ref search_text, value))
            {
                ApplyFilter();
                OnPropertyChanged(nameof(IsSearchEmptyVisible));
                OnPropertyChanged(nameof(HasFilteredItems));
            }
        }
    }

    private void ApplyFilter()
    {
        FilteredItems.Clear();
        string query = SearchText.Trim();

        IEnumerable<Audio> source = string.IsNullOrWhiteSpace(query)
            ? AllItems
            : AllItems.Where(item =>
                (item.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (Audio item in source)
        {
            FilteredItems.Add(item);
        }

        OnPropertyChanged(nameof(HasFilteredItems));
        OnPropertyChanged(nameof(IsSearchEmptyVisible));
    }

    private static string FormatTime(TimeSpan time)
    {
        if (time.TotalHours >= 1)
        {
            return $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}";
        }

        return $"{time.Minutes}:{time.Seconds:D2}";
    }
}
