using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.RunTests.AlternativeTest;
using PsychologyApp.Presentation.Pages.RunTests.FindProblem;
using PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;
using PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;
using PsychologyApp.Presentation.Pages.RunTests.Question;
using PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;
using PsychologyApp.Presentation.Features.SendReviewForm;
using PsychologyApp.Presentation.Pages.SendReviewForm.ReviewForm;
using PsychologyApp.Presentation.Pages.RunTests.StandardTest;
using PsychologyApp.Presentation.Pages.SearchPhysics.StartPhysics;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueCreated;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;
using PsychologyApp.Presentation.Pages.RunTests.TestHistory;
using PsychologyApp.Presentation.Pages.RunTests.TestResult;
using PsychologyApp.Presentation.Pages.RunTests.TestsList;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageQuotes.DependencyInjection;
using PsychologyApp.Presentation.Features.PlayMusic.DependencyInjection;
using PsychologyApp.Presentation.Features.SearchPhysics.DependencyInjection;
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

    public PracticeCompletionPage CreatePracticeCompletionPage(int streakDays) =>
        WithPressFeedback(techniquePageFactory.CreatePracticeCompletionPage(streakDays));

    private static TPage WithPressFeedback<TPage>(TPage page) where TPage : ContentPage
    {
        PressFeedbackHost.AttachToPage(page);
        return page;
    }
}
