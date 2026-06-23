using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.App.Routes;

public interface INavigationService
{
    INavigation Navigation { get; }
    Task GoBackAsync();
    Task GoToRootAsync();
    Task GoToTechniqueAsync(TechniqueId techniqueId);
    Task GoToCreatedAsync(long techniqueId);
    Task GoToDesignerAsync(long techniqueId);
    Task GoToUserProfileAsync();
    Task GoToOptionsAsync();
    Task GoToInfoAsync();
    Task GoToDonateAsync();
    Task GoToFormAsync();
    Task GoToSettingsAsync();
    Task GoToPhysicsSearchAsync();
    Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null);
    Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null);
    Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null);
    Task GoToLuscherTestAsync(LuscherMode mode);
    Task GoToStandardTestAsync();
    Task GoToAlternativeTestAsync();
    Task GoToTestHistoryAsync(string testId, string testTitle);
    Task GoToTestsListAsync();
    Task GoToTestResultAsync(
        int score,
        string interpretation,
        TechniqueId? recommendedTechnique = null,
        string? testId = null,
        string? interpretationDetail = null,
        string? analyzerId = null);
    Task GoToTestsTabAsync();
    Task GoToQuotesTabAsync();
    Task ShowOnboardingAsync();
}
