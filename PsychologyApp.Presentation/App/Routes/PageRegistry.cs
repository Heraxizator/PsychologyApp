using PsychologyApp.Presentation.Shared.Common.Diagnostics;
using PsychologyApp.Presentation.Features.Chat;
using PsychologyApp.Presentation.Pages.Chat.ChatList;
using PsychologyApp.Presentation.Pages.Chat.Companion;
using PsychologyApp.Presentation.Pages.Chat.Conversation;
using PsychologyApp.Presentation.Features.ClinicalCare;
using PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;
using PsychologyApp.Presentation.Pages.ClinicalCare.RiskCheck;
using PsychologyApp.Presentation.Pages.ClinicalCare.SafetyPlan;
using PsychologyApp.Presentation.Pages.ClinicalCare.Exposure;
using PsychologyApp.Presentation.Pages.ClinicalCare.BodyCheck;
using PsychologyApp.Presentation.Pages.ReadArticles.AnchorThoughts;
using PsychologyApp.Presentation.Pages.ReadArticles.Articles;
using PsychologyApp.Presentation.Pages.ReadArticles.ArticleReader;
using PsychologyApp.Presentation.Pages.ReadArticles.Courses;
using PsychologyApp.Presentation.Pages.ReadArticles.ForYou;
using PsychologyApp.Presentation.Features.ReadArticles;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.RunTests.AlternativeTest;
using PsychologyApp.Presentation.Pages.RunTests.FindProblem;
using PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileAlice;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfilePracticeHistory;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDataBackup;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;
using PsychologyApp.Presentation.Pages.ManageJournal.Journal;
using PsychologyApp.Presentation.Pages.ManageJournal.JournalOverview;
using PsychologyApp.Presentation.Pages.ManageJournal.JournalTimeline;
using PsychologyApp.Presentation.Pages.RunTests.Question;
using PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;
using PsychologyApp.Presentation.Features.SendReviewForm;
using PsychologyApp.Presentation.Pages.SendReviewForm.ReviewForm;
using PsychologyApp.Presentation.Pages.RunTests.StandardTest;
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
using PsychologyApp.Presentation.Shared.Services.Toasts;

namespace PsychologyApp.Presentation.App.Routes;

