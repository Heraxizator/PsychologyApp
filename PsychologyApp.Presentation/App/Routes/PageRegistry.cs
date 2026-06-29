using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.AlternativeTest;
using PsychologyApp.Presentation.Pages.FindProblem;
using PsychologyApp.Presentation.Pages.MusicPlayer;
using PsychologyApp.Presentation.Pages.PhysicsSearch;
using PsychologyApp.Presentation.Pages.ProfileDonate;
using PsychologyApp.Presentation.Pages.ProfileInfo;
using PsychologyApp.Presentation.Pages.ProfileOptions;
using PsychologyApp.Presentation.Pages.ProfileSettings;
using PsychologyApp.Presentation.Pages.ProfileUser;
using PsychologyApp.Presentation.Pages.Question;
using PsychologyApp.Presentation.Pages.QuoteFeed;
using PsychologyApp.Presentation.Features.SendReviewForm;
using PsychologyApp.Presentation.Pages.ReviewForm;
using PsychologyApp.Presentation.Pages.StandardTest;
using PsychologyApp.Presentation.Pages.StartPhysics;
using PsychologyApp.Presentation.Pages.TechniqueCreated;
using PsychologyApp.Presentation.Pages.TechniqueDesigner;
using PsychologyApp.Presentation.Pages.TechniqueSession;
using PsychologyApp.Presentation.Pages.TechniqueTheory;
using PsychologyApp.Presentation.Pages.Techniques;
using PsychologyApp.Presentation.Pages.TestHistory;
using PsychologyApp.Presentation.Pages.TestResult;
using PsychologyApp.Presentation.Pages.TestsList;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Shared.Services.Toasts;

namespace PsychologyApp.Presentation.App.Routes;

/// <summary>
/// App-layer composition root for full-screen pages (canonical FSD routes).
/// </summary>
public sealed class PageRegistry(
    IProfilePageFactory profilePageFactory,
    IReviewPageFactory reviewPageFactory,
    ITestPageFactory testPageFactory,
    ITechniquePageFactory techniquePageFactory,
    IPracticeTheoryNavigator practiceTheoryNavigator,
    IStartPhysicsViewModelFactory startPhysicsViewModelFactory,
    IMusicPlayerViewModelFactory musicPlayerViewModelFactory,
    IQuoteViewModelFactory quoteViewModelFactory,
    IPageViewModelActivator pageViewModelActivator,
    IPhysicsSearchViewModelFactory physicsSearchViewModelFactory,
    IToastService toastService) : IPageFactory
{
    public TechniquesPage CreateTechniquesPage() =>
        WithPressFeedback(techniquePageFactory.CreateTechniquesPage());

    public TestsListPage CreateTestsListPage() =>
        WithPressFeedback(testPageFactory.CreateTestsListPage());

    public TestHistoryPage CreateTestHistoryPage(string testId, string testTitle) =>
        WithPressFeedback(testPageFactory.CreateTestHistoryPage(testId, testTitle));

    public StartPhysicsPage CreateStartPhysicsPage() =>
        WithPressFeedback(new StartPhysicsPage(pageViewModelActivator, startPhysicsViewModelFactory));

    public MusicPlayerPage CreateMusicPlayerPage() =>
        WithPressFeedback(new MusicPlayerPage(musicPlayerViewModelFactory, toastService));

    public QuotePage CreateQuotePage() =>
        WithPressFeedback(new QuotePage(pageViewModelActivator, quoteViewModelFactory));

    public UserPage CreateUserPage() =>
        WithPressFeedback(profilePageFactory.CreateUserPage());

    public OptionsPage CreateOptionsPage() =>
        WithPressFeedback(profilePageFactory.CreateOptionsPage());

    public InfoPage CreateInfoPage() =>
        WithPressFeedback(profilePageFactory.CreateInfoPage());

    public DonatePage CreateDonatePage() =>
        WithPressFeedback(profilePageFactory.CreateDonatePage());

    public FormPage CreateFormPage() =>
        WithPressFeedback(reviewPageFactory.CreateFormPage());

    public SettingsPage CreateSettingsPage() =>
        WithPressFeedback(profilePageFactory.CreateSettingsPage());

    public PhysicsSearchPage CreatePhysicsSearchPage() =>
        WithPressFeedback(new PhysicsSearchPage(pageViewModelActivator, physicsSearchViewModelFactory));

    public TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null) =>
        WithPressFeedback(practiceTheoryNavigator.CreateTheoryPage(content, techniqueId));

    public FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null) =>
        WithPressFeedback(testPageFactory.CreateFindProblemPage(description, algorithm, comment, startTest, testId));

    public QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        WithPressFeedback(testPageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session));

    public StandardTestPage CreateStandardTestPage() =>
        WithPressFeedback(testPageFactory.CreateStandardTestPage());

    public AlternativeTestPage CreateAlternativeTestPage() =>
        WithPressFeedback(testPageFactory.CreateAlternativeTestPage());

    public TestResultPage CreateTestResultPage(TestResultInfo result) =>
        WithPressFeedback(testPageFactory.CreateTestResultPage(result));

    public CreatedPage CreateCreatedPage(long techniqueId) =>
        WithPressFeedback(techniquePageFactory.CreateCreatedPage(techniqueId));

    public DesignerPage CreateDesignerPage(long techniqueId) =>
        WithPressFeedback(techniquePageFactory.CreateDesignerPage(techniqueId));

    public TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId, INavigation hostNavigation) =>
        WithPressFeedback(techniquePageFactory.CreateTechniqueSessionPage(techniqueId, hostNavigation));

    private static TPage WithPressFeedback<TPage>(TPage page) where TPage : ContentPage
    {
        PressFeedbackHost.AttachToPage(page);
        return page;
    }
}
