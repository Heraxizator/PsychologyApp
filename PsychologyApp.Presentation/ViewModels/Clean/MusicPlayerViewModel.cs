using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Clean;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Clean;

public class MusicPlayerViewModel : BaseViewModel
{
    private const string PlaylistLoadedKey = "music_playlist_loaded";

    private readonly ILogger<MusicPlayerViewModel> _logger;
    private Audio? _activeTrack;
    private string search_text = string.Empty;

    public ObservableCollection<Audio> AllItems { get; } = [];
    public ObservableCollection<Audio> FilteredItems { get; } = [];

    public Func<string, Task>? PlayAudioHandler { get; set; }
    public Func<Task>? TogglePlaybackHandler { get; set; }
    public ICommand TogglePlayPauseCommand { get; }

    public string PageTitle => AppStrings.ShellTabCleaner;
    public string PrayerCollectionLabel => AppStrings.CleanerPrayerCollection;
    public string LoadLabel => AppStrings.CleanerLoad;
    public string SearchingPrayersText => AppStrings.CleanerSearchingPrayers;
    public string SearchPlaceholder => AppStrings.CleanerSearchPlaceholder;
    public string MoreInfoHeader => AppStrings.TestsMoreInfo;
    public string MoreInfoBody => AppStrings.CleanerMoreInfoBody;
    public string LoadFailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string NoPrayersFoundText => AppStrings.CleanerNoPrayersFound;
    public string NowPlayingLabel => AppStrings.CleanerNowPlaying;

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
        }
    }

    public bool HasActiveTrack => ActiveTrack is not null;
    public string ActiveTrackTitle => ActiveTrack?.Name ?? string.Empty;

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
        }
    }

    private bool _isPlaying;

    public string PlayPauseGlyph => IsPlaying ? "⏸" : "▶";

    public bool HasFilteredItems => FilteredItems.Count > 0;
    public bool IsSearchEmptyVisible => IsDone && !string.IsNullOrWhiteSpace(SearchText) && FilteredItems.Count == 0;

    public MusicPlayerViewModel(ILogger<MusicPlayerViewModel> logger)
    {
        _logger = logger;
        ModuleName = AppStrings.ShellTabCleaner;
        PageName = AppStrings.CleanerPrayersPage;

        Start = new AsyncCommand(LoadPlaylistAsync);
        Cancel = new Command(CancelProgress);
        TogglePlayPauseCommand = new AsyncCommand(TogglePlayPauseAsync);

        if (Preferences.Get(PlaylistLoadedKey, false))
        {
            LoadPlaylistAsync().FireAndForget();
        }
        else
        {
            SetCreated();
        }
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(PrayerCollectionLabel),
            nameof(LoadLabel),
            nameof(SearchingPrayersText),
            nameof(SearchPlaceholder),
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(LoadFailedText),
            nameof(RetryText),
            nameof(NoPrayersFoundText),
            nameof(NowPlayingLabel),
            nameof(PlayPauseGlyph));

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

    private async Task LoadPlaylistAsync()
    {
        try
        {
            SetInit();
            await Task.Yield();

            ObservableCollection<Audio> collection = MusicPlaylist.CreateDefault();
            InitItems(collection);
            Preferences.Set(PlaylistLoadedKey, true);
            SetDone();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load music playlist.");
            SetFail();
        }
    }

    private void InitItems(ObservableCollection<Audio> collection)
    {
        AllItems.Clear();

        foreach (Audio item in collection)
        {
            item.ClickCommand = new AsyncCommand(() => PlayTrackAsync(item));
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

    private async Task TogglePlayPauseAsync()
    {
        if (TogglePlaybackHandler is not null)
        {
            await TogglePlaybackHandler();
        }
    }

    public void SetPlaybackState(bool isPlaying) => IsPlaying = isPlaying;

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
}
