using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;

namespace PsychologyApp.Presentation.Services;

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
    Task GoToTheoryAsync(string content);
    Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Action action);
    Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer);
    Task GoToStandardTestAsync();
    Task GoToAlternativeTestAsync();
}
