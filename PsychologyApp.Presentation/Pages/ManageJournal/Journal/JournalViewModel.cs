using System.Collections.ObjectModel;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageJournal.Journal;

public sealed class JournalViewModel : BaseViewModel
{
    private readonly JournalMoodLoader _journalMoodLoader;
    private readonly IDialogService _dialogService;
    private int _loadGeneration;
    private long? _editorEntryId;
    private int _rangeDays = 7;
    private DateOnly? _filterDay;
    private DateOnly _editorDay = DateOnly.FromDateTime(DateTime.Today);
    private IReadOnlyList<JournalTimelineDayGroup> _allTimelineGroups = [];

    public JournalViewModel(
        JournalMoodLoader journalMoodLoader,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _journalMoodLoader = journalMoodLoader;
        _dialogService = dialogService;
        BindNavigation(navigationService);
        ModuleName = AppStrings.JournalTitle;
        PageName = AppStrings.JournalTitle;
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        RecordMoodCommand = new Command<object?>(parameter =>
        {
            int level = parameter switch
            {
                int value => value,
                string text when int.TryParse(text, out int parsed) => parsed,
                _ => 0
            };

            if (level is >= 1 and <= 5)
            {
                SelectedMoodLevel = level;
            }
        });
        SaveMoodCommand = new AsyncCommand(SaveMoodAsync);
        DeleteMoodCommand = new AsyncCommand(DeleteMoodAsync);
        ApplyPromptCommand = new Command<object?>(parameter =>
        {
            string? prompt = parameter switch
            {
                string key => ResolvePromptText(key),
                FilterChipTabItem chip => chip.Title,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(prompt))
            {
                return;
            }

            JournalNote = string.IsNullOrWhiteSpace(JournalNote)
                ? prompt
                : $"{JournalNote.TrimEnd()}\n{prompt}";
        });
        SelectRangeCommand = new Command<object?>(parameter =>
        {
            int days = parameter switch
            {
                int value => value,
                string text when int.TryParse(text, out int parsed) => parsed,
                _ => 0
            };

            if (days is not (7 or 30 or 90))
            {
                return;
            }

            if (days == _rangeDays && _filterDay is null)
            {
                return;
            }

            _rangeDays = days;
            _filterDay = null;
            SyncRangeFilters();
            LoadAsync().FireAndForget();
        });
        SelectDayCommand = new Command<object?>(parameter =>
        {
            DateOnly? day = parameter switch
            {
                DateOnly date => date,
                JournalDayChip chip => chip.Date,
                _ => null
            };

            if (day is null)
            {
                return;
            }

            if (_filterDay == day)
            {
                _filterDay = null;
                _editorDay = DateOnly.FromDateTime(DateTime.Today);
            }
            else
            {
                _filterDay = day;
                _editorDay = day.Value;
            }

            SyncRangeFilters();
            LoadAsync().FireAndForget();
        });
        SelectTimelineEntryCommand = new Command<object?>(parameter =>
        {
            if (parameter is not MoodNoteItem entry)
            {
                return;
            }

            _editorDay = entry.Day;
            _filterDay = entry.Day;
            _editorEntryId = entry.MoodEntryId;
            SelectedMoodLevel = entry.MoodLevel;
            JournalNote = entry.HasNote ? entry.NoteText : string.Empty;
            TodayMoodDisplay = entry.IsToday
                ? AppStrings.JournalEditTodayHint
                : AppStrings.JournalDayMoodLine(entry.Day, entry.MoodLevel, 5);
            OnPropertyChanged(nameof(HasEditorEntry));
            OnPropertyChanged(nameof(CanDeleteEntry));
            OnPropertyChanged(nameof(EditorDayTitle));
            OnPropertyChanged(nameof(MoodCheckInTitle));
            LoadAsync().FireAndForget();
        });

        RangeFilters =
        [
            new FilterChipTabItem { Key = "7", Title = AppStrings.JournalFilter7Days, IsSelected = true },
            new FilterChipTabItem { Key = "30", Title = AppStrings.JournalFilter30Days, IsSelected = false },
            new FilterChipTabItem { Key = "90", Title = AppStrings.JournalFilter90Days, IsSelected = false }
        ];

        PromptChips =
        [
            new FilterChipTabItem { Key = "helped", Title = AppStrings.JournalPromptHelped },
            new FilterChipTabItem { Key = "blocked", Title = AppStrings.JournalPromptBlocked },
            new FilterChipTabItem { Key = "grateful", Title = AppStrings.JournalPromptGrateful },
            new FilterChipTabItem { Key = "next", Title = AppStrings.JournalPromptNext }
        ];

        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalTitle;
    public string EditorDayTitle => AppStrings.JournalEditorDayTitle(_editorDay);
    public string MoodCheckInTitle =>
        _editorDay == DateOnly.FromDateTime(DateTime.Today)
            ? AppStrings.ProfileMoodCheckInTitle
            : AppStrings.JournalPastDayCheckInTitle;
    public string MoodTrendTitle => AppStrings.ProfileMoodTrendTitle;
    public string MoodTrendHint => AppStrings.ProfileMoodTrendHint;
    public bool ShowMoodTrendHint => !HasMoodTrendChart;
    public string EntriesTitle => AppStrings.JournalEntriesTitle;
    public string StatsSectionTitle => AppStrings.JournalMoodStatsTitle;
    public string WeekEmptyText => AppStrings.JournalWeekEmpty;
    public string NotePlaceholder => AppStrings.JournalNotePlaceholder;
    public string NoteSaveHint => AppStrings.JournalNoteSaveHint;
    public string SaveLabel => AppStrings.JournalSaveLabel;
    public string DeleteLabel => AppStrings.JournalDeleteLabel;
    public string SearchPlaceholder => AppStrings.JournalSearchPlaceholder;
    public string WeekMoodCheckInsLabel => AppStrings.WeekMoodCheckInsLabel;
    public string WeekAvgMoodLabel => AppStrings.WeekAvgMoodLabel;
    public string WeekMoodStreakLabel => AppStrings.JournalMoodStreakLabel;

    public ICommand BackCommand { get; }
    public ICommand RecordMoodCommand { get; }
    public ICommand SaveMoodCommand { get; }
    public ICommand DeleteMoodCommand { get; }
    public ICommand ApplyPromptCommand { get; }
    public ICommand SelectRangeCommand { get; }
    public ICommand SelectDayCommand { get; }
    public ICommand SelectTimelineEntryCommand { get; }

    public ObservableCollection<FilterChipTabItem> RangeFilters { get; }
    public ObservableCollection<FilterChipTabItem> PromptChips { get; }

    private IReadOnlyList<MoodChartPoint> _moodChartPoints = [];
    public IReadOnlyList<MoodChartPoint> MoodChartPoints
    {
        get => _moodChartPoints;
        private set => SetProperty(ref _moodChartPoints, value);
    }

    private string _moodChartSubtitle = string.Empty;
    public string MoodChartSubtitle
    {
        get => _moodChartSubtitle;
        private set => SetProperty(ref _moodChartSubtitle, value);
    }

    private bool _hasMoodTrendChart;
    public bool HasMoodTrendChart
    {
        get => _hasMoodTrendChart;
        private set
        {
            if (SetProperty(ref _hasMoodTrendChart, value))
            {
                OnPropertyChanged(nameof(ShowMoodTrendHint));
            }
        }
    }

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
    public string MoodNotesEmpty =>
        string.IsNullOrWhiteSpace(SearchQuery)
            ? AppStrings.JournalTimelineEmpty
            : AppStrings.JournalSearchEmpty;

    private IReadOnlyList<JournalDayChip> _weekDays = [];
    public IReadOnlyList<JournalDayChip> WeekDays
    {
        get => _weekDays;
        private set => SetProperty(ref _weekDays, value);
    }

    private int _selectedMoodLevel;
    public int SelectedMoodLevel
    {
        get => _selectedMoodLevel;
        private set => SetProperty(ref _selectedMoodLevel, value);
    }

    private string _todayMoodDisplay = string.Empty;
    public string TodayMoodDisplay
    {
        get => _todayMoodDisplay;
        private set
        {
            if (SetProperty(ref _todayMoodDisplay, value))
            {
                OnPropertyChanged(nameof(HasTodayMood));
            }
        }
    }

    public bool HasTodayMood => !string.IsNullOrWhiteSpace(TodayMoodDisplay);

    private string _moodHistorySummary = string.Empty;
    public string MoodHistorySummary
    {
        get => _moodHistorySummary;
        private set
        {
            if (SetProperty(ref _moodHistorySummary, value))
            {
                OnPropertyChanged(nameof(HasMoodHistorySummary));
            }
        }
    }

    public bool HasMoodHistorySummary => !string.IsNullOrWhiteSpace(MoodHistorySummary);

    private bool _hasMoodStats;
    public bool HasMoodStats
    {
        get => _hasMoodStats;
        private set
        {
            if (SetProperty(ref _hasMoodStats, value))
            {
                OnPropertyChanged(nameof(ShowStatsEmpty));
            }
        }
    }

    public bool ShowStatsEmpty => !HasMoodStats;

    private string _weekRangeSubtitle = string.Empty;
    public string WeekRangeSubtitle
    {
        get => _weekRangeSubtitle;
        private set => SetProperty(ref _weekRangeSubtitle, value);
    }

    private string _checkInCountDisplay = AppStrings.MetricEmptyValue;
    public string CheckInCountDisplay
    {
        get => _checkInCountDisplay;
        private set => SetProperty(ref _checkInCountDisplay, value);
    }

    private string _averageMoodDisplay = AppStrings.MetricEmptyValue;
    public string AverageMoodDisplay
    {
        get => _averageMoodDisplay;
        private set => SetProperty(ref _averageMoodDisplay, value);
    }

    private string _moodStreakDisplay = AppStrings.MetricEmptyValue;
    public string MoodStreakDisplay
    {
        get => _moodStreakDisplay;
        private set => SetProperty(ref _moodStreakDisplay, value);
    }

    private string _moodTrendLabel = string.Empty;
    public string MoodTrendLabel
    {
        get => _moodTrendLabel;
        private set
        {
            if (SetProperty(ref _moodTrendLabel, value))
            {
                OnPropertyChanged(nameof(HasMoodTrendPill));
            }
        }
    }

    public bool HasMoodTrendPill => !string.IsNullOrWhiteSpace(MoodTrendLabel);

    private string _bestWorstLabel = string.Empty;
    public string BestWorstLabel
    {
        get => _bestWorstLabel;
        private set
        {
            if (SetProperty(ref _bestWorstLabel, value))
            {
                OnPropertyChanged(nameof(HasBestWorstPill));
            }
        }
    }

    public bool HasBestWorstPill => !string.IsNullOrWhiteSpace(BestWorstLabel);

    private string _journalNote = string.Empty;
    public string JournalNote
    {
        get => _journalNote;
        set => SetProperty(ref _journalNote, value);
    }

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

    public bool HasEditorEntry => _editorEntryId is > 0;
    public bool CanDeleteEntry => HasEditorEntry;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(EditorDayTitle),
            nameof(MoodCheckInTitle),
            nameof(MoodTrendTitle),
            nameof(MoodTrendHint),
            nameof(ShowMoodTrendHint),
            nameof(EntriesTitle),
            nameof(MoodNotesEmpty),
            nameof(StatsSectionTitle),
            nameof(WeekEmptyText),
            nameof(NotePlaceholder),
            nameof(NoteSaveHint),
            nameof(SaveLabel),
            nameof(DeleteLabel),
            nameof(SearchPlaceholder),
            nameof(WeekMoodCheckInsLabel),
            nameof(WeekAvgMoodLabel),
            nameof(WeekMoodStreakLabel),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(MoodHistorySummary),
            nameof(HasMoodHistorySummary),
            nameof(HasMoodStats),
            nameof(ShowStatsEmpty),
            nameof(WeekRangeSubtitle),
            nameof(CheckInCountDisplay),
            nameof(AverageMoodDisplay),
            nameof(MoodStreakDisplay),
            nameof(MoodTrendLabel),
            nameof(HasMoodTrendPill),
            nameof(BestWorstLabel),
            nameof(HasBestWorstPill),
            nameof(HasEditorEntry),
            nameof(CanDeleteEntry));

