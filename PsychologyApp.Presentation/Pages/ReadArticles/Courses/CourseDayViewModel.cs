using System.Windows.Input;
using PsychologyApp.Application.Articles;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using PsychologyApp.Presentation.Widgets.ArticleBlocks;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Courses;

/// <summary>One day of a course: the reading, the task until tomorrow, a question to answer in one's own words, and closing the day.</summary>
public sealed class CourseDayViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private readonly IReadingService _reading;
    private readonly IDialogService _dialogs;
    private readonly IDatabaseReadySignal _databaseReady;
    private readonly string _courseId;
    private readonly int _day;
    private readonly ReadAloud _readAloud = new();
    private IReadOnlyList<ArticleBlockItem> _blocks = [];
    private IReadOnlyList<string> _spoken = [];
    private string _title = string.Empty;
    private string _caption = string.Empty;
    private string _question = string.Empty;
    private string _reflection = string.Empty;
    private bool _isDone;
    private bool _isOpen;
    private bool _busy;

    public CourseDayViewModel(
        INavigationService navigation,
        IReadingService reading,
        IDialogService dialogs,
        IDatabaseReadySignal databaseReady,
        string courseId,
        int day)
    {
        BindNavigation(navigation);
        _navigation = navigation;
        _reading = reading;
        _dialogs = dialogs;
        _databaseReady = databaseReady;
        _courseId = courseId;
        _day = day;
        BackCommand = new AsyncCommand(() => _navigation.GoBackAsync());
        CompleteCommand = new AsyncCommand(CompleteAsync);
        ListenCommand = new AsyncCommand(() => _readAloud.ToggleAsync(_spoken));
        _readAloud.Changed += () => UiThread.RunAsync(() => Notify(nameof(ListenText), nameof(ListenIcon))).FireAndForget();
        Load();
        LoadStateAsync().FireAndForget();
    }

    public ICommand BackCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand ListenCommand { get; }

    public string Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    public string Caption
    {
        get => _caption;
        private set => SetProperty(ref _caption, value);
    }

    public string Question
    {
        get => _question;
        private set => SetProperty(ref _question, value);
    }

    public IReadOnlyList<ArticleBlockItem> Blocks
    {
        get => _blocks;
        private set => SetProperty(ref _blocks, value);
    }

    /// <summary>The person's own words. Kept on the device only; never in a log or a report.</summary>
    public string Reflection
    {
        get => _reflection;
        set => SetProperty(ref _reflection, value);
    }

    public string ReflectionPlaceholder => AppStrings.CourseReflectionPlaceholder;
    public string CompleteText => AppStrings.CourseCompleteDay;
    public string DoneText => AppStrings.CourseDayDone;
    public string ListenText => _readAloud.IsSpeaking ? AppStrings.ArticleStopListening : AppStrings.ArticleListen;
    public string ListenIcon => _readAloud.IsSpeaking ? "StopCircle" : "VolumeUp";

    public bool IsDayDone
    {
        get => _isDone;
        private set
        {
            if (SetProperty(ref _isDone, value))
            {
                Notify(nameof(CanComplete), nameof(CanWrite));
            }
        }
    }

    public bool IsOpen
    {
        get => _isOpen;
        private set
        {
            if (SetProperty(ref _isOpen, value))
            {
                Notify(nameof(CanComplete), nameof(CanWrite));
            }
        }
    }

    public bool CanComplete => IsOpen && !IsDayDone;
    public bool CanWrite => CanComplete;

    public void StopListening() => _readAloud.Stop();

    private void Load()
    {
        if (ArticleContent.FindCourse(_courseId, AppStrings.Language) is not { } course || _day < 1 || _day > course.Days.Count)
        {
            return;
        }

        CourseDay day = course.Days[_day - 1];
        Title = day.Title;
        Caption = $"{course.Title} · {AppStrings.CourseDayLabel(_day)}";
        Question = day.Reflection;
        IReadOnlyList<ArticleBlock> blocks = [.. day.Blocks, new ArticleBlock(ArticleBlockKind.Task, day.Task)];
        Blocks = ArticleBlockItems.Build(blocks, _navigation);
        _spoken = ArticleSpeech.Parts(day.Title, blocks);
    }

    private async Task LoadStateAsync()
    {
        await _databaseReady.WaitAsync();
        CourseProgress? progress = (await _reading.GetAsync()).Course(_courseId);
        CourseDayRecord? record = progress?.Days.FirstOrDefault(entry => entry.Day == _day);
        CourseDayState state = _reading.StateOf(_day, progress);
        await UiThread.RunAsync(() =>
        {
            IsDayDone = state == CourseDayState.Done;
            IsOpen = state is CourseDayState.Open or CourseDayState.Done;
            if (record?.Reflection is { } words)
            {
                Reflection = words;
            }
        });
    }

    private async Task CompleteAsync()
    {
        if (_busy || !CanComplete)
        {
            return;
        }

        _busy = true;
        try
        {
            await _reading.StartCourseAsync(_courseId);
            CourseDayOutcome outcome = await _reading.CompleteDayAsync(_courseId, _day, Reflection);
            switch (outcome)
            {
                case CourseDayOutcome.Crisis:
                    // What was written matters more than the course: help first, the day stays open.
                    await _navigation.GoToCrisisHubAsync();
                    break;

                case CourseDayOutcome.NotOpen:
                    await _dialogs.ShowAsync(null, AppStrings.CourseDayNotOpen);
                    break;

                default:
                    IsDayDone = true;
                    UiHaptics.Tick();
                    bool finished = (await _reading.GetAsync()).Course(_courseId)?.FinishedUtc is not null;
                    await _dialogs.ShowAsync(null, finished ? AppStrings.CourseAllDone : AppStrings.CourseDayClosed(_day));
                    await _navigation.GoBackAsync();
                    break;
            }
        }
        finally
        {
            _busy = false;
        }
    }

    protected override void RefreshLocalizedProperties()
    {
        Load();
        Notify(nameof(ReflectionPlaceholder), nameof(CompleteText), nameof(DoneText), nameof(ListenText));
    }
}