/// <summary>
/// App-layer composition root for full-screen pages (canonical FSD routes).
/// </summary>
public sealed class PageRegistry(
    IProfilePageFactory profilePageFactory,
    IJournalPageFactory journalPageFactory,
    IReviewPageFactory reviewPageFactory,
    ITestPageFactory testPageFactory,
    ITechniquePageFactory techniquePageFactory,
    IClinicalCarePageFactory clinicalCarePageFactory,
    IReadArticlesPageFactory readArticlesPageFactory,
    IChatPageFactory chatPageFactory,
    IPracticeTheoryNavigator practiceTheoryNavigator,
    IMusicPlayerViewModelFactory musicPlayerViewModelFactory,
    IQuoteViewModelFactory quoteViewModelFactory,
    IPageViewModelActivator pageViewModelActivator,
    IToastService toastService) : IPageFactory
{
    public TechniquesPage CreateTechniquesPage() =>
        WithPressFeedback(techniquePageFactory.CreateTechniquesPage());

    public TestsListPage CreateTestsListPage() =>
        WithPressFeedback(testPageFactory.CreateTestsListPage());

    public TestHistoryPage CreateTestHistoryPage(string testId, string testTitle) =>
        WithPressFeedback(testPageFactory.CreateTestHistoryPage(testId, testTitle));

    public MusicPlayerPage CreateMusicPlayerPage() =>
        WithPressFeedback(new MusicPlayerPage(musicPlayerViewModelFactory, toastService));

    public QuotePage CreateQuotePage() =>
        WithPressFeedback(new QuotePage(pageViewModelActivator, quoteViewModelFactory));

    public UserPage CreateUserPage() =>
        WithPressFeedback(profilePageFactory.CreateUserPage());

    public PracticeHistoryPage CreatePracticeHistoryPage() =>
        WithPressFeedback(profilePageFactory.CreatePracticeHistoryPage());

    public PsychologyApp.Presentation.Pages.ManageProfile.ProfileInsights.InsightsPage CreateInsightsPage() =>
        WithPressFeedback(profilePageFactory.CreateInsightsPage());

    public JournalPage CreateJournalPage() =>
        WithPressFeedback(journalPageFactory.CreateJournalPage());

    public JournalOverviewPage CreateJournalOverviewPage() =>
        WithPressFeedback(journalPageFactory.CreateJournalOverviewPage());

    public JournalTimelinePage CreateJournalTimelinePage() =>
        WithPressFeedback(journalPageFactory.CreateJournalTimelinePage());

    public OptionsPage CreateOptionsPage() =>
        WithPressFeedback(profilePageFactory.CreateOptionsPage());

    public InfoPage CreateInfoPage() =>
        WithPressFeedback(profilePageFactory.CreateInfoPage());

    public DonatePage CreateDonatePage() =>
        WithPressFeedback(profilePageFactory.CreateDonatePage());

    public AlicePage CreateAlicePage() =>
        WithPressFeedback(profilePageFactory.CreateAlicePage());

    public FormPage CreateFormPage() =>
        WithPressFeedback(reviewPageFactory.CreateFormPage());

    public SettingsPage CreateSettingsPage() =>
        profilePageFactory.CreateSettingsPage();

    public DataBackupPage CreateDataBackupPage() =>
        WithPressFeedback(profilePageFactory.CreateDataBackupPage());

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

    public ContentPage CreateTechniquePage(TechniqueId techniqueId, INavigation hostNavigation) =>
        techniquePageFactory.HasDialogue(techniqueId)
            ? WithPressFeedback(techniquePageFactory.CreateTechniqueDialoguePage(techniqueId, hostNavigation))
            : CreateTechniqueSessionPage(techniqueId, hostNavigation);

    public PracticeCompletionPage CreatePracticeCompletionPage(int activeDays, string? completedItemKey = null, long? sessionResultId = null) =>
        WithPressFeedback(techniquePageFactory.CreatePracticeCompletionPage(activeDays, completedItemKey, sessionResultId));

    public ChatListPage CreateChatListPage(INavigation? hostNavigation) =>
        WithPressFeedback(chatPageFactory.CreateChatListPage(hostNavigation));

    public ChatPage CreateChatPage(long? sessionId, INavigation hostNavigation, string? topic = null, string? opener = null) =>
        WithPressFeedback(chatPageFactory.CreateChatPage(sessionId, hostNavigation, topic, opener));

    public CompanionProfilePage CreateCompanionProfilePage(INavigation hostNavigation) =>
        WithPressFeedback(chatPageFactory.CreateCompanionProfilePage(hostNavigation));

    public PsychologyApp.Presentation.Pages.Chat.ChatInfo.ChatInfoPage CreateChatInfoPage(long sessionId, INavigation hostNavigation) =>
        WithPressFeedback(chatPageFactory.CreateChatInfoPage(sessionId, hostNavigation));

    public CrisisHubPage CreateCrisisHubPage() =>
        WithPressFeedback(clinicalCarePageFactory.CreateCrisisHubPage());

    public RiskCheckPage CreateRiskCheckPage(string source) =>
        WithPressFeedback(clinicalCarePageFactory.CreateRiskCheckPage(source));

    public SafetyPlanPage CreateSafetyPlanPage() =>
        WithPressFeedback(clinicalCarePageFactory.CreateSafetyPlanPage());

    public ExposurePage CreateExposurePage() =>
        WithPressFeedback(clinicalCarePageFactory.CreateExposurePage());

    public BodyCheckPage CreateBodyCheckPage() =>
        WithPressFeedback(clinicalCarePageFactory.CreateBodyCheckPage());

    public ArticlesPage CreateArticlesPage() =>
        WithPressFeedback(readArticlesPageFactory.CreateArticlesPage());

    public ForYouPage CreateForYouPage() =>
        WithPressFeedback(readArticlesPageFactory.CreateForYouPage());

    public CoursesPage CreateCoursesPage() =>
        WithPressFeedback(readArticlesPageFactory.CreateCoursesPage());

    public ArticleReaderPage CreateArticleReaderPage(string articleId) =>
        WithPressFeedback(readArticlesPageFactory.CreateArticleReaderPage(articleId));

    public CoursePage CreateCoursePage(string courseId) =>
        WithPressFeedback(readArticlesPageFactory.CreateCoursePage(courseId));

    public CourseDayPage CreateCourseDayPage(string courseId, int day) =>
        WithPressFeedback(readArticlesPageFactory.CreateCourseDayPage(courseId, day));

    public AnchorThoughtsPage CreateAnchorThoughtsPage() =>
        WithPressFeedback(readArticlesPageFactory.CreateAnchorThoughtsPage());

    private static TPage WithPressFeedback<TPage>(TPage page) where TPage : ContentPage
    {
        PerfTrace.Mark($"Created {typeof(TPage).Name}");
        PressFeedbackHost.AttachToPage(page);
        return page;
    }
}
