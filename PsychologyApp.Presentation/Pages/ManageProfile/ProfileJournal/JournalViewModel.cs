using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileJournal;

public sealed class JournalViewModel : BaseViewModel
{
    private readonly ProfileMoodLoader _profileMoodLoader;
    private readonly INavigationService _navigationService;
    private int _loadGeneration;

    public JournalViewModel(ProfileMoodLoader profileMoodLoader, INavigationService navigationService)
    {
        _profileMoodLoader = profileMoodLoader;
        _navigationService = navigationService;
        BindNavigation(navigationService);
        ModuleName = AppStrings.ProfileTitle;
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
                RecordMoodAsync(level).FireAndForget();
            }
        });
        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalTitle;
    public string MoodCheckInTitle => AppStrings.ProfileMoodCheckInTitle;
    public string MoodTrendTitle => AppStrings.ProfileMoodTrendTitle;
    public string MoodTrendHint => AppStrings.ProfileMoodTrendHint;
    public bool ShowMoodTrendHint => !HasMoodTrendChart;
    public string MoodNotesTitle => AppStrings.ProfileMoodNotesTitle;
    public string WeeklyInsightTitle => AppStrings.ProfileWeeklyInsightTitle;
    public string NotePlaceholder => AppStrings.JournalNotePlaceholder;

    public ICommand BackCommand { get; }
    public ICommand RecordMoodCommand { get; }

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

    private IReadOnlyList<MoodNoteItem> _moodNotes = [];
    public IReadOnlyList<MoodNoteItem> MoodNotes
    {
        get => _moodNotes;
        private set
        {
            if (SetProperty(ref _moodNotes, value))
            {
                OnPropertyChanged(nameof(HasMoodNotes));
            }
        }
    }

    public bool HasMoodNotes => MoodNotes.Count > 0;

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

    private string _weeklyInsightText = string.Empty;
    public string WeeklyInsightText
    {
        get => _weeklyInsightText;
        private set
        {
            if (SetProperty(ref _weeklyInsightText, value))
            {
                OnPropertyChanged(nameof(HasWeeklyInsight));
            }
        }
    }

    public bool HasWeeklyInsight => !string.IsNullOrWhiteSpace(WeeklyInsightText);

    private string _journalNote = string.Empty;
    public string JournalNote
    {
        get => _journalNote;
        set => SetProperty(ref _journalNote, value);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MoodCheckInTitle),
            nameof(MoodTrendTitle),
            nameof(MoodTrendHint),
            nameof(ShowMoodTrendHint),
            nameof(MoodNotesTitle),
            nameof(WeeklyInsightTitle),
            nameof(NotePlaceholder),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(MoodHistorySummary),
            nameof(HasMoodHistorySummary),
            nameof(WeeklyInsightText),
            nameof(HasWeeklyInsight));
    }

    public Task ReloadAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            ProfileMoodSnapshot snapshot = await _profileMoodLoader.LoadAsync();
            if (generation != Volatile.Read(ref _loadGeneration))
            {
                return;
            }

            await UiThread.RunAsync(() =>
            {
                MoodChartPoints = snapshot.ChartPoints;
                MoodChartSubtitle = snapshot.ChartSubtitle;
                HasMoodTrendChart = snapshot.HasTrendChart;
                MoodNotes = snapshot.RecentNotes;
                SelectedMoodLevel = snapshot.SelectedMoodLevel;
                TodayMoodDisplay = snapshot.TodayMoodDisplay;
                MoodHistorySummary = snapshot.MoodHistorySummary;
                WeeklyInsightText = snapshot.WeeklyInsightText;
            });
        }
        catch
        {
            // Journal content is optional; empty state still works.
        }
    }

    private async Task RecordMoodAsync(int moodLevel)
    {
        SelectedMoodLevel = moodLevel;
        string? note = string.IsNullOrWhiteSpace(JournalNote) ? null : JournalNote.Trim();
        await _profileMoodLoader.RecordMoodAsync(moodLevel, note);
        TodayMoodDisplay = AppStrings.TodayMoodLine(moodLevel, 5);
        JournalNote = string.Empty;
        await LoadAsync();
    }
}
