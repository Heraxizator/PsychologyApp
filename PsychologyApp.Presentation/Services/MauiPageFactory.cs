using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Views.Motivator;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Views.Practice;
using PsychologyApp.Presentation.Views.Clean;
using PsychologyApp.Presentation.Views.Physics;
using PsychologyApp.Presentation.Views.Profile;
using PsychologyApp.Presentation.Views.Review;
using PsychologyApp.Presentation.Views.Practice.Techniques;
using PsychologyApp.Presentation.Views.Practice.Constructor;
using PsychologyApp.Presentation.Views.Tests;

namespace PsychologyApp.Presentation.Services;

public sealed class MauiPageFactory(
    IProfilePageFactory profilePageFactory,
    ITestPageFactory testPageFactory,
    ITechniquePageFactory techniquePageFactory,
    IStartPhysicsViewModelFactory startPhysicsViewModelFactory,
    IMusicPlayerViewModelFactory musicPlayerViewModelFactory,
    IQuoteViewModelFactory quoteViewModelFactory,
    IPageViewModelActivator pageViewModelActivator,
    IPhysicsSearchViewModelFactory physicsSearchViewModelFactory) : IPageFactory
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
        WithPressFeedback(new MusicPlayerPage(musicPlayerViewModelFactory));

    public QuotePage CreateQuotePage() =>
        WithPressFeedback(new QuotePage(pageViewModelActivator, quoteViewModelFactory));

    public UserPage CreateUserPage() =>
        WithPressFeedback(profilePageFactory.CreateUserPage());

    public OptionsPage CreateOptionsPage() =>
        WithPressFeedback(profilePageFactory.CreateOptionsPage());

    public InfoPage CreateInfoPage() =>
        WithPressFeedback(profilePageFactory.CreateInfoPage());

    public DonatePage CreateDonatePage(INavigation navigation) =>
        WithPressFeedback(profilePageFactory.CreateDonatePage(navigation));

    public FormPage CreateFormPage() =>
        WithPressFeedback(profilePageFactory.CreateFormPage());

    public SettingsPage CreateSettingsPage() =>
        WithPressFeedback(profilePageFactory.CreateSettingsPage());

    public PhysicsSearchPage CreatePhysicsSearchPage() =>
        WithPressFeedback(new PhysicsSearchPage(pageViewModelActivator, physicsSearchViewModelFactory));

    public TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null) =>
        WithPressFeedback(testPageFactory.CreateTheoryPage(content, techniqueId));

    public FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Action action, string? testId = null) =>
        WithPressFeedback(testPageFactory.CreateFindProblemPage(description, algorithm, comment, action, testId));

    public QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        WithPressFeedback(testPageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session));

    public StandardTestPage CreateStandardTestPage() =>
        WithPressFeedback(testPageFactory.CreateStandardTestPage());

    public AlternativeTestPage CreateAlternativeTestPage() =>
        WithPressFeedback(testPageFactory.CreateAlternativeTestPage());

    public CreatedPage CreateCreatedPage(long techniqueId) =>
        WithPressFeedback(techniquePageFactory.CreateCreatedPage(techniqueId));

    public DesignerPage CreateDesignerPage(long techniqueId) =>
        WithPressFeedback(techniquePageFactory.CreateDesignerPage(techniqueId));

    public TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId) =>
        WithPressFeedback(techniquePageFactory.CreateTechniqueSessionPage(techniqueId));

    private static TPage WithPressFeedback<TPage>(TPage page) where TPage : ContentPage
    {
        PressFeedbackHost.AttachToPage(page);
        return page;
    }
}
