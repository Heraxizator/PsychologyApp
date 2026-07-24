using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;

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
    Task GoToPracticeHistoryAsync();
    Task GoToJournalAsync();
    Task GoToJournalOverviewAsync();
    Task GoToJournalTimelineAsync();
    Task GoToOptionsAsync();
    Task GoToInfoAsync();
    Task GoToDonateAsync();
    Task GoToAliceAsync();
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
    Task<NavigationRunStatus> GoToTestResultAsync(
        int score,
        string interpretation,
        TechniqueId? recommendedTechnique = null,
        string? testId = null,
        string? interpretationDetail = null,
        string? analyzerId = null,
        QuestionnaireResultDetail? detail = null,
        CancellationToken cancellationToken = default);
    Task GoToTestsTabAsync();
    Task GoToPracticeTabAsync();
    Task GoToQuotesTabAsync();
    Task GoToQuotesFavoritesAsync();
    Task GoToPracticeCompletionAsync(int streakDays, string? completedItemKey = null, long? sessionResultId = null);
    Task ShowOnboardingAsync();
    Task GoToCrisisHubAsync();
    Task GoToRiskCheckAsync(string source);
}
