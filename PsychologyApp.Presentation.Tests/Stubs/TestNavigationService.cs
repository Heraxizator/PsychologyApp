using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Tests;

internal sealed class TestNavigationService(INavigation navigation) : INavigationService
{
    public INavigation Navigation => navigation;

    public Task GoBackAsync() => navigation.PopAsync(false);

    public Task GoToRootAsync() => navigation.PopToRootAsync(true);

    public Task GoToTechniqueAsync(TechniqueId techniqueId) => Task.CompletedTask;

    public Task GoToCreatedAsync(long techniqueId) => Task.CompletedTask;

    public Task GoToDesignerAsync(long techniqueId) => Task.CompletedTask;

    public Task GoToUserProfileAsync() => Task.CompletedTask;

    public Task GoToOptionsAsync() => Task.CompletedTask;

    public Task GoToInfoAsync() => Task.CompletedTask;

    public Task GoToDonateAsync() => Task.CompletedTask;

    public Task GoToFormAsync() => Task.CompletedTask;

    public Task GoToSettingsAsync() => Task.CompletedTask;

    public Task GoToPhysicsSearchAsync() => Task.CompletedTask;

    public Task GoToTheoryAsync(string content) => Task.CompletedTask;

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Action action) =>
        Task.CompletedTask;

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer) =>
        Task.CompletedTask;

    public Task GoToStandardTestAsync() => Task.CompletedTask;

    public Task GoToAlternativeTestAsync() => Task.CompletedTask;
}
