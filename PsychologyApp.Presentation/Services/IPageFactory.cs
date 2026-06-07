using PsychologyApp.Presentation.Views.Motivator;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Profile;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Views;
using PsychologyApp.Presentation.Views.About;
using PsychologyApp.Presentation.Views.Profile;
using PsychologyApp.Presentation.Views.Review;
using PsychologyApp.Presentation.Views.Settings;
using PsychologyApp.Presentation.Views.TechniquePages;
using PsychologyApp.Presentation.Views.TechniquePages.ConstructorPages;
using PsychologyApp.Presentation.Views.Tests;

namespace PsychologyApp.Presentation.Services;

public interface IPageFactory
{
    TechniquesPage CreateTechniquesPage();
    TestsListPage CreateTestsListPage();
    Views.Physics.StartPhysicsPage CreateStartPhysicsPage();
    Views.Clean.MusicPlayerPage CreateMusicPlayerPage();
    QuotePage CreateQuotePage();
    UserPage CreateUserPage();
    OptionsPage CreateOptionsPage();
    InfoPage CreateInfoPage();
    DonatePage CreateDonatePage(INavigation navigation);
    FormPage CreateFormPage();
    SettingsPage CreateSettingsPage();
    Views.Physics.PhysicsSearchPage CreatePhysicsSearchPage();
    TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null);
    FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Action action, string? testId = null);
    QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, Modules.Tests.Collection.TestSessionInfo? session = null);
    StandardTestPage CreateStandardTestPage();
    AlternativeTestPage CreateAlternativeTestPage();
    CreatedPage CreateCreatedPage(long techniqueId);
    DesignerPage CreateDesignerPage(long techniqueId);
    TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId);
}
