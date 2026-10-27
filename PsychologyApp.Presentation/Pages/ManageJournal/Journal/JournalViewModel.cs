using System.Collections.ObjectModel;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Entities.Journal;
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
    private readonly JournalMoodLoader _journalMoodLoader;
    private readonly JournalEditorContext _editorContext;
    private readonly IDialogService _dialogService;
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
    private bool _noteLooksLikeCrisis;
    private bool _factorsOpened;

    public JournalViewModel(
        JournalMoodLoader journalMoodLoader,
        JournalEditorContext editorContext,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _journalMoodLoader = journalMoodLoader;
        _editorContext = editorContext;
        _dialogService = dialogService;
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
        SelectSlotCommand = new Command<object?>(parameter =>
        {
            JournalCheckInSlot? slot = parameter switch
            {
                "morning" => JournalCheckInSlot.Morning,
                "evening" => JournalCheckInSlot.Evening,
                _ => null
            };
            if (slot is not null)
            {
                SelectSlotAsync(slot.Value).FireAndForget();
            }
        });
        FlushNoteCommand = new AsyncCommand(FlushPendingNoteSaveAsync);
        ShowFactorsCommand = new Command(() =>
        {
            _factorsOpened = true;
            NotifyFactors();
        });
        OpenLowMoodBreathCommand = new AsyncCommand(async () =>
        {
            await FlushPendingNoteSaveAsync();
            await navigationService.GoToTechniqueAsync(TechniqueId.CyclicSigh);
        });
        OpenLowMoodAnchorsCommand = new AsyncCommand(async () =>
        {
            await FlushPendingNoteSaveAsync();
            await navigationService.GoToAnchorThoughtsAsync();
        });
        OpenLowMoodHelpCommand = new AsyncCommand(async () =>
        {
            await FlushPendingNoteSaveAsync();
            await navigationService.GoToCrisisHubAsync();
        });
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

        SlotFilters =
        [
            new FilterChipTabItem { Key = "morning", Title = AppStrings.JournalSlotMorning, IsSelected = _editorSlot == JournalCheckInSlot.Morning },
            new FilterChipTabItem { Key = "evening", Title = AppStrings.JournalSlotEvening, IsSelected = _editorSlot == JournalCheckInSlot.Evening }
        ];

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
    public string LowMoodTitle => AppStrings.JournalLowMoodTitle;
    public string LowMoodBreathText => AppStrings.JournalLowMoodBreath;
    public string LowMoodAnchorsText => AppStrings.JournalLowMoodAnchors;
    public string LowMoodHelpText => AppStrings.JournalLowMoodHelp;
    public string OpenOverviewLabel => AppStrings.JournalOpenOverview;
    public string OpenTimelineLabel => AppStrings.JournalOpenTimeline;
    public string SlotMorningLabel => AppStrings.JournalSlotMorning;
    public string PromptTitle => AppStrings.JournalPromptTitle;
    public string EmptyTitle => AppStrings.JournalEmptyTitle;
    public string EmptyBody => AppStrings.JournalEmptyBody;
    public string EmptyPrivacy => AppStrings.JournalEmptyPrivacy;
    public string FactorsShowText => AppStrings.JournalFactorsShow;

    /// <summary>The question of the day for the day being written about, so a past day keeps the question it had.</summary>
    public string PromptText => AppStrings.JournalPrompt(JournalPrompts.For(_editorDay));

    private JournalMoodStats? _weekStats;
    public bool HasWeekStats => _weekStats?.HasStats == true;

    /// <summary>The week's figures in one quiet line under the strip; the overview has them in full.</summary>
    public string WeekSummaryText => _weekStats is { HasStats: true } stats
        ? AppStrings.JournalWeekSummary(stats.CheckInCount, stats.AverageMoodDisplay)
        : string.Empty;

    /// <summary>
    /// Nothing this week and nothing chosen yet today: the page is a single welcome with the faces on it, not empty cards. The faces
    /// stay on this screen, so the first check-in is still one tap.
    /// </summary>
    public bool ShowEmptyState =>
        !HasWeekStats && !HasMoodSelected && _editorDay == DateOnly.FromDateTime(DateTime.Today);

    public bool ShowWeek => !ShowEmptyState;

    /// <summary>The factors fold away behind one link until they are wanted; a note that already has some shows them.</summary>
    public bool ShowFactors => _factorsOpened || ActivityChips.Any(chip => chip.IsActive);
    public bool ShowFactorsLink => !ShowFactors;

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

    /// <summary>Only the day: morning or evening is the chip below it, changed in one tap.</summary>
    public string DayHeaderText => AppStrings.JournalEditorDayTitle(_editorDay);

    public string MoodSavedStatus =>
        HasMoodSelected ? AppStrings.TodayMoodSaved : string.Empty;

    public bool CanGoNextWeek =>
        _weekStripEnd < DateOnly.FromDateTime(DateTime.Today);

    public ObservableCollection<JournalActivityChip> ActivityChips { get; }
    public ObservableCollection<FilterChipTabItem> SlotFilters { get; }

    public ICommand OpenOverviewCommand { get; }
    public ICommand OpenTimelineCommand { get; }
    public ICommand SelectSlotCommand { get; }
    public ICommand FlushNoteCommand { get; }
    public ICommand ShowFactorsCommand { get; }
    public ICommand OpenLowMoodBreathCommand { get; }
    public ICommand OpenLowMoodAnchorsCommand { get; }
    public ICommand OpenLowMoodHelpCommand { get; }
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
                NotifyLowMoodSupport();
                OnPropertyChanged(nameof(HasMoodSelected));
                OnPropertyChanged(nameof(ShowMoodQuestion));
                OnPropertyChanged(nameof(ShowNoteEditor));
                OnPropertyChanged(nameof(MoodSavedStatus));
                NotifyEmptyState();
            }
        }
    }

    public bool HasMoodSelected => SelectedMoodLevel is >= 1 and <= 5;
    public bool ShowMoodQuestion => !HasMoodSelected;
    public bool ShowNoteEditor => HasMoodSelected;
    private JournalLowMoodSupportKind LowMoodSupport =>
        JournalLowMoodSupport.For(_editorDay == DateOnly.FromDateTime(DateTime.Today), SelectedMoodLevel, _noteLooksLikeCrisis);

    public bool ShowLowMoodSupport => LowMoodSupport != JournalLowMoodSupportKind.None;
    public bool ShowLowMoodPractices => LowMoodSupport == JournalLowMoodSupportKind.PracticesAndHelp;

    /// <summary>In a crisis the card suggests nothing: it says what the crisis prompt says and leads only to help.</summary>
    public string LowMoodBody =>
        LowMoodSupport == JournalLowMoodSupportKind.HelpOnly ? AppStrings.JournalCrisisPromptBody : AppStrings.JournalLowMoodBody;

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
            nameof(LowMoodTitle),
            nameof(LowMoodBody),
            nameof(LowMoodBreathText),
            nameof(LowMoodAnchorsText),
            nameof(LowMoodHelpText),
            nameof(OpenOverviewLabel),
            nameof(OpenTimelineLabel),
            nameof(SlotMorningLabel),
            nameof(SlotEveningLabel),
            nameof(DayHeaderText),
            nameof(MoodSavedStatus),
            nameof(HasMoodSelected),
            nameof(ShowMoodQuestion),
            nameof(ShowNoteEditor),
            nameof(CanGoNextWeek),
            nameof(PromptTitle),
            nameof(PromptText),
            nameof(EmptyTitle),
            nameof(EmptyBody),
            nameof(EmptyPrivacy),
            nameof(FactorsShowText),
            nameof(WeekSummaryText));
        foreach (JournalActivityChip chip in ActivityChips)
        {
            chip.Label = JournalNoteFactors.GetLabel(chip.Key);
        }

        foreach (FilterChipTabItem filter in SlotFilters)
        {
            filter.Title = filter.Key == "morning" ? AppStrings.JournalSlotMorning : AppStrings.JournalSlotEvening;
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

    private async Task SelectDayAsync(DateOnly day)
    {
        await FlushPendingNoteSaveAsync();
        _editorDay = day;
        _factorsOpened = false;
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
        _factorsOpened = false;
        SyncSlotFilters();
        await LoadAsync();
    }

    private void SyncSlotFilters()
    {
        string key = _editorSlot == JournalCheckInSlot.Morning ? "morning" : "evening";
        foreach (FilterChipTabItem filter in SlotFilters)
        {
            filter.IsSelected = filter.Key == key;
        }
    }

    private void NotifyEmptyState() => Notify(nameof(ShowEmptyState), nameof(ShowWeek));

    private void NotifyFactors() => Notify(nameof(ShowFactors), nameof(ShowFactorsLink));

    private void NotifyLowMoodSupport() =>
        Notify(nameof(ShowLowMoodSupport), nameof(ShowLowMoodPractices), nameof(LowMoodBody));

    private void SyncActivityChips()
    {
        foreach (JournalActivityChip chip in ActivityChips)
        {
            chip.IsActive = JournalNoteFactors.HasFactor(JournalNote, chip.Key);
        }

        NotifyFactors();
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
        SyncSlotFilters();
        _noteLooksLikeCrisis = snapshot.EditorNoteLooksLikeCrisis;
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
        OnThisDayText = snapshot.OnThisDayLastYearText ?? string.Empty;

        Notify(nameof(HasWeekStats), nameof(WeekSummaryText), nameof(PromptText));
        NotifyEmptyState();
        OnPropertyChanged(nameof(DayHeaderText));
        NotifyLowMoodSupport();
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
        _noteLooksLikeCrisis = looksLikeCrisis;
        OnPropertyChanged(nameof(MoodSavedStatus));
        NotifyLowMoodSupport();
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
