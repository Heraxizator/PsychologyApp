using System.Windows.Input;
using PsychologyApp.Application.Articles;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using PsychologyApp.Presentation.Widgets.LibraryCard;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Courses;

/// <summary>The "Courses" top tab of the Library: what guided self-help is, and the courses with their progress.</summary>
public sealed class CoursesViewModel : BaseViewModel
{
    private readonly INavigationService _navigation;
    private readonly IReadingService _reading;
    private readonly IDatabaseReadySignal _databaseReady;
    private ReadingState _state = new();
    private IReadOnlyList<LibraryCardItem> _courses = [];

    public CoursesViewModel(INavigationService navigation, IReadingService reading, IDatabaseReadySignal databaseReady)
    {
        BindNavigation(navigation);
        _navigation = navigation;
        _reading = reading;
        _databaseReady = databaseReady;
        Load();
    }

    public string PageTitle => AppStrings.ArticlesCourses;
    public string LeadText => AppStrings.ArticlesCoursesLead;

    public IReadOnlyList<LibraryCardItem> Courses
    {
        get => _courses;
        private set => SetProperty(ref _courses, value);
    }

    /// <summary>Called whenever the page appears: a day closed meanwhile changes a course's progress.</summary>
    public async Task RefreshAsync()
    {
        try
        {
            await _databaseReady.WaitAsync();
            ReadingState state = await _reading.GetAsync();
            await UiThread.RunAsync(() =>
            {
                _state = state;
                Load();
            });
        }
        catch (Exception loadError)
        {
            // The courses are in the app; only their progress is missing.
            BestEffort.Report(loadError);
        }
    }

    private void Load() =>
        Courses = [.. ArticleContent.Courses(AppStrings.IsEnglish(AppStrings.Language)).Select(course => LibraryCards.Course(course, _state, _navigation))];

    protected override void RefreshLocalizedProperties()
    {
        Load();
        Notify(nameof(PageTitle), nameof(LeadText));
    }
}
