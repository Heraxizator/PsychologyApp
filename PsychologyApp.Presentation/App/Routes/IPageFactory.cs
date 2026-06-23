using PsychologyApp.Presentation.Pages.TechniqueCreated;
using PsychologyApp.Presentation.Pages.TestResult;
using PsychologyApp.Presentation.Pages.AlternativeTest;
using PsychologyApp.Presentation.Pages.StandardTest;
using PsychologyApp.Presentation.Pages.Question;
using PsychologyApp.Presentation.Pages.FindProblem;
using PsychologyApp.Presentation.Pages.TechniqueTheory;
using PsychologyApp.Presentation.Pages.ProfileSettings;
using PsychologyApp.Presentation.Pages.ProfileDonate;
using PsychologyApp.Presentation.Pages.ProfileInfo;
using PsychologyApp.Presentation.Pages.ProfileOptions;
using PsychologyApp.Presentation.Pages.TestHistory;
using PsychologyApp.Presentation.Pages.StartPhysics;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.Techniques;
using PsychologyApp.Presentation.Pages.ProfileUser;
using PsychologyApp.Presentation.Pages.ReviewForm;
using PsychologyApp.Presentation.Pages.TechniqueSession;
using PsychologyApp.Presentation.Pages.TechniqueDesigner;
using PsychologyApp.Presentation.Pages.TestsList;
using PsychologyApp.Presentation.Pages.PhysicsSearch;
using PsychologyApp.Presentation.Pages.MusicPlayer;
using PsychologyApp.Presentation.Pages.QuoteFeed;

namespace PsychologyApp.Presentation.App.Routes;

public interface IPageFactory
{
    TechniquesPage CreateTechniquesPage();
    TestsListPage CreateTestsListPage();
    TestHistoryPage CreateTestHistoryPage(string testId, string testTitle);
    StartPhysicsPage CreateStartPhysicsPage();
    MusicPlayerPage CreateMusicPlayerPage();
    QuotePage CreateQuotePage();
    UserPage CreateUserPage();
    OptionsPage CreateOptionsPage();
    InfoPage CreateInfoPage();
    DonatePage CreateDonatePage(INavigation navigation);
    FormPage CreateFormPage();
    SettingsPage CreateSettingsPage();
    PhysicsSearchPage CreatePhysicsSearchPage();
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
