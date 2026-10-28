using System.Windows.Input;
using PsychologyApp.Application.Articles;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using PsychologyApp.Presentation.Widgets.LibraryCard;

namespace PsychologyApp.Presentation.Pages.ReadArticles.ForYou;

/// <summary>
/// The "For you" top tab of the Library, from the device's own data: a course in progress, an article not read to the end, picks from
/// recent questionnaires and the mood journal, and the saved articles. Empty, it says how it fills up instead of showing nothing.
/// </summary>
public sealed class ForYouViewModel : BaseViewModel
{
    /// <summary>The same back arrow as on every other pushed page (the page hides the system bar).</summary>
    public ICommand BackCommand { get; }

    private const int MoodDays = 30;

    private readonly INavigationService _navigation;
    private readonly IReadingService _reading;
    private readonly IUserProgressService _progress;
    private readonly IDatabaseReadySignal _databaseReady;
    private IReadOnlyList<LibraryCardItem> _picks = [];
    private IReadOnlyList<LibraryCardItem> _saved = [];
    private IReadOnlyList<LibraryCardItem> _starters = [];
    private bool _loaded;

    public ForYouViewModel(INavigationService navigation, IReadingService reading, IUserProgressService progress, IDatabaseReadySignal databaseReady)
    {
        BindNavigation(navigation);
        BackCommand = new AsyncCommand(GoBackAsync);
        _navigation = navigation;
        _reading = reading;
        _progress = progress;
        _databaseReady = databaseReady;
        TakeTestCommand = new AsyncCommand(() => _navigation.GoToTestsTabAsync());
    }

    public string PageTitle => AppStrings.ArticlesForYou;
    public string SavedTitle => AppStrings.ForYouSavedTitle;
    public string EmptyText => AppStrings.ForYouEmpty;
    public string TakeTestText => AppStrings.ForYouTakeTest;
    public ICommand TakeTestCommand { get; }

    public IReadOnlyList<LibraryCardItem> Picks
    {
        get => _picks;
        private set => SetProperty(ref _picks, value);
    }

    public IReadOnlyList<LibraryCardItem> Saved
    {
        get => _saved;
        private set => SetProperty(ref _saved, value);
    }

    /// <summary>
    /// With nothing to go on yet, a short way in instead of an empty page: what anxiety is, the anxiety course and the article on
    /// sleep, the most common reasons to open the app.
    /// </summary>
    public IReadOnlyList<LibraryCardItem> Starters
    {
        get => _starters;
        private set => SetProperty(ref _starters, value);
    }

    public bool HasPicks => Picks.Count > 0;
    public bool HasSaved => Saved.Count > 0;

    /// <summary>Shown once loaded and there is nothing to suggest or keep.</summary>
    public bool IsEmpty => _loaded && !HasPicks && !HasSaved;

    /// <summary>Called whenever the page appears: a course day closed, an article read or a test taken changes it.</summary>
    public async Task RefreshAsync()
    {
        try
        {
            await _databaseReady.WaitAsync();
            ReadingState state = await _reading.GetAsync();
            IReadOnlyDictionary<string, TestResultDTO> results = await _progress.GetLatestTestResultsAsync(ReadingRecommender.TestIds);
            IReadOnlyList<MoodEntryDTO> moods = await _progress.GetRecentMoodsAsync(MoodDays);
            IReadOnlyList<ReadingPick> picks = ReadingRecommender.Pick([.. results.Values], moods, state, DateTime.UtcNow);
            string language = AppStrings.Language;
            await UiThread.RunAsync(() =>
            {
                Picks = [.. picks.Select(pick => Card(pick, state, language)).OfType<LibraryCardItem>()];
                Saved = [.. state.Saved.Select(id => ArticleContent.Find(id, language)).OfType<Article>().Select(article => LibraryCards.Article(article, state, _navigation))];
                Starters = HasPicks || HasSaved ? [] : StarterCards(state, language);
                _loaded = true;
                Notify(nameof(HasPicks), nameof(HasSaved), nameof(IsEmpty));
            });
        }
        catch (Exception loadError)
        {
            // The personal part is missing this time; the other Library tabs still work.
            BestEffort.Report(loadError);
        }
    }

    private LibraryCardItem? Card(ReadingPick pick, ReadingState state, string language)
    {
        string reason = pick.Reason switch
        {
            ReadingPickReason.ContinueCourse => AppStrings.PickContinueCourse,
            ReadingPickReason.ContinueReading => AppStrings.PickContinueReading,
            ReadingPickReason.Mood => AppStrings.PickMood,
            _ => AppStrings.PickTest(TestName(pick.TestId))
        };

        return pick.IsCourse
            ? ArticleContent.FindCourse(pick.Id, language) is { } course ? LibraryCards.Course(course, state, _navigation, reason) : null
            : ArticleContent.Find(pick.Id, language) is { } article ? LibraryCards.Article(article, state, _navigation, reason) : null;
    }

    private List<LibraryCardItem> StarterCards(ReadingState state, string language)
    {
        List<LibraryCardItem> cards = [];
        if (ArticleContent.Find(ArticleIds.Anxiety, language) is { } anxiety)
        {
            cards.Add(LibraryCards.Article(anxiety, state, _navigation, AppStrings.PickStarter));
        }

        if (ArticleContent.FindCourse(CourseIds.Anxiety, language) is { } course)
        {
            cards.Add(LibraryCards.Course(course, state, _navigation, AppStrings.PickStarter));
        }

        if (ArticleContent.Find(ArticleIds.Sleep, language) is { } sleep)
        {
            cards.Add(LibraryCards.Article(sleep, state, _navigation, AppStrings.PickStarter));
        }

        return cards;
    }

    private static string TestName(string? testId) => testId switch
    {
        "pcl5" => "PCL-5",
        "pcptsd5" => "PC-PTSD-5",
        "phq9" => "PHQ-9",
        "isi" => "ISI-7",
        "gad7" => "GAD-7",
        _ => testId ?? string.Empty
    };

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(SavedTitle), nameof(EmptyText), nameof(TakeTestText));
        RefreshAsync().FireAndForget();
    }
}
