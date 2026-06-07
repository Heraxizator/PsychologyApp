using PsychologyApp.Presentation.Views.Motivator;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Profile;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Modules.Tests.Collection;
using PsychologyApp.Presentation.Views;
using PsychologyApp.Presentation.Views.About;
using PsychologyApp.Presentation.Views.Clean;
using PsychologyApp.Presentation.Views.Physics;
using PsychologyApp.Presentation.Views.Profile;
using PsychologyApp.Presentation.Views.Review;
using PsychologyApp.Presentation.Views.Settings;
using PsychologyApp.Presentation.Views.TechniquePages;
using PsychologyApp.Presentation.Views.TechniquePages.ConstructorPages;
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
        techniquePageFactory.CreateTechniquesPage();

    public TestsListPage CreateTestsListPage() =>
        testPageFactory.CreateTestsListPage();

    public StartPhysicsPage CreateStartPhysicsPage() =>
        new(pageViewModelActivator, startPhysicsViewModelFactory);

    public MusicPlayerPage CreateMusicPlayerPage() =>
        new(musicPlayerViewModelFactory);

    public QuotePage CreateQuotePage() =>
        new(pageViewModelActivator, quoteViewModelFactory);

    public UserPage CreateUserPage() =>
        profilePageFactory.CreateUserPage();

    public OptionsPage CreateOptionsPage() =>
        profilePageFactory.CreateOptionsPage();

    public InfoPage CreateInfoPage() =>
        profilePageFactory.CreateInfoPage();

    public DonatePage CreateDonatePage(INavigation navigation) =>
        profilePageFactory.CreateDonatePage(navigation);

    public FormPage CreateFormPage() =>
        profilePageFactory.CreateFormPage();

    public SettingsPage CreateSettingsPage() =>
        profilePageFactory.CreateSettingsPage();

    public PhysicsSearchPage CreatePhysicsSearchPage() =>
        new(pageViewModelActivator, physicsSearchViewModelFactory);

    public TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null) =>
        testPageFactory.CreateTheoryPage(content, techniqueId);

    public FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Action action, string? testId = null) =>
        testPageFactory.CreateFindProblemPage(description, algorithm, comment, action, testId);

    public QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, Modules.Tests.Collection.TestSessionInfo? session = null) =>
        testPageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session);

    public StandardTestPage CreateStandardTestPage() =>
        testPageFactory.CreateStandardTestPage();

    public AlternativeTestPage CreateAlternativeTestPage() =>
        testPageFactory.CreateAlternativeTestPage();

    public CreatedPage CreateCreatedPage(long techniqueId) =>
        techniquePageFactory.CreateCreatedPage(techniqueId);

    public DesignerPage CreateDesignerPage(long techniqueId) =>
        techniquePageFactory.CreateDesignerPage(techniqueId);

    public TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId) =>
        techniquePageFactory.CreateTechniqueSessionPage(techniqueId);
}
