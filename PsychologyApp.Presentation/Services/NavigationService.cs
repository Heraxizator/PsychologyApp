using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Modules.Tests;
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
    Task GoToTheoryAsync(string content);
    Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Action action);
    Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer);
    Task GoToStandardTestAsync();
    Task GoToAlternativeTestAsync();
}

public sealed class MauiNavigationService(INavigation navigation, IPageFactory pageFactory) : INavigationService
{
    public INavigation Navigation => navigation;

    public Task GoBackAsync() =>
        navigation.PopAsync(false);

    public Task GoToRootAsync() =>
        navigation.PopToRootAsync(false);

    public Task GoToTechniqueAsync(TechniqueId techniqueId) =>
        navigation.PushAsync(pageFactory.CreateTechniqueSessionPage(techniqueId), false);

    public Task GoToCreatedAsync(long techniqueId) =>
        navigation.PushAsync(pageFactory.CreateCreatedPage(techniqueId), false);

    public Task GoToDesignerAsync(long techniqueId) =>
        navigation.PushAsync(pageFactory.CreateDesignerPage(techniqueId), false);

    public Task GoToUserProfileAsync() =>
        navigation.PushAsync(pageFactory.CreateUserPage(), false);

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

    public Task GoToTheoryAsync(string content) =>
        navigation.PushAsync(pageFactory.CreateTheoryPage(content), false);

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Action action) =>
        navigation.PushAsync(pageFactory.CreateFindProblemPage(description, algorithm, comment, action), false);

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer) =>
        navigation.PushAsync(pageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer), false);

    public Task GoToStandardTestAsync() =>
        navigation.PushAsync(pageFactory.CreateStandardTestPage(), false);

    public Task GoToAlternativeTestAsync() =>
        navigation.PushAsync(pageFactory.CreateAlternativeTestPage(), false);
}

