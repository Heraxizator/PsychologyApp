using PsychologyApp.Presentation.Views.Motivator;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Views.Practice;
using PsychologyApp.Presentation.Views.Profile;
using PsychologyApp.Presentation.Views.Review;
using PsychologyApp.Presentation.Views.Practice.Techniques;
using PsychologyApp.Presentation.Views.Practice.Constructor;
using PsychologyApp.Presentation.Views.Tests;

namespace PsychologyApp.Presentation.Services;

public interface IPageFactory
{
    TechniquesPage CreateTechniquesPage();
    TestsListPage CreateTestsListPage();
    TestHistoryPage CreateTestHistoryPage(string testId, string testTitle);
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
    FindProblemPage CreateFindProblemPage(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null);
    QuestionPage CreateQuestionPage(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null);
    StandardTestPage CreateStandardTestPage();
    AlternativeTestPage CreateAlternativeTestPage();
    TestResultPage CreateTestResultPage(TestResultInfo result);
    CreatedPage CreateCreatedPage(long techniqueId);
    DesignerPage CreateDesignerPage(long techniqueId);
    TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId);
}
