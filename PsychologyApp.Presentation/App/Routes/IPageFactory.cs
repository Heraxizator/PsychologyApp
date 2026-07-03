using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueCreated;
using PsychologyApp.Presentation.Pages.RunTests.TestResult;
using PsychologyApp.Presentation.Pages.RunTests.AlternativeTest;
using PsychologyApp.Presentation.Pages.RunTests.StandardTest;
using PsychologyApp.Presentation.Pages.RunTests.Question;
using PsychologyApp.Presentation.Pages.RunTests.FindProblem;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;
using PsychologyApp.Presentation.Pages.RunTests.TestHistory;
using PsychologyApp.Presentation.Pages.SearchPhysics.StartPhysics;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;
using PsychologyApp.Presentation.Pages.SendReviewForm.ReviewForm;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;
using PsychologyApp.Presentation.Pages.RunTests.TestsList;
using PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;
using PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;
using PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

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
    DonatePage CreateDonatePage();
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
    TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId, INavigation hostNavigation);
    PracticeCompletionPage CreatePracticeCompletionPage(int streakDays);
}
