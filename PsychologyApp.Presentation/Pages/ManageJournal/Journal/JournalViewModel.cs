using Microsoft.Maui.ApplicationModel.DataTransfer;
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
    private int _loadGeneration;
    private long? _editorEntryId;
    private DateOnly _editorDay = DateOnly.FromDateTime(DateTime.Today);
    private DateOnly _weekStripEnd = DateOnly.FromDateTime(DateTime.Today);

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
        BindNavigation(navigationService);
        ModuleName = AppStrings.JournalTitle;
        PageName = AppStrings.JournalTitle;
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        OpenOverviewCommand = new AsyncCommand(() => navigationService.GoToJournalOverviewAsync());
        OpenTimelineCommand = new AsyncCommand(() => navigationService.GoToJournalTimelineAsync());
        OpenPracticeSuggestCommand = new Command(() => _shellTabNavigator.OpenPracticeTab());
        ShareCommand = new AsyncCommand(ShareAsync);
        PrevWeekCommand = new Command(() => ShiftWeek(-7));
        NextWeekCommand = new Command(() => ShiftWeek(7), () => CanGoNextWeek);
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
        SaveMoodCommand = new AsyncCommand(SaveMoodAsync);
        DeleteMoodCommand = new AsyncCommand(DeleteMoodAsync);
        ApplyPromptCommand = new Command<object?>(parameter =>
        {
            string? prompt = parameter switch
            {
                "helped" => AppStrings.JournalPromptHelped,
                "next" => AppStrings.JournalPromptNext,
                "blocked" => AppStrings.JournalPromptBlocked,
                "grateful" => AppStrings.JournalPromptGrateful,
                string key => ResolvePromptText(key),
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
        ToggleFactorCommand = new Command<object?>(parameter =>
        {
            if (parameter is not string key)
            {
                return;
            }

            JournalNote = JournalNoteFactors.ToggleFactor(JournalNote, key);
            NotifyFactorSelection();
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

            _editorDay = day.Value;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (_editorDay > _weekStripEnd || _editorDay < _weekStripEnd.AddDays(-6))
            {
                _weekStripEnd = JournalMoodLoader.ClampStripEnd(_editorDay, today);
            }

            LoadAsync().FireAndForget();
        });

        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalTitle;
    public string MoodCheckInTitle =>
        _editorDay == DateOnly.FromDateTime(DateTime.Today)
            ? AppStrings.ProfileMoodCheckInTitle
            : AppStrings.JournalPastDayCheckInTitle;
    public string NotePlaceholder => AppStrings.JournalNotePlaceholder;
    public string NoteSectionTitle => AppStrings.JournalNoteSectionTitle;
    public string QuestionsSectionTitle => AppStrings.JournalQuestionsSectionTitle;
    public string FactorsSectionTitle => AppStrings.JournalFactorsSectionTitle;
    public string DayEmptyCaption => AppStrings.JournalPickMoodHint;
    public string SaveLabel => AppStrings.JournalSaveLabel;
    public string DeleteLabel => AppStrings.JournalDeleteLabel;
    public string ShareLabel => AppStrings.JournalShareLabel;
    public string OpenOverviewLabel => AppStrings.JournalOpenOverview;
    public string OpenTimelineLabel => AppStrings.JournalOpenTimeline;
    public string PromptHelpedLabel => AppStrings.JournalPromptHelpedShort;
    public string PromptNextLabel => AppStrings.JournalPromptNextShort;
    public string PromptBlockedLabel => AppStrings.JournalPromptBlockedShort;
    public string PromptGratefulLabel => AppStrings.JournalPromptGratefulShort;
    public string FactorSleepLabel => AppStrings.JournalFactorSleepLabel;
    public string FactorPeopleLabel => AppStrings.JournalFactorPeopleLabel;
    public string FactorPracticeLabel => AppStrings.JournalFactorPracticeLabel;
    public string PracticeSuggestText => AppStrings.JournalTryShortPractice;
    public string WeekNavPrevLabel => AppStrings.JournalWeekNavPrev;
    public string WeekNavNextLabel => AppStrings.JournalWeekNavNext;

    public string WeekStripTitle =>
        AppStrings.WeekRangeLabel(_weekStripEnd.AddDays(-6), _weekStripEnd);

    public bool CanGoNextWeek =>
        _weekStripEnd < DateOnly.FromDateTime(DateTime.Today);

    public bool IsSleepFactorActive => JournalNoteFactors.HasFactor(JournalNote, JournalNoteFactors.SleepKey);
    public bool IsPeopleFactorActive => JournalNoteFactors.HasFactor(JournalNote, JournalNoteFactors.PeopleKey);
    public bool IsPracticeFactorActive => JournalNoteFactors.HasFactor(JournalNote, JournalNoteFactors.PracticeKey);

    private string _weekInsightText = string.Empty;
    public string WeekInsightText
    {
        get => _weekInsightText;
        private set
        {
            if (SetProperty(ref _weekInsightText, value))
            {
                OnPropertyChanged(nameof(HasWeekInsight));
            }
        }
    }

    public bool HasWeekInsight => !string.IsNullOrWhiteSpace(WeekInsightText);

    public ICommand BackCommand { get; }
    public ICommand OpenOverviewCommand { get; }
    public ICommand OpenTimelineCommand { get; }
    public ICommand OpenPracticeSuggestCommand { get; }
    public ICommand ShareCommand { get; }
    public ICommand PrevWeekCommand { get; }
    public ICommand NextWeekCommand { get; }
    public ICommand RecordMoodCommand { get; }
    public ICommand SaveMoodCommand { get; }
    public ICommand DeleteMoodCommand { get; }
    public ICommand ApplyPromptCommand { get; }
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
                OnPropertyChanged(nameof(CanShareEntry));
            }
        }
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
                OnPropertyChanged(nameof(ShowDayEmptyCaption));
            }
        }
    }

    public bool HasTodayMood => !string.IsNullOrWhiteSpace(TodayMoodDisplay);
    public bool ShowDayEmptyCaption => !HasTodayMood;
    public bool ShowPracticeSuggest =>
        _editorDay == DateOnly.FromDateTime(DateTime.Today)
        && SelectedMoodLevel is 1 or 2;

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

    private string _journalNote = string.Empty;
    public string JournalNote
    {
        get => _journalNote;
        set
        {
            if (SetProperty(ref _journalNote, value))
            {
                NotifyFactorSelection();
            }
        }
    }

    public bool HasEditorEntry => _editorEntryId is > 0;
    public bool CanDeleteEntry => HasEditorEntry;
    public bool CanShareEntry => SelectedMoodLevel is >= 1 and <= 5;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MoodCheckInTitle),
            nameof(NotePlaceholder),
            nameof(NoteSectionTitle),
            nameof(QuestionsSectionTitle),
            nameof(FactorsSectionTitle),
            nameof(DayEmptyCaption),
            nameof(SaveLabel),
            nameof(DeleteLabel),
            nameof(ShareLabel),
            nameof(OpenOverviewLabel),
            nameof(OpenTimelineLabel),
            nameof(PromptHelpedLabel),
            nameof(PromptNextLabel),
            nameof(PromptBlockedLabel),
            nameof(PromptGratefulLabel),
            nameof(FactorSleepLabel),
            nameof(FactorPeopleLabel),
            nameof(FactorPracticeLabel),
            nameof(PracticeSuggestText),
            nameof(WeekNavPrevLabel),
            nameof(WeekNavNextLabel),
            nameof(WeekStripTitle),
            nameof(WeekInsightText),
            nameof(HasWeekInsight),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(ShowDayEmptyCaption),
            nameof(ShowPracticeSuggest),
            nameof(MoodHistorySummary),
            nameof(HasMoodHistorySummary),
            nameof(HasEditorEntry),
            nameof(CanDeleteEntry),
            nameof(CanShareEntry),
            nameof(CanGoNextWeek),
            nameof(IsSleepFactorActive),
            nameof(IsPeopleFactorActive),
            nameof(IsPracticeFactorActive));
        (NextWeekCommand as Command)?.ChangeCanExecute();
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

    private static string? ResolvePromptText(string key) => key switch
    {
        "helped" => AppStrings.JournalPromptHelped,
        "blocked" => AppStrings.JournalPromptBlocked,
        "grateful" => AppStrings.JournalPromptGrateful,
        "next" => AppStrings.JournalPromptNext,
        _ => null
    };

    private void ShiftWeek(int dayDelta)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        _weekStripEnd = JournalMoodLoader.ClampStripEnd(_weekStripEnd.AddDays(dayDelta), today);
        if (_editorDay > _weekStripEnd || _editorDay < _weekStripEnd.AddDays(-6))
        {
            _editorDay = _weekStripEnd;
        }

        OnPropertyChanged(nameof(WeekStripTitle));
        OnPropertyChanged(nameof(CanGoNextWeek));
        (NextWeekCommand as Command)?.ChangeCanExecute();
        LoadAsync().FireAndForget();
    }

    private void NotifyFactorSelection()
    {
        OnPropertyChanged(nameof(IsSleepFactorActive));
        OnPropertyChanged(nameof(IsPeopleFactorActive));
        OnPropertyChanged(nameof(IsPracticeFactorActive));
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
                weekStripEnd: _weekStripEnd);
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
        WeekDays = snapshot.WeekDays;
        _weekStripEnd = snapshot.WeekStripEnd;
        _editorEntryId = snapshot.EditorEntryId;
        _editorDay = snapshot.EditorDay;
        SelectedMoodLevel = snapshot.SelectedMoodLevel;
        TodayMoodDisplay = snapshot.EditorMoodDisplay;
        MoodHistorySummary = snapshot.MoodHistorySummary;
        JournalNote = snapshot.EditorNote ?? string.Empty;
        WeekInsightText = AppStrings.JournalWeekInsightLine(
            snapshot.Stats.CheckInCount,
            snapshot.Stats.MoodTrendLabel,
            snapshot.Stats.MoodStreakDisplay);

        OnPropertyChanged(nameof(HasEditorEntry));
        OnPropertyChanged(nameof(CanDeleteEntry));
        OnPropertyChanged(nameof(MoodCheckInTitle));
        OnPropertyChanged(nameof(ShowPracticeSuggest));
        OnPropertyChanged(nameof(WeekStripTitle));
        OnPropertyChanged(nameof(CanGoNextWeek));
        (NextWeekCommand as Command)?.ChangeCanExecute();
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
        OnPropertyChanged(nameof(ShowPracticeSuggest));
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

    private async Task ShareAsync()
    {
        if (SelectedMoodLevel is < 1 or > 5)
        {
            return;
        }

        string day = _editorDay.ToString("d");
        IReadOnlyList<string> factors = JournalNoteFactors.ExtractActiveLabels(JournalNote);
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = AppStrings.JournalShareTitle,
            Text = AppStrings.JournalShareEntryWithFactors(day, SelectedMoodLevel, JournalNote, factors)
        });
    }
}
