using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using AppShell = PsychologyApp.Presentation.AppShell;
using PsychologyApp.Presentation.Services.Shell;

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
    Task GoToLuscherTestAsync(LuscherMode mode);
    Task GoToStandardTestAsync();
    Task GoToAlternativeTestAsync();
    Task GoToTestHistoryAsync(string testId, string testTitle);
    Task GoToTestsListAsync();
    Task GoToTestResultAsync(int score, string interpretation, TechniqueId? recommendedTechnique = null);
    Task GoToTestsTabAsync();
    Task GoToQuotesTabAsync();
    Task ShowOnboardingAsync();
}

public sealed class MauiNavigationService(
    INavigation navigation,
    IPageFactory pageFactory,
    IShellStartupCoordinator shellStartupCoordinator) : INavigationService
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

    public Task GoToLuscherTestAsync(LuscherMode mode) => mode switch
    {
        LuscherMode.Standard => navigation.PushAsync(pageFactory.CreateStandardTestPage(), false),
        LuscherMode.Brief => navigation.PushAsync(pageFactory.CreateAlternativeTestPage(), false),
        _ => navigation.PushAsync(pageFactory.CreateAlternativeTestPage(), false)
    };

    public Task GoToStandardTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Standard);

    public Task GoToAlternativeTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Brief);

    public Task GoToTestHistoryAsync(string testId, string testTitle) =>
        navigation.PushAsync(pageFactory.CreateTestHistoryPage(testId, testTitle), true);

    public Task GoToTestsListAsync() =>
        navigation.PushAsync(pageFactory.CreateTestsListPage(), true);

    public Task GoToTestResultAsync(int score, string interpretation, TechniqueId? recommendedTechnique = null) =>
        navigation.PushAsync(
            pageFactory.CreateTestResultPage(new TestResultInfo
            {
                Score = score,
                Interpretation = interpretation,
                RecommendedTechnique = recommendedTechnique
            }),
            true);

    public Task GoToTestsTabAsync()
    {
        if (Microsoft.Maui.Controls.Shell.Current is AppShell appShell)
        {
            appShell.CurrentItem = appShell.TestsTab;
        }

        return Task.CompletedTask;
    }

    public Task GoToQuotesTabAsync()
    {
        if (Microsoft.Maui.Controls.Shell.Current is AppShell appShell)
        {
            appShell.CurrentItem = appShell.QuotesTab;
        }

        return Task.CompletedTask;
    }

    public Task ShowOnboardingAsync() =>
        shellStartupCoordinator.ShowOnboardingAsync(
            navigation,
            techniqueId =>
            {
                if (techniqueId is null)
                {
                    return Task.CompletedTask;
                }

                return GoToTechniqueAsync(techniqueId.Value);
            });
}

