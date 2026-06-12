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
    private string _selectedCategoryKey = string.Empty;
    private bool _isPlaying;
    private bool _isBuffering;
    private string _positionDisplay = "0:00";
    private string _durationDisplay = "0:00";
    private double _progressFraction;

    public ObservableCollection<Audio> AllItems { get; } = [];
    public ObservableCollection<Audio> FilteredItems { get; } = [];
    public ObservableCollection<PrayerCategoryFilter> CategoryFilters { get; } = [];

    public Func<string, Task>? PlayAudioHandler { get; set; }
    public Func<Task>? TogglePlaybackHandler { get; set; }
    public Func<double, Task>? SeekHandler { get; set; }
    public Func<Task>? PrefetchHandler { get; set; }
    public ICommand TogglePlayPauseCommand { get; }
    public ICommand PlayNextCommand { get; }
    public ICommand PlayPreviousCommand { get; }
    public ICommand SelectCategoryCommand { get; }

    public string PageTitle => AppStrings.CleanerPrayersPage;
    public string SearchPlaceholder => AppStrings.CleanerSearchPlaceholder;
    public string NoPrayersFoundText => AppStrings.CleanerNoPrayersFound;
    public string NowPlayingLabel => AppStrings.CleanerNowPlaying;
    public string BufferingText => AppStrings.CleanerPreparingAudio;
    public string OfflineBadgeText => AppStrings.CleanerOfflineBadge;
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
            OnPropertyChanged(nameof(ActiveTrackCategory));
            SyncActiveTrackFlags();
        }
    }

    public bool HasActiveTrack => ActiveTrack is not null;
    public string ActiveTrackTitle => ActiveTrack?.Name ?? string.Empty;
    public string ActiveTrackCategory => ActiveTrack?.Category ?? string.Empty;
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

    public double ProgressFraction
    {
        get => _progressFraction;
        set => SetProperty(ref _progressFraction, value);
    }

    public bool HasFilteredItems => FilteredItems.Count > 0;
    public bool IsSearchEmptyVisible => IsDone && !HasFilteredItems && AllItems.Count > 0;

    public MusicPlayerViewModel(ILogger<MusicPlayerViewModel> logger)
    {
        _logger = logger;
        ModuleName = AppStrings.ShellTabCleanerShort;
        PageName = AppStrings.CleanerPrayersPage;

        TogglePlayPauseCommand = new AsyncCommand(TogglePlayPauseAsync);
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

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SearchPlaceholder),
            nameof(NoPrayersFoundText),
            nameof(NowPlayingLabel),
            nameof(BufferingText),
            nameof(OfflineBadgeText),
            nameof(ProgressDisplay),
            nameof(ActiveTrackCategory));

        if (IsDone)
        {
            string? activeUrl = ActiveTrack?.URL;
            string categoryKey = _selectedCategoryKey;
            InitializePlaylist();
            _selectedCategoryKey = categoryKey;
            ApplyFilter();
            if (!string.IsNullOrWhiteSpace(activeUrl))
            {
                ActiveTrack = AllItems.FirstOrDefault(item => item.URL == activeUrl);
            }
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
        ProgressFraction = duration.TotalSeconds > 0
            ? Math.Clamp(position.TotalSeconds / duration.TotalSeconds, 0, 1)
            : 0;
        OnPropertyChanged(nameof(ProgressDisplay));
    }

    public async Task SeekToFractionAsync(double fraction)
    {
        if (SeekHandler is not null)
        {
            await SeekHandler(fraction);
        }
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

        BuildCategoryFilters();
        ApplyFilter();
    }

    private void BuildCategoryFilters()
    {
        CategoryFilters.Clear();

        CategoryFilters.Add(new PrayerCategoryFilter
        {
            Key = string.Empty,
            Title = AppStrings.CleanerCategoryAll,
            IsSelected = string.IsNullOrEmpty(_selectedCategoryKey)
        });

        foreach (string category in AllItems
                     .Select(item => item.Category)
                     .Where(category => !string.IsNullOrWhiteSpace(category))
                     .Distinct(StringComparer.Ordinal))
        {
            CategoryFilters.Add(new PrayerCategoryFilter
            {
                Key = category!,
                Title = category!,
                IsSelected = category == _selectedCategoryKey
            });
        }
    }

    private void SelectCategory(string? key)
    {
        _selectedCategoryKey = key ?? string.Empty;
        foreach (PrayerCategoryFilter filter in CategoryFilters)
        {
            filter.IsSelected = filter.Key == _selectedCategoryKey;
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
        Audio? next = ResolveAdjacentTrack(step: 1);
        if (next is not null)
        {
            await PlayTrackAsync(next);
        }
    }

    private async Task PlayPreviousAsync()
    {
        Audio? previous = ResolveAdjacentTrack(step: -1);
        if (previous is not null)
        {
            await PlayTrackAsync(previous);
        }
    }

    private Audio? ResolveAdjacentTrack(int step)
    {
        if (ActiveTrack is null || FilteredItems.Count == 0)
        {
            return null;
        }

        int index = FilteredItems.IndexOf(ActiveTrack);
        if (index < 0)
        {
            index = AllItems.IndexOf(ActiveTrack);
            if (index < 0)
            {
                return FilteredItems[0];
            }

            return AllItems[Math.Clamp(index + step, 0, AllItems.Count - 1)];
        }

        int nextIndex = (index + step + FilteredItems.Count) % FilteredItems.Count;
        return FilteredItems[nextIndex];
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
            }
        }
    }

    private void ApplyFilter()
    {
        string query = SearchText.Trim();

        IEnumerable<Audio> source = AllItems;

        if (!string.IsNullOrWhiteSpace(_selectedCategoryKey))
        {
            source = source.Where(item => item.Category == _selectedCategoryKey);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            source = source.Where(item =>
                (item.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Category?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        FilteredItems.Clear();
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
