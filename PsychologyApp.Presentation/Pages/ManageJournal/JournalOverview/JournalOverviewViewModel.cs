using System.Collections.ObjectModel;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageJournal.JournalOverview;

public sealed class JournalOverviewViewModel : BaseViewModel
{
    private readonly JournalMoodLoader _journalMoodLoader;
    private readonly JournalScreenCoordinator _journalScreenCoordinator;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private int _loadGeneration;
    private int _rangeDays = 7;
    private DateOnly _weekStripEnd = DateOnly.FromDateTime(DateTime.Today);
    private DateOnly _monthCursor = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private int _yearCursor = DateTime.Today.Year;
    private JournalCalendarScale _calendarScale = JournalCalendarScale.Week;

    public JournalOverviewViewModel(
        JournalMoodLoader journalMoodLoader,
        JournalScreenCoordinator journalScreenCoordinator,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _journalMoodLoader = journalMoodLoader;
        _journalScreenCoordinator = journalScreenCoordinator;
        _navigationService = navigationService;
        _dialogService = dialogService;
        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        PrevPeriodCommand = new Command(() => ShiftPeriod(-1), () => true);
        NextPeriodCommand = new Command(() => ShiftPeriod(1), () => CanGoNextPeriod);
        ExportCommand = new AsyncCommand(ExportAsync);
        SelectCalendarScaleCommand = new Command<object?>(parameter =>
        {
            JournalCalendarScale? scale = parameter switch
            {
                JournalCalendarScale value => value,
                string key when key == "week" => JournalCalendarScale.Week,
                string key when key == "month" => JournalCalendarScale.Month,
                string key when key == "year" => JournalCalendarScale.Year,
                FilterChipTabItem chip when chip.Key == "week" => JournalCalendarScale.Week,
                FilterChipTabItem chip when chip.Key == "month" => JournalCalendarScale.Month,
                FilterChipTabItem chip when chip.Key == "year" => JournalCalendarScale.Year,
                _ => null
            };

            if (scale is null || scale == _calendarScale)
            {
                return;
            }

            _calendarScale = scale.Value;
            SyncCalendarScaleFilters();
            NotifyPeriodProperties();
            LoadAsync().FireAndForget();
        });
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
        SelectDayCommand = new Command<object?>(parameter =>
        {
            DateOnly? day = parameter switch
            {
                DateOnly date => date,
                JournalDayChip chip => chip.Date,
                JournalMonthCell cell when cell.IsEnabled && cell.Date is DateOnly monthDay => monthDay,
                JournalYearCell yearCell when yearCell.IsEnabled && yearCell.Date is DateOnly yearDay => yearDay,
                _ => null
            };

            if (day is null)
            {
                return;
            }

            _journalScreenCoordinator.OpenEditorDayAsync(day.Value, _navigationService).FireAndForget();
        });

        CalendarScaleFilters =
        [
            new FilterChipTabItem { Key = "week", Title = AppStrings.JournalCalendarScaleWeek, IsSelected = true },
            new FilterChipTabItem { Key = "month", Title = AppStrings.JournalCalendarScaleMonth, IsSelected = false },
            new FilterChipTabItem { Key = "year", Title = AppStrings.JournalCalendarScaleYear, IsSelected = false }
        ];
        RangeFilters =
        [
            new FilterChipTabItem { Key = "7", Title = AppStrings.JournalFilter7Days, IsSelected = true },
            new FilterChipTabItem { Key = "30", Title = AppStrings.JournalFilter30Days, IsSelected = false },
            new FilterChipTabItem { Key = "90", Title = AppStrings.JournalFilter90Days, IsSelected = false }
        ];

        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.JournalOverviewTitle;
    public string MoodTrendTitle => AppStrings.JournalDynamicsTitle;
    public string MoodTrendHint => AppStrings.ProfileMoodTrendHint;
    public bool ShowMoodTrendHint => !HasMoodTrendChart;
    public string ExportLabel => AppStrings.JournalExportLabel;
    public string CalendarSectionTitle => AppStrings.JournalCalendarSectionTitle;
    public string StatsSectionTitle => AppStrings.JournalStatsSectionTitle;
    public string FactorsSectionTitle => AppStrings.JournalFactorsSectionTitle;
    public string WeekMoodCheckInsLabel => AppStrings.WeekMoodCheckInsLabel;
    public string WeekAvgMoodLabel => AppStrings.WeekAvgMoodLabel;
    public string StreakMetricLabel => AppStrings.JournalStreakMetricLabel;
    public string PeriodNavPrevLabel => AppStrings.JournalPeriodNavPrev;
    public string PeriodNavNextLabel => AppStrings.JournalPeriodNavNext;

    public bool IsWeekScale => _calendarScale == JournalCalendarScale.Week;
    public bool IsMonthScale => _calendarScale == JournalCalendarScale.Month;
    public bool IsYearScale => _calendarScale == JournalCalendarScale.Year;

    public string PeriodTitle => _calendarScale switch
    {
        JournalCalendarScale.Month => MonthTitle,
        JournalCalendarScale.Year => YearTitle,
        _ => WeekStripTitle
    };

    public string WeekStripTitle =>
        AppStrings.WeekRangeLabel(_weekStripEnd.AddDays(-6), _weekStripEnd);

    public bool CanGoNextPeriod => _calendarScale switch
    {
        JournalCalendarScale.Month => CanGoNextMonth,
        JournalCalendarScale.Year => CanGoNextYear,
        _ => CanGoNextWeek
    };

    private bool CanGoNextWeek =>
        _weekStripEnd < DateOnly.FromDateTime(DateTime.Today);

    private bool CanGoNextMonth
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly currentMonth = new(today.Year, today.Month, 1);
            return _monthCursor < currentMonth;
        }
    }

    private bool CanGoNextYear => _yearCursor < DateTime.Today.Year;

    public ICommand BackCommand { get; }
    public ICommand SelectRangeCommand { get; }
    public ICommand SelectCalendarScaleCommand { get; }
    public ICommand SelectDayCommand { get; }
    public ICommand PrevPeriodCommand { get; }
    public ICommand NextPeriodCommand { get; }
    public ICommand ExportCommand { get; }
    public ObservableCollection<FilterChipTabItem> CalendarScaleFilters { get; }
    public ObservableCollection<FilterChipTabItem> RangeFilters { get; }

    private IReadOnlyList<JournalDayChip> _weekDays = [];
    public IReadOnlyList<JournalDayChip> WeekDays
    {
        get => _weekDays;
        private set => SetProperty(ref _weekDays, value);
    }

    private IReadOnlyList<JournalMonthCell> _monthCells = [];
    public IReadOnlyList<JournalMonthCell> MonthCells
    {
        get => _monthCells;
        private set => SetProperty(ref _monthCells, value);
    }

    private string _monthTitle = string.Empty;
    public string MonthTitle
    {
        get => _monthTitle;
        private set => SetProperty(ref _monthTitle, value);
    }

    private IReadOnlyList<JournalYearCell> _yearCells = [];
    public IReadOnlyList<JournalYearCell> YearCells
    {
        get => _yearCells;
        private set => SetProperty(ref _yearCells, value);
    }

    private string _yearTitle = string.Empty;
    public string YearTitle
    {
        get => _yearTitle;
        private set => SetProperty(ref _yearTitle, value);
    }

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
        private set => SetProperty(ref _hasMoodStats, value);
    }

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
        private set => SetProperty(ref _moodTrendLabel, value);
    }

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

    private string _overviewInsightText = string.Empty;
    public string OverviewInsightText
    {
        get => _overviewInsightText;
        private set => SetProperty(ref _overviewInsightText, value);
    }

    private string _practiceMoodInsightText = string.Empty;
    public string PracticeMoodInsightText
    {
        get => _practiceMoodInsightText;
        private set
        {
            if (SetProperty(ref _practiceMoodInsightText, value))
            {
                OnPropertyChanged(nameof(HasPracticeMoodInsight));
            }
        }
    }

    public bool HasPracticeMoodInsight => !string.IsNullOrWhiteSpace(PracticeMoodInsightText);

    private IReadOnlyList<string> _activityPills = [];
    public IReadOnlyList<string> ActivityPills
    {
        get => _activityPills;
        private set
        {
            if (SetProperty(ref _activityPills, value))
            {
                OnPropertyChanged(nameof(HasFactorPills));
            }
        }
    }

    public bool HasFactorPills => ActivityPills.Count > 0;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MoodTrendTitle),
            nameof(MoodTrendHint),
            nameof(ShowMoodTrendHint),
            nameof(ExportLabel),
            nameof(CalendarSectionTitle),
            nameof(StatsSectionTitle),
            nameof(FactorsSectionTitle),
            nameof(WeekMoodCheckInsLabel),
            nameof(WeekAvgMoodLabel),
            nameof(StreakMetricLabel),
            nameof(PeriodNavPrevLabel),
            nameof(PeriodNavNextLabel),
            nameof(PeriodTitle),
            nameof(WeekStripTitle),
            nameof(CanGoNextPeriod),
            nameof(IsWeekScale),
            nameof(IsMonthScale),
            nameof(IsYearScale),
            nameof(HasMoodStats),
            nameof(WeekRangeSubtitle),
            nameof(CheckInCountDisplay),
            nameof(AverageMoodDisplay),
            nameof(MoodStreakDisplay),
            nameof(MoodTrendLabel),
            nameof(BestWorstLabel),
            nameof(HasBestWorstPill),
            nameof(OverviewInsightText),
            nameof(PracticeMoodInsightText),
            nameof(HasPracticeMoodInsight),
            nameof(HasFactorPills));
        (NextPeriodCommand as Command)?.ChangeCanExecute();

        foreach (FilterChipTabItem filter in CalendarScaleFilters)
        {
            filter.Title = filter.Key switch
            {
                "month" => AppStrings.JournalCalendarScaleMonth,
                "year" => AppStrings.JournalCalendarScaleYear,
                _ => AppStrings.JournalCalendarScaleWeek
            };
        }

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

    private void ShiftPeriod(int direction)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        switch (_calendarScale)
        {
            case JournalCalendarScale.Month:
                _monthCursor = JournalMoodLoader.ClampMonth(_monthCursor.AddMonths(direction), today);
                break;
            case JournalCalendarScale.Year:
                _yearCursor = JournalMoodLoader.ClampYear(_yearCursor + direction, today);
                break;
            default:
                _weekStripEnd = JournalMoodLoader.ClampStripEnd(_weekStripEnd.AddDays(direction * 7), today);
                break;
        }

        NotifyPeriodProperties();
        LoadAsync().FireAndForget();
    }

    private void NotifyPeriodProperties()
    {
        OnPropertyChanged(nameof(PeriodTitle));
        OnPropertyChanged(nameof(WeekStripTitle));
        OnPropertyChanged(nameof(CanGoNextPeriod));
        OnPropertyChanged(nameof(IsWeekScale));
        OnPropertyChanged(nameof(IsMonthScale));
        OnPropertyChanged(nameof(IsYearScale));
        (NextPeriodCommand as Command)?.ChangeCanExecute();
    }

    private void SyncCalendarScaleFilters()
    {
        string key = _calendarScale switch
        {
            JournalCalendarScale.Month => "month",
            JournalCalendarScale.Year => "year",
            _ => "week"
        };
        foreach (FilterChipTabItem filter in CalendarScaleFilters)
        {
            filter.IsSelected = filter.Key == key;
        }
    }

    private void SyncRangeFilters()
    {
        foreach (FilterChipTabItem filter in RangeFilters)
        {
            filter.IsSelected = filter.Key == _rangeDays.ToString();
        }
    }

    private async Task ExportAsync()
    {
        try
        {
            IReadOnlyList<MoodEntryDTO> moods = await _journalMoodLoader.GetExportMoodsAsync();
            if (moods.Count == 0)
            {
                await _dialogService.ShowAsync(AppStrings.JournalExportTitle, AppStrings.JournalExportEmpty);
                return;
            }

            await JournalCsvExporter.ShareAsync(moods);
        }
        catch
        {
            await _dialogService.ShowAsync(AppStrings.JournalExportTitle, AppStrings.JournalExportEmpty);
        }
    }

    private async Task LoadAsync()
    {
        int generation = Interlocked.Increment(ref _loadGeneration);
        try
        {
            JournalMoodSnapshot snapshot = await _journalMoodLoader.LoadAsync(
                _rangeDays,
                weekStripEnd: _weekStripEnd,
                monthCursor: _monthCursor,
                yearCursor: _yearCursor,
                calendarScale: _calendarScale);
            if (generation != Volatile.Read(ref _loadGeneration))
            {
                return;
            }

            await UiThread.RunAsync(() =>
            {
                _weekStripEnd = snapshot.WeekStripEnd;
                _monthCursor = snapshot.MonthCursor;
                _yearCursor = snapshot.YearCursor;
                WeekDays = snapshot.WeekDays;
                MonthCells = snapshot.MonthCells;
                MonthTitle = snapshot.MonthTitle;
                YearCells = snapshot.YearCells;
                YearTitle = snapshot.YearTitle;
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
                OverviewInsightText = stats.HasStats
                    ? AppStrings.JournalOverviewInsightLine(
                        stats.CheckInCount,
                        stats.AverageMoodDisplay,
                        stats.MoodTrendLabel,
                        stats.MoodStreakDisplay)
                    : AppStrings.JournalOverviewInsightEmpty;
                PracticeMoodInsightText = snapshot.PracticeMoodInsight;
                ActivityPills = snapshot.ActivityInsights
                    .Select(insight => insight.DisplayPill)
                    .ToList();
                NotifyPeriodProperties();
            });
        }
        catch
        {
            // Overview is optional.
        }
    }
}
