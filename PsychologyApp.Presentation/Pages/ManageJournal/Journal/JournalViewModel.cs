using System.Collections.ObjectModel;
using System.Globalization;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Lib.Navigation;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageJournal.Journal;

public sealed class JournalViewModel : BaseViewModel
{
    private const int RecentNoteCount = 3;

    private readonly JournalMoodLoader _journalMoodLoader;
    private readonly JournalEditorContext _editorContext;
    private readonly IDialogService _dialogService;
    private readonly IShellTabNavigator _shellTabNavigator;
    private readonly INavigationService _navigationService;
    private int _loadGeneration;
    private long? _editorEntryId;
    private DateOnly _editorDay = DateOnly.FromDateTime(DateTime.Today);
    private DateOnly _weekStripEnd = DateOnly.FromDateTime(DateTime.Today);
    private JournalCheckInSlot _editorSlot =
        MoodCheckInSlotPolicy.IsMorningLocalHour(DateTime.Now.Hour)
            ? JournalCheckInSlot.Morning
            : JournalCheckInSlot.Evening;
    private CancellationTokenSource? _noteSaveCts;
    private bool _suppressNoteAutosave;
    private bool _noteDirty;
    private string? _crisisOfferedForNote;

    public JournalViewModel(
        JournalMoodLoader journalMoodLoader,
        JournalEditorContext editorContext,
        IDialogService dialogService,
        IShellTabNavigator shellTabNavigator,
        INavigationService navigationService)
    {
        _journalMoodLoader = journalMoodLoader;
        _editorContext = editorContext;
        _dialogService = dialogService;
        _shellTabNavigator = shellTabNavigator;
        _navigationService = navigationService;
        BindNavigation(navigationService);
        ModuleName = AppStrings.JournalTitle;
        PageName = AppStrings.JournalTitle;
        OpenOverviewCommand = new AsyncCommand(async () =>
        {
            await FlushPendingNoteSaveAsync();
            await navigationService.GoToJournalOverviewAsync();
        });
        OpenTimelineCommand = new AsyncCommand(async () =>
        {
            await FlushPendingNoteSaveAsync();
            await navigationService.GoToJournalTimelineAsync();
        });
        PickSlotCommand = new AsyncCommand(PickSlotAsync);
        FlushNoteCommand = new AsyncCommand(FlushPendingNoteSaveAsync);
        OpenPracticeSuggestCommand = new Command(() => _shellTabNavigator.OpenPracticeTab());
        RecordMoodCommand = new AsyncCommand<object?>(async parameter =>
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
                await SaveMoodAsync();
            }
        });
        ToggleFactorCommand = new Command<object?>(parameter =>
        {
            string? key = parameter switch
            {
                string text => text,
                JournalActivityChip chip => chip.Key,
                _ => null
            };
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            JournalNote = JournalNoteFactors.ToggleFactor(JournalNote, key);
            SyncActivityChips();
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

            SelectDayAsync(day.Value).FireAndForget();
        });

        ActivityChips = new ObservableCollection<JournalActivityChip>(
            JournalNoteFactors.PrimaryKeys.Select(key => new JournalActivityChip
            {
                Key = key,
                Label = JournalNoteFactors.GetLabel(key)
            }));

        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalTitle;
    public string MoodQuestionHero => AppStrings.JournalHowNowQuestion;
    /// <summary>Neutral, because the question of the day stands above the note; a second question in the field competed with it.</summary>
    public string NotePlaceholder => AppStrings.JournalPromptPlaceholder;
    public string PracticeSuggestText => AppStrings.JournalTryShortPractice;
    public string OpenOverviewLabel => AppStrings.JournalOpenOverview;
    public string OpenTimelineLabel => AppStrings.JournalOpenTimeline;
    public string SlotMorningLabel => AppStrings.JournalSlotMorning;
    public string WeekCardTitle => AppStrings.JournalWeekCardTitle;
    public string WeekCardEmpty => AppStrings.JournalWeekCardEmpty;
    public string WeekOpenOverviewText => AppStrings.JournalWeekOpenOverview;
    public string CheckInsLabel => AppStrings.WeekMoodCheckInsLabel;
    public string AverageLabel => AppStrings.WeekAvgMoodLabel;
    public string StreakLabel => AppStrings.JournalStreakMetricLabel;
    public string PromptTitle => AppStrings.JournalPromptTitle;
    public string RecentTitle => AppStrings.JournalRecentTitle;
    public string RecentAllText => AppStrings.JournalRecentAll;

    /// <summary>The question of the day for the day being written about, so a past day keeps the question it had.</summary>
    public string PromptText => AppStrings.JournalPrompt(JournalPrompts.For(_editorDay));

    private JournalMoodStats? _weekStats;
    public bool HasWeekStats => _weekStats?.HasStats == true;
    public string WeekCheckIns => _weekStats?.CheckInCount.ToString(CultureInfo.CurrentCulture) ?? string.Empty;
    public string WeekAverage => _weekStats?.AverageMoodDisplay ?? string.Empty;
    public string WeekStreak => _weekStats?.MoodStreakDisplay ?? string.Empty;

    private string _weekInsight = string.Empty;
    /// <summary>What the week shows about practices and mood, when there is enough to say something.</summary>
    public string WeekInsight
    {
        get => _weekInsight;
        private set
        {
            if (SetProperty(ref _weekInsight, value))
            {
                OnPropertyChanged(nameof(HasWeekInsight));
            }
        }
    }

    public bool HasWeekInsight => WeekInsight.Length > 0;

    private IReadOnlyList<MoodNoteItem> _recentNotes = [];
    /// <summary>The last few notes of the week, newest first: what was written comes back without opening the history.</summary>
    public IReadOnlyList<MoodNoteItem> RecentNotes
    {
        get => _recentNotes;
        private set
        {
            if (SetProperty(ref _recentNotes, value))
            {
                OnPropertyChanged(nameof(HasRecentNotes));
            }
        }
    }

    public bool HasRecentNotes => RecentNotes.Count > 0;

    private string _onThisDay = string.Empty;
    public string OnThisDayText
    {
        get => _onThisDay;
        private set
        {
            if (SetProperty(ref _onThisDay, value))
            {
                OnPropertyChanged(nameof(HasOnThisDay));
            }
        }
    }

    public bool HasOnThisDay => OnThisDayText.Length > 0;
    public string SlotEveningLabel => AppStrings.JournalSlotEvening;

    public string DayHeaderText =>
        AppStrings.JournalEditorTitle(AppStrings.JournalEditorDayTitle(_editorDay), _editorSlot == JournalCheckInSlot.Evening);

    public string MoodSavedStatus =>
        HasMoodSelected ? AppStrings.TodayMoodSaved : string.Empty;

    public bool CanGoNextWeek =>
        _weekStripEnd < DateOnly.FromDateTime(DateTime.Today);

    public ObservableCollection<JournalActivityChip> ActivityChips { get; }

    public ICommand OpenOverviewCommand { get; }
    public ICommand OpenTimelineCommand { get; }
    public ICommand PickSlotCommand { get; }
    public ICommand FlushNoteCommand { get; }
    public ICommand OpenPracticeSuggestCommand { get; }
    public ICommand RecordMoodCommand { get; }
    public ICommand ToggleFactorCommand { get; }
    public ICommand SelectDayCommand { get; }

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
        private set
        {
            if (SetProperty(ref _selectedMoodLevel, value))
            {
                OnPropertyChanged(nameof(ShowPracticeSuggest));
                OnPropertyChanged(nameof(HasMoodSelected));
                OnPropertyChanged(nameof(ShowMoodQuestion));
                OnPropertyChanged(nameof(ShowNoteEditor));
                OnPropertyChanged(nameof(MoodSavedStatus));
            }
        }
    }

    public bool HasMoodSelected => SelectedMoodLevel is >= 1 and <= 5;
    public bool ShowMoodQuestion => !HasMoodSelected;
    public bool ShowNoteEditor => HasMoodSelected;
    public bool ShowPracticeSuggest =>
        _editorDay == DateOnly.FromDateTime(DateTime.Today)
        && SelectedMoodLevel is 1 or 2;

    private string _journalNote = string.Empty;
    public string JournalNote
    {
        get => _journalNote;
        set
        {
            if (!SetProperty(ref _journalNote, value))
            {
                return;
            }

            SyncActivityChips();
            if (!_suppressNoteAutosave)
            {
                _noteDirty = true;
                ScheduleNoteAutosave();
            }
        }
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MoodQuestionHero),
            nameof(NotePlaceholder),
            nameof(PracticeSuggestText),
            nameof(OpenOverviewLabel),
            nameof(OpenTimelineLabel),
            nameof(SlotMorningLabel),
            nameof(SlotEveningLabel),
            nameof(DayHeaderText),
            nameof(MoodSavedStatus),
            nameof(ShowPracticeSuggest),
            nameof(HasMoodSelected),
            nameof(ShowMoodQuestion),
            nameof(ShowNoteEditor),
            nameof(CanGoNextWeek),
            nameof(WeekCardTitle),
            nameof(WeekCardEmpty),
            nameof(WeekOpenOverviewText),
            nameof(CheckInsLabel),
            nameof(AverageLabel),
            nameof(StreakLabel),
            nameof(PromptTitle),
            nameof(PromptText),
            nameof(RecentTitle),
            nameof(RecentAllText));
        foreach (JournalActivityChip chip in ActivityChips)
        {
            chip.Label = JournalNoteFactors.GetLabel(chip.Key);
        }
    }

    public Task ReloadAsync()
    {
        if (_editorContext.ConsumePendingEditorDay() is DateOnly pendingDay)
        {
            _editorDay = pendingDay;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            _weekStripEnd = JournalMoodLoader.ClampStripEnd(pendingDay, today);
        }

        return LoadAsync();
    }

    public async Task FlushPendingNoteSaveAsync()
    {
        CancelNoteAutosave();
        if (!_noteDirty || !HasMoodSelected)
        {
            return;
        }

        await SaveMoodAsync(reload: false);
    }

    private async Task PickSlotAsync()
    {
        string? picked = await _dialogService.PickOptionAsync(
            AppStrings.JournalSlotPickerTitle,
            [AppStrings.JournalSlotMorning, AppStrings.JournalSlotEvening],
            AppStrings.Cancel);
        if (picked == AppStrings.JournalSlotMorning)
        {
            await SelectSlotAsync(JournalCheckInSlot.Morning);
        }
        else if (picked == AppStrings.JournalSlotEvening)
        {
            await SelectSlotAsync(JournalCheckInSlot.Evening);
        }
    }

    private async Task SelectDayAsync(DateOnly day)
    {
        await FlushPendingNoteSaveAsync();
        _editorDay = day;
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        if (_editorDay > _weekStripEnd || _editorDay < _weekStripEnd.AddDays(-6))
        {
            _weekStripEnd = JournalMoodLoader.ClampStripEnd(_editorDay, today);
        }

        await LoadAsync();
    }

    private async Task SelectSlotAsync(JournalCheckInSlot slot)
    {
        if (_editorSlot == slot)
        {
            return;
        }

        await FlushPendingNoteSaveAsync();
        _editorSlot = slot;
        OnPropertyChanged(nameof(DayHeaderText));
        await LoadAsync();
    }

    private void SyncActivityChips()
    {
        foreach (JournalActivityChip chip in ActivityChips)
        {
            chip.IsActive = JournalNoteFactors.HasFactor(JournalNote, chip.Key);
        }
    }

    private void CancelNoteAutosave()
    {
        CancellationTokenSource? previous = Interlocked.Exchange(ref _noteSaveCts, null);
        previous?.Cancel();
        previous?.Dispose();
    }

    private void ScheduleNoteAutosave()
    {
        if (!HasMoodSelected)
        {
            return;
        }

        CancellationTokenSource next = new();
        CancellationTokenSource? previous = Interlocked.Exchange(ref _noteSaveCts, next);
        previous?.Cancel();
        previous?.Dispose();
        SaveNoteDebouncedAsync(next.Token).FireAndForget();
    }

    private async Task SaveNoteDebouncedAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(800, token);
            await SaveMoodAsync();
        }
        catch (OperationCanceledException)
        {
            // Newer edit or explicit flush superseded this save.
        }
    }

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            JournalMoodSnapshot snapshot = await _journalMoodLoader.LoadAsync(
                rangeDays: 7,
                filterDay: null,
                editorDay: _editorDay,
                weekStripEnd: _weekStripEnd,
                editorSlot: _editorSlot);
            if (generation != Volatile.Read(ref _loadGeneration))
            {
                return;
            }

            await UiThread.RunAsync(() => ApplySnapshot(snapshot));
        }
        catch (Exception bestEffortError)
        {
            // Journal content is optional; empty state still works.
            BestEffort.Report(bestEffortError);
        }
    }

    /// <summary>
    /// The journal reloads every time its tab appears. When the week shows the same days with the same moods, only the selection
    /// moves: replacing the list would rebuild all seven day cells for nothing.
    /// </summary>
    private void ApplyWeekDays(IReadOnlyList<JournalDayChip> days)
    {
        bool sameDays = days.Count == _weekDays.Count
            && days.Zip(_weekDays).All(pair => pair.First.Date == pair.Second.Date
                && pair.First.MoodLevel == pair.Second.MoodLevel
                && pair.First.MoodGlyph == pair.Second.MoodGlyph
                && pair.First.HasEntry == pair.Second.HasEntry
                && pair.First.DayLabel == pair.Second.DayLabel);
        if (!sameDays)
        {
            WeekDays = days;
            return;
        }

        foreach ((JournalDayChip fresh, JournalDayChip shown) in days.Zip(_weekDays))
        {
            shown.IsSelected = fresh.IsSelected;
        }
    }

    private void ApplySnapshot(JournalMoodSnapshot snapshot)
    {
        CancelNoteAutosave();
        _noteDirty = false;
        ApplyWeekDays(snapshot.WeekDays);
        _weekStripEnd = snapshot.WeekStripEnd;
        _editorEntryId = snapshot.EditorEntryId;
        _editorDay = snapshot.EditorDay;
        _editorSlot = snapshot.EditorSlot;
        SelectedMoodLevel = snapshot.SelectedMoodLevel;
        _suppressNoteAutosave = true;
        try
        {
            JournalNote = snapshot.EditorNote ?? string.Empty;
        }
        finally
        {
            _suppressNoteAutosave = false;
        }

        _weekStats = snapshot.Stats;
        WeekInsight = snapshot.PracticeMoodInsight ?? string.Empty;
        OnThisDayText = snapshot.OnThisDayLastYearText ?? string.Empty;
        RecentNotes = [.. snapshot.TimelineEntries.Where(entry => entry.HasNote).Take(RecentNoteCount)];

        Notify(nameof(HasWeekStats), nameof(WeekCheckIns), nameof(WeekAverage), nameof(WeekStreak), nameof(PromptText));
        OnPropertyChanged(nameof(DayHeaderText));
        OnPropertyChanged(nameof(ShowPracticeSuggest));
        OnPropertyChanged(nameof(ShowMoodQuestion));
        OnPropertyChanged(nameof(ShowNoteEditor));
        OnPropertyChanged(nameof(MoodSavedStatus));
        OnPropertyChanged(nameof(CanGoNextWeek));
    }

    private async Task SaveMoodAsync(bool reload = true)
    {
        if (SelectedMoodLevel is < 1 or > 5)
        {
            return;
        }

        string? note = string.IsNullOrWhiteSpace(JournalNote) ? null : JournalNote.Trim();
        bool looksLikeCrisis = await _journalMoodLoader.SaveMoodAsync(
            SelectedMoodLevel,
            note,
            _editorEntryId,
            _editorDay,
            _editorSlot);
        _noteDirty = false;
        OnPropertyChanged(nameof(MoodSavedStatus));
        OnPropertyChanged(nameof(ShowPracticeSuggest));
        if (reload)
        {
            await LoadAsync();
        }

        // Asked once per distinct note text: autosave fires repeatedly while the person is still typing.
        if (looksLikeCrisis && !string.Equals(note, _crisisOfferedForNote, StringComparison.Ordinal))
        {
            _crisisOfferedForNote = note;
            await OfferCrisisHelpAsync();
        }
    }

    private async Task OfferCrisisHelpAsync()
    {
        bool open = await _dialogService.AskAsync(
            AppStrings.JournalCrisisPromptTitle,
            AppStrings.JournalCrisisPromptBody,
            AppStrings.JournalCrisisPromptAccept,
            AppStrings.JournalCrisisPromptDecline);
        if (open)
        {
            await _navigationService.GoToCrisisHubAsync();
        }
    }
}