        foreach (FilterChipTabItem filter in RangeFilters)
        {
            filter.Title = filter.Key switch
            {
                "90" => AppStrings.JournalFilter90Days,
                "30" => AppStrings.JournalFilter30Days,
                _ => AppStrings.JournalFilter7Days
            };
        }

        foreach (FilterChipTabItem prompt in PromptChips)
        {
            prompt.Title = prompt.Key switch
            {
                "helped" => AppStrings.JournalPromptHelped,
                "blocked" => AppStrings.JournalPromptBlocked,
                "grateful" => AppStrings.JournalPromptGrateful,
                _ => AppStrings.JournalPromptNext
            };
        }
    }

    public Task ReloadAsync() => LoadAsync();

    private static string? ResolvePromptText(string key) => key switch
    {
        "helped" => AppStrings.JournalPromptHelped,
        "blocked" => AppStrings.JournalPromptBlocked,
        "grateful" => AppStrings.JournalPromptGrateful,
        "next" => AppStrings.JournalPromptNext,
        _ => key
    };

    private void SyncRangeFilters()
    {
        foreach (FilterChipTabItem filter in RangeFilters)
        {
            filter.IsSelected = filter.Key == _rangeDays.ToString();
        }
    }

    private void ApplySearchFilter()
    {
        TimelineGroups = JournalMoodLoader.FilterGroupsByNoteSearch(_allTimelineGroups, SearchQuery);
    }

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            JournalMoodSnapshot snapshot = await _journalMoodLoader.LoadAsync(
                _rangeDays,
                _filterDay,
                _editorDay);
            if (generation != Volatile.Read(ref _loadGeneration))
            {
                return;
            }

            await UiThread.RunAsync(() => ApplySnapshot(snapshot));
        }
        catch
        {
            // Journal content is optional; empty state still works.
        }
    }

    private void ApplySnapshot(JournalMoodSnapshot snapshot)
    {
        MoodChartPoints = snapshot.ChartPoints;
        MoodChartSubtitle = snapshot.ChartSubtitle;
        HasMoodTrendChart = snapshot.HasTrendChart;
        _allTimelineGroups = snapshot.TimelineGroups;
        ApplySearchFilter();
        WeekDays = snapshot.WeekDays;
        _editorEntryId = snapshot.EditorEntryId;
        _editorDay = snapshot.EditorDay;
        SelectedMoodLevel = snapshot.SelectedMoodLevel;
        TodayMoodDisplay = snapshot.EditorMoodDisplay;
        MoodHistorySummary = snapshot.MoodHistorySummary;
        JournalNote = snapshot.EditorNote ?? string.Empty;
        WeekRangeSubtitle = snapshot.RangeSubtitle;

        JournalMoodStats stats = snapshot.Stats;
        HasMoodStats = stats.HasStats;
        CheckInCountDisplay = stats.HasStats
            ? stats.CheckInCount.ToString()
            : AppStrings.MetricEmptyValue;
        AverageMoodDisplay = stats.AverageMoodDisplay;
        MoodStreakDisplay = stats.MoodStreakDisplay;
        MoodTrendLabel = stats.MoodTrendLabel;
        BestWorstLabel = stats.BestWorstLabel;

        OnPropertyChanged(nameof(HasEditorEntry));
        OnPropertyChanged(nameof(CanDeleteEntry));
        OnPropertyChanged(nameof(EditorDayTitle));
        OnPropertyChanged(nameof(MoodCheckInTitle));
    }

    private async Task SaveMoodAsync()
    {
        if (SelectedMoodLevel is < 1 or > 5)
        {
            await _dialogService.ShowAsync(AppStrings.JournalTitle, AppStrings.JournalNeedMoodToSave);
            return;
        }

        string? note = string.IsNullOrWhiteSpace(JournalNote) ? null : JournalNote.Trim();
        await _journalMoodLoader.SaveMoodAsync(SelectedMoodLevel, note, _editorEntryId, _editorDay);
        TodayMoodDisplay = AppStrings.TodayMoodSaved;
        await LoadAsync();
    }

    private async Task DeleteMoodAsync()
    {
        if (_editorEntryId is not > 0)
        {
            return;
        }

        bool confirmed = await _dialogService.AskAsync(
            AppStrings.JournalDeleteConfirmTitle,
            AppStrings.JournalDeleteConfirmMessage,
            AppStrings.JournalDeleteConfirmAccept,
            AppStrings.JournalDeleteConfirmCancel);
        if (!confirmed)
        {
            return;
        }

        await _journalMoodLoader.DeleteMoodAsync(_editorEntryId.Value);
        _editorEntryId = null;
        SelectedMoodLevel = 0;
        JournalNote = string.Empty;
        TodayMoodDisplay = string.Empty;
        await LoadAsync();
    }
}
