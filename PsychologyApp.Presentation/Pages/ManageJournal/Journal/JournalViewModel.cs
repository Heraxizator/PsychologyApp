using System.Collections.ObjectModel;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Core.Charts;
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
    private DateOnly _monthCursor = new(DateTime.Today.Year, DateTime.Today.Month, 1);
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

    /// <summary>
    /// The day or slot changed and its check-in is still loading: the entry id, mood and note on screen belong to the one before, so a
    /// face tapped or a note saved now would overwrite that other check-in. Nothing is saved until the snapshot of the new one arrives.
    /// </summary>
    private bool _editorStale;

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
        PrevMonthCommand = new Command(() => ShiftMonth(-1));
        NextMonthCommand = new Command(() => ShiftMonth(1));
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

            if (level is >= 1 and <= 5 && !_editorStale)
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
                JournalMonthCell { IsEnabled: true, Date: DateOnly cellDay } => cellDay,
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
    /// <summary>The question of the day as the field's hint, so the note card has no block of its own above the field.</summary>
    public string NotePlaceholder => ShowPrompt ? PromptText : AppStrings.JournalPastDayPlaceholder;
    public string LowMoodTitle => AppStrings.JournalLowMoodTitle;
    public string LowMoodBreathText => AppStrings.JournalLowMoodBreath;
    public string LowMoodAnchorsText => AppStrings.JournalLowMoodAnchors;
    public string LowMoodHelpText => AppStrings.JournalLowMoodHelp;
    public string OpenOverviewLabel => AppStrings.JournalOpenOverview;
    public string OpenTimelineLabel => AppStrings.JournalOpenTimeline;
    public string MonthNavPrevLabel => AppStrings.JournalMonthNavPrev;
    public string MonthNavNextLabel => AppStrings.JournalMonthNavNext;
    public string EarlierWeeksText => AppStrings.JournalEarlierWeeks;
    public string SlotMorningLabel => AppStrings.JournalSlotMorning;
    public string EmptyTitle => AppStrings.JournalEmptyTitle;
    public string EmptyBody => AppStrings.JournalEmptyBody;
    public string EmptyPrivacy => AppStrings.JournalEmptyPrivacy;
    public string FactorsShowText => AppStrings.JournalFactorsShow;

    public string PromptText => AppStrings.JournalPrompt(JournalPrompts.For(_editorDay));

    /// <summary>
    /// Only today: most questions ask about today or now ("What was easier today?"), which reads wrong over Monday's note. A past
    /// day gets a plain note.
    /// </summary>
    public bool ShowPrompt => IsToday;

    private bool IsToday => _editorDay == DateOnly.FromDateTime(DateTime.Today);

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

    /// <summary>The month as a map of moods, in the middle of the journal; the card under it is the day chosen here.</summary>
    public bool ShowCalendar => !ShowEmptyState;

    private bool _hasEarlierEntries;

    /// <summary>The way to the overview on a quiet week, for someone with check-ins from before it; a newcomer has nothing to look back on.</summary>
    public bool ShowEarlierWeeks => ShowEmptyState && _hasEarlierEntries;

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

    /// <summary>The day chosen in the calendar, above its morning and evening.</summary>
    public string DayHeaderText => AppStrings.JournalEditorDayTitle(_editorDay);

    private JournalSlotPreview? _morning;
    private JournalSlotPreview? _evening;

    public bool IsMorningSelected => _editorSlot == JournalCheckInSlot.Morning;
    public bool IsEveningSelected => _editorSlot == JournalCheckInSlot.Evening;
    public bool HasMorning => _morning is not null;
    public bool HasEvening => _evening is not null;
    public bool IsMorningEmpty => _morning is null;
    public bool IsEveningEmpty => _evening is null;

    public int? MorningTileLevel => _morning?.MoodLevel;
    public int? EveningTileLevel => _evening?.MoodLevel;
    public int MorningMoodLevel => _morning?.MoodLevel ?? 0;
    public int EveningMoodLevel => _evening?.MoodLevel ?? 0;
    public string MorningIcon => MoodPalette.IconName(MorningMoodLevel);
    public string EveningIcon => MoodPalette.IconName(EveningMoodLevel);
    public string MorningSubtitle => SlotSubtitle(_morning);
    public string EveningSubtitle => SlotSubtitle(_evening);

    /// <summary>What a screen reader says for a row: the slot, then its mood in words and its note, or that it is still to fill.</summary>
    public string MorningDescription => SlotDescription(AppStrings.JournalSlotMorning, _morning);
    public string EveningDescription => SlotDescription(AppStrings.JournalSlotEvening, _evening);

    private static string SlotSubtitle(JournalSlotPreview? slot) =>
        slot is null ? AppStrings.JournalSlotAdd : slot.Note ?? AppStrings.MoodLevelName(slot.MoodLevel);

    private static string SlotDescription(string slotName, JournalSlotPreview? slot) =>
        slot is null
            ? slotName + ". " + AppStrings.JournalSlotAdd
            : slotName + ". " + AppStrings.MoodLevelName(slot.MoodLevel) + (slot.Note is null ? string.Empty : ". " + slot.Note);

    public string MoodSavedStatus =>
        HasMoodSelected ? AppStrings.TodayMoodSaved : string.Empty;

    public bool CanGoNextMonth =>
        _monthCursor < new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);

    private string _monthTitle = string.Empty;
    public string MonthTitle
    {
        get => _monthTitle;
        private set => SetProperty(ref _monthTitle, value);
    }

    private IReadOnlyList<JournalMonthCell> _monthCells = [];
    public IReadOnlyList<JournalMonthCell> MonthCells
    {
        get => _monthCells;
        private set => SetProperty(ref _monthCells, value);
    }

    public ObservableCollection<JournalActivityChip> ActivityChips { get; }

    public ICommand OpenOverviewCommand { get; }
    public ICommand OpenTimelineCommand { get; }

    public ICommand PrevMonthCommand { get; }
    public ICommand NextMonthCommand { get; }
    public ICommand SelectSlotCommand { get; }
    public ICommand FlushNoteCommand { get; }
    public ICommand ShowFactorsCommand { get; }
    public ICommand OpenLowMoodBreathCommand { get; }
    public ICommand OpenLowMoodAnchorsCommand { get; }
    public ICommand OpenLowMoodHelpCommand { get; }
    public ICommand RecordMoodCommand { get; }
    public ICommand ToggleFactorCommand { get; }
    public ICommand SelectDayCommand { get; }

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
                OnPropertyChanged(nameof(ShowNoteEditor));
                OnPropertyChanged(nameof(MoodSavedStatus));
                NotifyEmptyState();
            }
        }
    }

    public bool HasMoodSelected => SelectedMoodLevel is >= 1 and <= 5;
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
            nameof(MonthNavPrevLabel),
            nameof(MonthNavNextLabel),
            nameof(EarlierWeeksText),
            nameof(SlotMorningLabel),
            nameof(SlotEveningLabel),
            nameof(DayHeaderText),
            nameof(MoodSavedStatus),
            nameof(HasMoodSelected),
            nameof(ShowNoteEditor),
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


        NotifySlots();
    }

    /// <summary>
    /// On every appearing. Without a day asked for by another page the journal opens on today and the current slot again: a past day
    /// left open in the card read as today's check-in when the person came back, and the face tapped then went to that day.
    /// </summary>
    public Task ReloadAsync()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly day = _editorContext.ConsumePendingEditorDay() ?? today;
        // The slot stays when the day does (a check-in of today the person was on); back to today it is the current one.
        JournalCheckInSlot slot = day == _editorDay || day != today ? _editorSlot : CurrentSlot();
        _monthCursor = new DateOnly(day.Year, day.Month, 1);
        MoveEditor(day, slot);
        return LoadAsync();
    }

    private static JournalCheckInSlot CurrentSlot() =>
        MoodCheckInSlotPolicy.IsMorningLocalHour(DateTime.Now.Hour) ? JournalCheckInSlot.Morning : JournalCheckInSlot.Evening;

    /// <summary>Points the editor at another check-in; until it loads, the one on screen is not saved over it.</summary>
    private void MoveEditor(DateOnly day, JournalCheckInSlot slot)
    {
        if (day == _editorDay && slot == _editorSlot)
        {
            return;
        }

        _editorDay = day;
        _editorSlot = slot;
        _editorStale = true;
        CancelNoteAutosave();
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
        MoveEditor(day, _editorSlot);
        _factorsOpened = false;
        await LoadAsync();
    }

    private async Task SelectSlotAsync(JournalCheckInSlot slot)
    {
        if (_editorSlot == slot)
        {
            return;
        }

        await FlushPendingNoteSaveAsync();
        MoveEditor(_editorDay, slot);
        _factorsOpened = false;
        NotifySlots();
        await LoadAsync();
    }

    /// <summary>Another month in the calendar; the open day stays open below until a day of that month is tapped.</summary>
    private void ShiftMonth(int direction)
    {
        DateOnly next = JournalMoodLoader.ClampMonth(_monthCursor.AddMonths(direction), DateOnly.FromDateTime(DateTime.Today));
        if (next == _monthCursor)
        {
            return;
        }

        _monthCursor = next;
        OnPropertyChanged(nameof(CanGoNextMonth));
        LoadAsync().FireAndForget();
    }

    private void NotifySlots() =>
        Notify(nameof(IsMorningSelected), nameof(IsEveningSelected), nameof(HasMorning), nameof(HasEvening), nameof(IsMorningEmpty),
            nameof(IsEveningEmpty), nameof(MorningTileLevel), nameof(EveningTileLevel), nameof(MorningMoodLevel), nameof(EveningMoodLevel),
            nameof(MorningIcon), nameof(EveningIcon), nameof(MorningSubtitle), nameof(EveningSubtitle), nameof(MorningDescription),
            nameof(EveningDescription));

    private void NotifyEmptyState() => Notify(nameof(ShowEmptyState), nameof(ShowCalendar), nameof(ShowEarlierWeeks));

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
                monthCursor: _monthCursor,
                editorSlot: _editorSlot,
                calendarScale: JournalCalendarScale.Month);
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

    private void ApplySnapshot(JournalMoodSnapshot snapshot)
    {
        CancelNoteAutosave();
        _noteDirty = false;
        MonthCells = snapshot.MonthCells;
        _monthCursor = snapshot.MonthCursor;
        MonthTitle = snapshot.MonthTitle;
        _editorEntryId = snapshot.EditorEntryId;
        _editorDay = snapshot.EditorDay;
        _editorSlot = snapshot.EditorSlot;
        _editorStale = false;
        _morning = snapshot.Morning;
        _evening = snapshot.Evening;
        NotifySlots();
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
        _hasEarlierEntries = snapshot.HasEarlierEntries;

        Notify(nameof(HasWeekStats), nameof(WeekSummaryText), nameof(PromptText), nameof(NotePlaceholder));
        NotifyEmptyState();
        OnPropertyChanged(nameof(DayHeaderText));
        Notify(nameof(ShowPrompt), nameof(NotePlaceholder));
        NotifyLowMoodSupport();
        OnPropertyChanged(nameof(ShowNoteEditor));
        OnPropertyChanged(nameof(MoodSavedStatus));
        OnPropertyChanged(nameof(CanGoNextMonth));
    }

    private async Task SaveMoodAsync(bool reload = true)
    {
        if (SelectedMoodLevel is < 1 or > 5 || _editorStale)
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
