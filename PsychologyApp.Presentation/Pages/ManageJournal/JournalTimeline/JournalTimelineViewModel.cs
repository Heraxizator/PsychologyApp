using Microsoft.Maui.ApplicationModel.DataTransfer;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageJournal.JournalTimeline;

public sealed class JournalTimelineViewModel : BaseViewModel
{
    private readonly JournalMoodLoader _journalMoodLoader;
    private readonly JournalScreenCoordinator _journalScreenCoordinator;
    private readonly INavigationService _navigationService;
    private int _loadGeneration;
    private IReadOnlyList<JournalTimelineDayGroup> _allTimelineGroups = [];
    private string _moodStreakDisplay = AppStrings.MetricEmptyValue;

    public JournalTimelineViewModel(
        JournalMoodLoader journalMoodLoader,
        JournalScreenCoordinator journalScreenCoordinator,
        INavigationService navigationService)
    {
        _journalMoodLoader = journalMoodLoader;
        _journalScreenCoordinator = journalScreenCoordinator;
        _navigationService = navigationService;
        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        SelectTimelineEntryCommand = new Command<object?>(parameter => SelectTimelineEntryAsync(parameter).FireAndForget());
        ShareEntryCommand = new Command<object?>(parameter => ShareEntryAsync(parameter).FireAndForget());
        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalTimelineTitle;
    public string SearchPlaceholder => AppStrings.JournalSearchPlaceholder;
    public string ShareLabel => AppStrings.JournalShareLabel;

    public ICommand BackCommand { get; }
    public ICommand SelectTimelineEntryCommand { get; }
    public ICommand ShareEntryCommand { get; }

    private IReadOnlyList<JournalTimelineDayGroup> _timelineGroups = [];
    public IReadOnlyList<JournalTimelineDayGroup> TimelineGroups
    {
        get => _timelineGroups;
        private set
        {
            if (SetProperty(ref _timelineGroups, value))
            {
                OnPropertyChanged(nameof(HasMoodNotes));
                OnPropertyChanged(nameof(ShowMoodNotesEmpty));
                OnPropertyChanged(nameof(MoodNotesEmpty));
            }
        }
    }

    public bool HasMoodNotes => TimelineGroups.Count > 0;
    public bool ShowMoodNotesEmpty => !HasMoodNotes;

    private string _timelineStreakText = string.Empty;
    public string TimelineStreakText
    {
        get => _timelineStreakText;
        private set
        {
            if (SetProperty(ref _timelineStreakText, value))
            {
                OnPropertyChanged(nameof(HasTimelineStreak));
            }
        }
    }

    public bool HasTimelineStreak => !string.IsNullOrWhiteSpace(TimelineStreakText);

    public string MoodNotesEmpty =>
        string.IsNullOrWhiteSpace(SearchQuery)
            ? AppStrings.JournalTimelineEmpty
            : AppStrings.JournalSearchEmpty;

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                ApplySearchFilter();
            }
        }
    }

    protected override void RefreshLocalizedProperties()
    {
        TimelineStreakText = AppStrings.JournalTimelineStreakLine(_moodStreakDisplay);
        Notify(
            nameof(PageTitle),
            nameof(SearchPlaceholder),
            nameof(ShareLabel),
            nameof(MoodNotesEmpty),
            nameof(HasMoodNotes),
            nameof(ShowMoodNotesEmpty),
            nameof(TimelineStreakText),
            nameof(HasTimelineStreak));
    }

    public Task ReloadAsync() => LoadAsync();

    private void ApplySearchFilter()
    {
        TimelineGroups = JournalMoodLoader.FilterGroupsByNoteSearch(_allTimelineGroups, SearchQuery);
    }

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            JournalMoodSnapshot snapshot = await _journalMoodLoader.LoadAsync(90);
            if (generation != Volatile.Read(ref _loadGeneration))
            {
                return;
            }

            await UiThread.RunAsync(() =>
            {
                _allTimelineGroups = snapshot.TimelineGroups;
                _moodStreakDisplay = snapshot.Stats.MoodStreakDisplay;
                TimelineStreakText = AppStrings.JournalTimelineStreakLine(_moodStreakDisplay);
                ApplySearchFilter();
            });
        }
        catch
        {
            // Timeline is optional.
        }
    }

    private async Task SelectTimelineEntryAsync(object? parameter)
    {
        if (parameter is not MoodNoteItem entry)
        {
            return;
        }

        await _journalScreenCoordinator.OpenEditorDayAsync(entry.Day, _navigationService);
    }

    private async Task ShareEntryAsync(object? parameter)
    {
        if (parameter is not MoodNoteItem entry)
        {
            return;
        }

        string note = entry.HasNote ? entry.NoteText : string.Empty;
        IReadOnlyList<string> factors = JournalNoteFactors.ExtractActiveLabels(note);
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = AppStrings.JournalShareTitle,
            Text = AppStrings.JournalShareEntryWithFactors(entry.DateText, entry.MoodLevel, note, factors)
        });
    }
}
