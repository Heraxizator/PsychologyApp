using System.Collections.ObjectModel;
using System.Globalization;
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
    private readonly IShellTabNavigator _shellTabNavigator;
    private readonly INavigationService _navigationService;
    private int _loadGeneration;
    private long? _editorEntryId;
    private DateOnly _editorDay = DateOnly.FromDateTime(DateTime.Today);
    private DateOnly _weekStripEnd = DateOnly.FromDateTime(DateTime.Today);
    private JournalCheckInSlot _editorSlot =
        DateTime.Now.Hour < 15 ? JournalCheckInSlot.Morning : JournalCheckInSlot.Evening;
    private CancellationTokenSource? _noteSaveCts;
    private bool _suppressNoteAutosave;
    private bool _noteDirty;

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
        BackCommand = new AsyncCommand(async () =>
        {
            await FlushPendingNoteSaveAsync();
            await navigationService.GoBackAsync();
        });
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
                SaveMoodAsync().FireAndForget();
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
    public string NotePlaceholder => AppStrings.JournalNotePlaceholder;
    public string PracticeSuggestText => AppStrings.JournalTryShortPractice;
    public string OpenOverviewLabel => AppStrings.JournalOpenOverview;
    public string OpenTimelineLabel => AppStrings.JournalOpenTimeline;
    public string SlotMorningLabel => AppStrings.JournalSlotMorning;
    public string SlotEveningLabel => AppStrings.JournalSlotEvening;

    public string DayHeaderText =>
        $"{AppStrings.JournalEditorDayTitle(_editorDay)}, {(_editorSlot == JournalCheckInSlot.Evening ? SlotEveningLabel : SlotMorningLabel).ToLower(CultureInfo.CurrentCulture)}";

    public string MoodSavedStatus =>
        HasMoodSelected ? AppStrings.TodayMoodSaved : string.Empty;

    public bool CanGoNextWeek =>
        _weekStripEnd < DateOnly.FromDateTime(DateTime.Today);

    public ObservableCollection<JournalActivityChip> ActivityChips { get; }

    public ICommand BackCommand { get; }
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
            nameof(CanGoNextWeek));
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
        catch
        {
            // Journal content is optional; empty state still works.
        }
    }

    private void ApplySnapshot(JournalMoodSnapshot snapshot)
    {
        CancelNoteAutosave();
        _noteDirty = false;
        WeekDays = snapshot.WeekDays;
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
        await _journalMoodLoader.SaveMoodAsync(
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
    }
}
