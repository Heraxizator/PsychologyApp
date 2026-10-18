using System.Windows.Input;
using PsychologyApp.Application.Articles;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Courses;

public sealed class CourseDayItem
{
    public required string NumberText { get; init; }
    public required string Title { get; init; }
    public required string StateText { get; init; }
    public required string Icon { get; init; }
    public required bool IsAvailable { get; init; }
    public required ICommand OpenCommand { get; init; }
    public double Opacity => IsAvailable ? 1 : 0.6;
}

/// <summary>A course: what it is, the measure at the start and the end, and the days with their state (done, open, tomorrow, later).</summary>
public sealed class CourseViewModel : BaseViewModel
{
    private const int HistoryLimit = 50;

    private readonly INavigationService _navigation;
    private readonly IReadingService _reading;
    private readonly IUserProgressService _progress;
    private readonly IDialogService _dialogs;
    private readonly IDatabaseReadySignal _databaseReady;
    private readonly string _courseId;
    private Course? _course;
    private CourseProgress? _state;
    private IReadOnlyList<CourseDayItem> _days = [];
    private string _measureText = string.Empty;
    private string _progressText = string.Empty;
    private string _primaryText = string.Empty;
    private bool _canAct = true;

    public CourseViewModel(
        INavigationService navigation,
        IReadingService reading,
        IUserProgressService progress,
        IDialogService dialogs,
        IDatabaseReadySignal databaseReady,
        string courseId)
    {
        BindNavigation(navigation);
        _navigation = navigation;
        _reading = reading;
        _progress = progress;
        _dialogs = dialogs;
        _databaseReady = databaseReady;
        _courseId = courseId;
        BackCommand = new AsyncCommand(() => _navigation.GoBackAsync());
        PrimaryCommand = new AsyncCommand(StartOrContinueAsync);
        TakeTestCommand = new AsyncCommand(() => _course is { } course ? _navigation.GoToTestAsync(course.TestId) : Task.CompletedTask);
        ResetCommand = new AsyncCommand(ResetAsync);
        _course = ArticleContent.FindCourse(courseId, AppStrings.IsEnglish(AppStrings.Language));
    }

    public ICommand BackCommand { get; }
    public ICommand PrimaryCommand { get; }
    public ICommand TakeTestCommand { get; }
    public ICommand ResetCommand { get; }

    public string Title => _course?.Title ?? string.Empty;
    public string Summary => _course?.Summary ?? string.Empty;
    public string Icon => _course?.Icon ?? "AutoStories";
    public string MeasureTitle => AppStrings.CourseMeasureTitle;
    public string MeasureHint => _course is { } course ? AppStrings.CourseMeasureHint(course.TestName) : string.Empty;
    public string TakeTestText => _course is { } course ? AppStrings.CourseTakeTest(course.TestName) : string.Empty;
    public string ResetText => AppStrings.CourseReset;
    public bool IsStarted => _state is not null;

    public string MeasureText
    {
        get => _measureText;
        private set
        {
            if (SetProperty(ref _measureText, value))
            {
                OnPropertyChanged(nameof(HasMeasure));
            }
        }
    }

    public bool HasMeasure => MeasureText.Length > 0;

    public string CourseProgressText
    {
        get => _progressText;
        private set => SetProperty(ref _progressText, value);
    }

    public string PrimaryText
    {
        get => _primaryText;
        private set => SetProperty(ref _primaryText, value);
    }

    public bool CanAct
    {
        get => _canAct;
        private set => SetProperty(ref _canAct, value);
    }

    public IReadOnlyList<CourseDayItem> Days
    {
        get => _days;
        private set => SetProperty(ref _days, value);
    }

    /// <summary>Called whenever the page appears: a day closed on the day screen changes this one.</summary>
    public async Task RefreshAsync()
    {
        if (_course is not { } course)
        {
            return;
        }

        await _databaseReady.WaitAsync();
        ReadingState reading = await _reading.GetAsync();
        IReadOnlyList<TestResultDTO> history = await _progress.GetTestResultHistoryAsync(course.TestId, HistoryLimit);
        await UiThread.RunAsync(() => Apply(course, reading.Course(course.Id), history));
    }

    private void Apply(Course course, CourseProgress? state, IReadOnlyList<TestResultDTO> history)
    {
        _state = state;
        CourseMeasurement measure = CourseMeasure.Of(history, state?.StartedUtc);
        MeasureText = measure switch
        {
            { Before: { } before, Now: { } now } => AppStrings.CourseMeasureChange(course.TestName, before, now),
            { Now: { } now } => AppStrings.CourseMeasureOnce(course.TestName, now),
            _ => string.Empty
        };

        int done = state?.Days.Count ?? 0;
        CourseProgressText = state is null ? AppStrings.ArticlesCourseMeta(course.Days.Count, course.Days.Max(day => day.Minutes)) : AppStrings.CourseProgress(done, course.Days.Count);
        int? next = CourseRules.NextDay(course, state);
        CourseDayState? nextState = next is { } day ? _reading.StateOf(day, state) : null;
        PrimaryText = state is null ? AppStrings.CourseStart
            : next is null ? AppStrings.CourseFinished
            : nextState == CourseDayState.Open ? AppStrings.CourseContinue(next.Value)
            : AppStrings.CourseDayTomorrow;
        CanAct = state is null || nextState == CourseDayState.Open;

        Days = course.Days.Select((day, index) =>
        {
            int number = index + 1;
            CourseDayState dayState = _reading.StateOf(number, state);
            bool available = dayState is CourseDayState.Done or CourseDayState.Open;
            return new CourseDayItem
            {
                NumberText = AppStrings.CourseDayLabel(number),
                Title = day.Title,
                StateText = dayState switch
                {
                    CourseDayState.Done => AppStrings.CourseDayDone,
                    CourseDayState.Open => AppStrings.ArticleMinutes(day.Minutes),
                    CourseDayState.Tomorrow => AppStrings.CourseDayTomorrow,
                    _ => AppStrings.CourseDayLocked
                },
                Icon = dayState switch
                {
                    CourseDayState.Done => "TaskAlt",
                    CourseDayState.Open => "AutoStories",
                    _ => "LockClock"
                },
                IsAvailable = available,
                OpenCommand = new AsyncCommand(() => available ? _navigation.GoToCourseDayAsync(course.Id, number) : Task.CompletedTask)
            };
        }).ToList();
        Notify(nameof(IsStarted));
    }

    private async Task StartOrContinueAsync()
    {
        if (_course is not { } course || !CanAct)
        {
            return;
        }

        if (_state is null)
        {
            await _reading.StartCourseAsync(course.Id);
        }

        await _navigation.GoToCourseDayAsync(course.Id, CourseRules.NextDay(course, _state) ?? 1);
    }

    private async Task ResetAsync()
    {
        if (_course is not { } course
            || !await _dialogs.AskAsync(course.Title, AppStrings.CourseResetConfirm, AppStrings.Yes, AppStrings.Cancel))
        {
            return;
        }

        await _reading.ResetCourseAsync(course.Id);
        await RefreshAsync();
    }

    protected override void RefreshLocalizedProperties()
    {
        _course = ArticleContent.FindCourse(_courseId, AppStrings.IsEnglish(AppStrings.Language));
        Notify(nameof(Title), nameof(Summary), nameof(MeasureTitle), nameof(MeasureHint), nameof(TakeTestText), nameof(ResetText));
        RefreshAsync().FireAndForget();
    }
}
