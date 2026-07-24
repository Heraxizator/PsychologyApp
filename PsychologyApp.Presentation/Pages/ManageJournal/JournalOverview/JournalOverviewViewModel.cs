using System.Collections.ObjectModel;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageJournal.JournalOverview;

public sealed class JournalOverviewViewModel : BaseViewModel
{
    private readonly JournalMoodLoader _journalMoodLoader;
    private int _loadGeneration;
    private int _rangeDays = 7;

    public JournalOverviewViewModel(
        JournalMoodLoader journalMoodLoader,
        INavigationService navigationService)
    {
        _journalMoodLoader = journalMoodLoader;
        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        SelectRangeCommand = new Command<object?>(parameter =>
        {
            int days = parameter switch
            {
                int value => value,
                string text when int.TryParse(text, out int parsed) => parsed,
                FilterChipTabItem chip when int.TryParse(chip.Key, out int keyDays) => keyDays,
                _ => 0
            };

            if (days is not (7 or 30 or 90) || days == _rangeDays)
            {
                return;
            }

            _rangeDays = days;
            SyncRangeFilters();
            LoadAsync().FireAndForget();
        });

        RangeFilters =
        [
            new FilterChipTabItem { Key = "7", Title = AppStrings.JournalFilter7Days, IsSelected = true },
            new FilterChipTabItem { Key = "30", Title = AppStrings.JournalFilter30Days, IsSelected = false },
            new FilterChipTabItem { Key = "90", Title = AppStrings.JournalFilter90Days, IsSelected = false }
        ];

        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalOverviewTitle;
    public string MoodTrendTitle => AppStrings.ProfileMoodTrendTitle;
    public string MoodTrendHint => AppStrings.ProfileMoodTrendHint;
    public bool ShowMoodTrendHint => !HasMoodTrendChart;
    public string StatsSectionTitle => AppStrings.JournalMoodStatsTitle;
    public string WeekEmptyText => AppStrings.JournalWeekEmpty;
    public string WeekMoodCheckInsLabel => AppStrings.WeekMoodCheckInsLabel;
    public string WeekAvgMoodLabel => AppStrings.WeekAvgMoodLabel;
    public string WeekMoodStreakLabel => AppStrings.JournalMoodStreakLabel;

    public ICommand BackCommand { get; }
    public ICommand SelectRangeCommand { get; }
    public ObservableCollection<FilterChipTabItem> RangeFilters { get; }

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

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MoodTrendTitle),
            nameof(MoodTrendHint),
            nameof(ShowMoodTrendHint),
            nameof(StatsSectionTitle),
            nameof(WeekEmptyText),
            nameof(WeekMoodCheckInsLabel),
            nameof(WeekAvgMoodLabel),
            nameof(WeekMoodStreakLabel),
            nameof(HasMoodStats),
            nameof(ShowStatsEmpty),
            nameof(WeekRangeSubtitle),
            nameof(CheckInCountDisplay),
            nameof(AverageMoodDisplay),
            nameof(MoodStreakDisplay),
            nameof(MoodTrendLabel),
            nameof(HasMoodTrendPill),
            nameof(BestWorstLabel),
            nameof(HasBestWorstPill));

        foreach (FilterChipTabItem filter in RangeFilters)
        {
            filter.Title = filter.Key switch
            {
                "90" => AppStrings.JournalFilter90Days,
                "30" => AppStrings.JournalFilter30Days,
                _ => AppStrings.JournalFilter7Days
            };
        }
    }

    public Task ReloadAsync() => LoadAsync();

    private void SyncRangeFilters()
    {
        foreach (FilterChipTabItem filter in RangeFilters)
        {
            filter.IsSelected = filter.Key == _rangeDays.ToString();
        }
    }

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            JournalMoodSnapshot snapshot = await _journalMoodLoader.LoadAsync(_rangeDays);
            if (generation != Volatile.Read(ref _loadGeneration))
            {
                return;
            }

            await UiThread.RunAsync(() =>
            {
                MoodChartPoints = snapshot.ChartPoints;
                MoodChartSubtitle = snapshot.ChartSubtitle;
                HasMoodTrendChart = snapshot.HasTrendChart;
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
            });
        }
        catch
        {
            // Overview is optional.
        }
    }
}
