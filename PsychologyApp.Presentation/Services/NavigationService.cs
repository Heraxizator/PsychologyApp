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

public sealed class MauiNavigationService(
    INavigation navigation,
    IPageFactory pageFactory,
    IShellStartupCoordinator shellStartupCoordinator) : INavigationService
{
    public INavigation Navigation => navigation;

    public Task GoBackAsync() =>
        NavigationCoordinator.RunAsync(() => navigation.PopAsync(true));

    public Task GoToRootAsync() =>
        NavigationCoordinator.RunAsync(() => navigation.PopToRootAsync(true));

    public Task GoToTechniqueAsync(TechniqueId techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateTechniqueSessionPage(techniqueId), true));

    public Task GoToCreatedAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateCreatedPage(techniqueId), false));

    public Task GoToDesignerAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateDesignerPage(techniqueId), false));

    public Task GoToUserProfileAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateUserPage(), true));

    public Task GoToOptionsAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateOptionsPage(), false));

    public Task GoToInfoAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateInfoPage(), false));

    public Task GoToDonateAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateDonatePage(navigation), false));

    public Task GoToFormAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateFormPage(), false));

    public Task GoToSettingsAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateSettingsPage(), false));

    public Task GoToPhysicsSearchAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreatePhysicsSearchPage(), false));

    public Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateTheoryPage(content, techniqueId), false));

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateFindProblemPage(description, algorithm, comment, startTest, testId), false));

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session), true));

    public Task GoToLuscherTestAsync(LuscherMode mode) =>
        NavigationCoordinator.RunPushAsync(() => mode switch
        {
            LuscherMode.Standard => navigation.PushAsync(pageFactory.CreateStandardTestPage(), false),
            LuscherMode.Brief => navigation.PushAsync(pageFactory.CreateAlternativeTestPage(), false),
            _ => navigation.PushAsync(pageFactory.CreateAlternativeTestPage(), false)
        });

    public Task GoToStandardTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Standard);

    public Task GoToAlternativeTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Brief);

    public Task GoToTestHistoryAsync(string testId, string testTitle) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateTestHistoryPage(testId, testTitle), true));

    public Task GoToTestsListAsync() =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(pageFactory.CreateTestsListPage(), true));

    public Task GoToTestResultAsync(
        int score,
        string interpretation,
        TechniqueId? recommendedTechnique = null,
        string? testId = null,
        string? interpretationDetail = null,
        string? analyzerId = null) =>
        NavigationCoordinator.RunPushAsync(() => navigation.PushAsync(
            pageFactory.CreateTestResultPage(new TestResultInfo
            {
                Score = score,
                Interpretation = interpretation,
                InterpretationDetail = interpretationDetail,
                AnalyzerId = analyzerId,
                RecommendedTechnique = recommendedTechnique,
                TestId = testId
            }),
            true));

    public Task GoToTestsTabAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Microsoft.Maui.Controls.Shell.Current is AppShell appShell)
            {
                appShell.MaterializeTab(appShell.TestsTab);
                appShell.CurrentItem = appShell.TestsTab;
            }

            return Task.CompletedTask;
        });

    public Task GoToQuotesTabAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Microsoft.Maui.Controls.Shell.Current is AppShell appShell)
            {
                appShell.MaterializeTab(appShell.QuotesTab);
                appShell.CurrentItem = appShell.QuotesTab;
            }

            return Task.CompletedTask;
        });

    public Task ShowOnboardingAsync() =>
        NavigationCoordinator.RunAsync(() => shellStartupCoordinator.ShowOnboardingAsync(
            navigation,
            techniqueId =>
            {
                if (techniqueId is null)
                {
                    return Task.CompletedTask;
                }

                return GoToTechniqueAsync(techniqueId.Value);
            }));
}

