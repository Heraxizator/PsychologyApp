using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Modules.Tests.Collection;
using PsychologyApp.Presentation.Views.TechniquePages;
using PsychologyApp.Presentation.Views.TechniquePages.ConstructorPages;

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
    Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null);
    Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Action action, string? testId = null);
    Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null);
    Task GoToStandardTestAsync();
    Task GoToAlternativeTestAsync();
}

public sealed class MauiNavigationService(INavigation navigation, IPageFactory pageFactory) : INavigationService
{
    public INavigation Navigation => navigation;

    public Task GoBackAsync() =>
        navigation.PopAsync(true);

    public Task GoToRootAsync() =>
        navigation.PopToRootAsync(true);

    public Task GoToTechniqueAsync(TechniqueId techniqueId) =>
        navigation.PushAsync(pageFactory.CreateTechniqueSessionPage(techniqueId), true);

    public Task GoToCreatedAsync(long techniqueId) =>
        navigation.PushAsync(pageFactory.CreateCreatedPage(techniqueId), false);

    public Task GoToDesignerAsync(long techniqueId) =>
        navigation.PushAsync(pageFactory.CreateDesignerPage(techniqueId), false);

    public Task GoToUserProfileAsync() =>
        navigation.PushAsync(pageFactory.CreateUserPage(), true);

    public Task GoToOptionsAsync() =>
        navigation.PushAsync(pageFactory.CreateOptionsPage(), false);

    public Task GoToInfoAsync() =>
        navigation.PushAsync(pageFactory.CreateInfoPage(), false);

    public Task GoToDonateAsync() =>
        navigation.PushAsync(pageFactory.CreateDonatePage(navigation), false);

    public Task GoToFormAsync() =>
        navigation.PushAsync(pageFactory.CreateFormPage(), false);

    public Task GoToSettingsAsync() =>
        navigation.PushAsync(pageFactory.CreateSettingsPage(), false);

    public Task GoToPhysicsSearchAsync() =>
        navigation.PushAsync(pageFactory.CreatePhysicsSearchPage(), false);

    public Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null) =>
        navigation.PushAsync(pageFactory.CreateTheoryPage(content, techniqueId), false);

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Action action, string? testId = null) =>
        navigation.PushAsync(pageFactory.CreateFindProblemPage(description, algorithm, comment, action, testId), false);

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        navigation.PushAsync(pageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session), true);

    public Task GoToStandardTestAsync() =>
        navigation.PushAsync(pageFactory.CreateStandardTestPage(), false);

    public Task GoToAlternativeTestAsync() =>
        navigation.PushAsync(pageFactory.CreateAlternativeTestPage(), false);
}

