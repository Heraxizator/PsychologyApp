using Microsoft.Maui.ApplicationModel.DataTransfer;
using PsychologyApp.Presentation.Entities.Journal;
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
    private readonly JournalEditorContext _editorContext;
    private readonly IDialogService _dialogService;
    private int _loadGeneration;
    private long? _editorEntryId;
    private DateOnly _editorDay = DateOnly.FromDateTime(DateTime.Today);

    public JournalViewModel(
        JournalMoodLoader journalMoodLoader,
        JournalEditorContext editorContext,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _journalMoodLoader = journalMoodLoader;
        _editorContext = editorContext;
        _dialogService = dialogService;
        BindNavigation(navigationService);
        ModuleName = AppStrings.JournalTitle;
        PageName = AppStrings.JournalTitle;
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        OpenOverviewCommand = new AsyncCommand(() => navigationService.GoToJournalOverviewAsync());
        OpenTimelineCommand = new AsyncCommand(() => navigationService.GoToJournalTimelineAsync());
        ShareCommand = new AsyncCommand(ShareAsync);
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
    public string NoteSaveHint => AppStrings.JournalNoteSaveHint;
    public string SaveLabel => AppStrings.JournalSaveLabel;
    public string DeleteLabel => AppStrings.JournalDeleteLabel;
    public string ShareLabel => AppStrings.JournalShareLabel;
    public string OpenOverviewLabel => AppStrings.JournalOpenOverview;
    public string OpenTimelineLabel => AppStrings.JournalOpenTimeline;
    public string PromptHelpedLabel => AppStrings.JournalPromptHelpedShort;
    public string PromptNextLabel => AppStrings.JournalPromptNextShort;
    public string WeekStripTitle => AppStrings.JournalRecentDaysTitle;

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
    public ICommand ShareCommand { get; }
    public ICommand RecordMoodCommand { get; }
    public ICommand SaveMoodCommand { get; }
    public ICommand DeleteMoodCommand { get; }
    public ICommand ApplyPromptCommand { get; }
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

    private string _journalNote = string.Empty;
    public string JournalNote
    {
        get => _journalNote;
        set => SetProperty(ref _journalNote, value);
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
            nameof(NoteSaveHint),
            nameof(SaveLabel),
            nameof(DeleteLabel),
            nameof(ShareLabel),
            nameof(OpenOverviewLabel),
            nameof(OpenTimelineLabel),
            nameof(PromptHelpedLabel),
            nameof(PromptNextLabel),
            nameof(WeekStripTitle),
            nameof(WeekInsightText),
            nameof(HasWeekInsight),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(MoodHistorySummary),
            nameof(HasMoodHistorySummary),
            nameof(HasEditorEntry),
            nameof(CanDeleteEntry),
            nameof(CanShareEntry));
    }

    public Task ReloadAsync()
    {
        if (_editorContext.ConsumePendingEditorDay() is DateOnly pendingDay)
        {
            _editorDay = pendingDay;
        }

        return LoadAsync();
    }

    private static string? ResolvePromptText(string key) => key switch
    {
        "helped" => AppStrings.JournalPromptHelped,
        "blocked" => AppStrings.JournalPromptBlocked,
        "grateful" => AppStrings.JournalPromptGrateful,
        "next" => AppStrings.JournalPromptNext,
        _ => key
    };

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            JournalMoodSnapshot snapshot = await _journalMoodLoader.LoadAsync(
                rangeDays: 7,
                filterDay: null,
                editorDay: _editorDay);
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
        OnPropertyChanged(nameof(CanShareEntry));
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
        OnPropertyChanged(nameof(CanShareEntry));
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
        OnPropertyChanged(nameof(CanShareEntry));
        await LoadAsync();
    }

    private async Task ShareAsync()
    {
        if (SelectedMoodLevel is < 1 or > 5)
        {
            return;
        }

        string mood = AppStrings.FormatAverageMood(SelectedMoodLevel);
        string day = _editorDay.ToString("d");
        string note = JournalNote?.Trim() ?? string.Empty;
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = AppStrings.JournalShareTitle,
            Text = AppStrings.JournalShareText(day, mood, note)
        });
    }
}
