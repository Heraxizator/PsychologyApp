using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using AppShell = PsychologyApp.Presentation.App.AppShell;

namespace PsychologyApp.Presentation.App.Routes;

public sealed class MauiNavigationService(
    INavigation navigation,
    IPageFactory pageFactory,
    IShellStartupCoordinator shellStartupCoordinator) : INavigationService
{
    public INavigation Navigation => ResolveNavigation();

    public Task GoBackAsync() =>
        NavigationCoordinator.RunAsync(() => ResolveNavigation().PopAsync(true));

    public Task GoToRootAsync() =>
        NavigationCoordinator.RunAsync(() => ResolveNavigation().PopToRootAsync(true));

    public Task GoToTechniqueAsync(TechniqueId techniqueId) =>
        NavigationCoordinator.RunPushAsync(() =>
        {
            INavigation hostNavigation = ResolveNavigation();
            return hostNavigation.PushAsync(
                pageFactory.CreateTechniqueSessionPage(techniqueId, hostNavigation),
                true);
        });

    public Task GoToCreatedAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateCreatedPage(techniqueId), false));

    public Task GoToDesignerAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateDesignerPage(techniqueId), false));

    public Task GoToUserProfileAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateUserPage(), true));

    public Task GoToOptionsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateOptionsPage(), false));

    public Task GoToInfoAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateInfoPage(), false));

    public Task GoToDonateAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateDonatePage(ResolveNavigation()), false));

    public Task GoToFormAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateFormPage(), false));

    public Task GoToSettingsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateSettingsPage(), false));

    public Task GoToPhysicsSearchAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreatePhysicsSearchPage(), false));

    public Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateTheoryPage(content, techniqueId), false));

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateFindProblemPage(description, algorithm, comment, startTest, testId), false));

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session), true));

    public Task GoToLuscherTestAsync(LuscherMode mode) =>
        NavigationCoordinator.RunPushAsync(() => mode switch
        {
            LuscherMode.Standard => ResolveNavigation().PushAsync(pageFactory.CreateStandardTestPage(), false),
            LuscherMode.Brief => ResolveNavigation().PushAsync(pageFactory.CreateAlternativeTestPage(), false),
            _ => ResolveNavigation().PushAsync(pageFactory.CreateAlternativeTestPage(), false)
        });

    public Task GoToStandardTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Standard);

    public Task GoToAlternativeTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Brief);

    public Task GoToTestHistoryAsync(string testId, string testTitle) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateTestHistoryPage(testId, testTitle), true));

    public Task GoToTestsListAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(pageFactory.CreateTestsListPage(), true));

    public Task GoToTestResultAsync(
        int score,
        string interpretation,
        TechniqueId? recommendedTechnique = null,
        string? testId = null,
        string? interpretationDetail = null,
        string? analyzerId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(
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
                appShell.MaterializeTab(appShell.QuotesShellTab);
                appShell.CurrentItem = appShell.QuotesShellTab;
            }

            return Task.CompletedTask;
        });

    public Task ShowOnboardingAsync() =>
        NavigationCoordinator.RunAsync(() => shellStartupCoordinator.ShowOnboardingAsync(
            ResolveNavigation(),
            CompleteOnboardingWithTechniqueAsync));

    private Task CompleteOnboardingWithTechniqueAsync(TechniqueId? techniqueId)
    {
        if (techniqueId is null)
        {
            return Task.CompletedTask;
        }

        UserPreferences.SetPendingTechnique(techniqueId.Value);

        if (Microsoft.Maui.Controls.Shell.Current is AppShell appShell)
        {
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
            appShell.OpenPendingTechniqueIfNeeded();
        }

        return Task.CompletedTask;
    }

    private INavigation ResolveNavigation()
    {
        // Prefer the navigation stack bound to the page that created this service.
        // Shell.Current.CurrentPage can be AppShell on Android, which pushes to the wrong stack.
        if (navigation is not null)
        {
            return navigation;
        }

        if (Microsoft.Maui.Controls.Shell.Current?.CurrentPage?.Navigation is INavigation shellNavigation)
        {
            return shellNavigation;
        }

        Page? windowPage = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
        if (windowPage?.Navigation is INavigation windowNavigation)
        {
            return windowNavigation;
        }

        throw new InvalidOperationException("Navigation is not available.");
    }
}
