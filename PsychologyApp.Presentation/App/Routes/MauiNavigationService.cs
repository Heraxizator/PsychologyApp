using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using AppShell = PsychologyApp.Presentation.App.AppShell;

namespace PsychologyApp.Presentation.App.Routes;

public sealed class MauiNavigationService : INavigationService
{
    private readonly INavigation? _navigation;
    private readonly WeakReference<ContentPage>? _ownerPage;
    private readonly IPageFactory _pageFactory;
    private readonly IShellStartupCoordinator _shellStartupCoordinator;

    public MauiNavigationService(
        NavigationContext context,
        IPageFactory pageFactory,
        IShellStartupCoordinator shellStartupCoordinator)
    {
        _navigation = context.Navigation;
        _ownerPage = context.OwnerPage is { } page ? new WeakReference<ContentPage>(page) : null;
        _pageFactory = pageFactory;
        _shellStartupCoordinator = shellStartupCoordinator;
    }

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
                _pageFactory.CreateTechniqueSessionPage(techniqueId, hostNavigation),
                true);
        });

    public Task GoToCreatedAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateCreatedPage(techniqueId), false));

    public Task GoToDesignerAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateDesignerPage(techniqueId), false));

    public Task GoToUserProfileAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateUserPage(), true));

    public Task GoToOptionsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateOptionsPage(), false));

    public Task GoToInfoAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateInfoPage(), false));

    public Task GoToDonateAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateDonatePage(), false));

    public Task GoToFormAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateFormPage(), false));

    public Task GoToSettingsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateSettingsPage(), false));

    public Task GoToPhysicsSearchAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreatePhysicsSearchPage(), false));

    public Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateTheoryPage(content, techniqueId), false));

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateFindProblemPage(description, algorithm, comment, startTest, testId), false));

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session), true));

    public Task GoToLuscherTestAsync(LuscherMode mode) =>
        NavigationCoordinator.RunPushAsync(() => mode switch
        {
            LuscherMode.Standard => ResolveNavigation().PushAsync(_pageFactory.CreateStandardTestPage(), false),
            LuscherMode.Brief => ResolveNavigation().PushAsync(_pageFactory.CreateAlternativeTestPage(), false),
            _ => ResolveNavigation().PushAsync(_pageFactory.CreateAlternativeTestPage(), false)
        });

    public Task GoToStandardTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Standard);

    public Task GoToAlternativeTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Brief);

    public Task GoToTestHistoryAsync(string testId, string testTitle) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateTestHistoryPage(testId, testTitle), true));

    public Task GoToTestsListAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateTestsListPage(), true));

    public Task<NavigationRunStatus> GoToTestResultAsync(
        int score,
        string interpretation,
        TechniqueId? recommendedTechnique = null,
        string? testId = null,
        string? interpretationDetail = null,
        string? analyzerId = null,
        QuestionnaireResultDetail? detail = null,
        CancellationToken cancellationToken = default) =>
        NavigationCoordinator.RunCompletionPushAsync(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Let the current page finish layout/animation before pushing the result page.
            await Task.Yield();

            TestResultInfo result = new()
            {
                Score = score,
                Interpretation = interpretation,
                InterpretationDetail = interpretationDetail,
                AnalyzerId = analyzerId,
                RecommendedTechnique = recommendedTechnique,
                TestId = testId,
                Detail = detail
            };

            await ResolveNavigation().PushAsync(
                _pageFactory.CreateTestResultPage(result),
                animated: false);
        });

    public Task GoToPracticeCompletionAsync(int streakDays) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(
            _pageFactory.CreatePracticeCompletionPage(streakDays),
            true));

    public Task GoToTestsTabAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.MaterializeTab(appShell.TestsTab);
                appShell.CurrentItem = appShell.TestsTab;
            }

            return Task.CompletedTask;
        });

    public Task GoToQuotesTabAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.MaterializeTab(appShell.QuotesShellTab);
                appShell.CurrentItem = appShell.QuotesShellTab;
            }

            return Task.CompletedTask;
        });

    public Task ShowOnboardingAsync() =>
        NavigationCoordinator.RunAsync(() => _shellStartupCoordinator.ShowOnboardingAsync(
            ResolveNavigation(),
            CompleteOnboardingWithTechniqueAsync));

    private Task CompleteOnboardingWithTechniqueAsync(TechniqueId? techniqueId)
    {
        if (techniqueId is null)
        {
            return Task.CompletedTask;
        }

        UserPreferences.SetPendingTechnique(techniqueId.Value);

        if (Shell.Current is AppShell appShell)
        {
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
            appShell.OpenPendingTechniqueIfNeeded();
        }

        return Task.CompletedTask;
    }

    private INavigation ResolveNavigation()
    {
        if (_ownerPage?.TryGetTarget(out ContentPage? owner) == true && owner.Navigation is INavigation ownerNavigation)
        {
            return ownerNavigation;
        }

        Page? shellPage = Shell.Current?.CurrentPage;
        if (shellPage is not null && shellPage is not Shell && shellPage.Navigation is INavigation shellNavigation)
        {
            return shellNavigation;
        }

        if (ShellNavigationResolver.TryGetActiveTabStackNavigation() is INavigation tabStackNavigation)
        {
            return tabStackNavigation;
        }

        if (_navigation is not null)
        {
            return _navigation;
        }

        Page? windowPage = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
        if (windowPage?.Navigation is INavigation windowNavigation)
        {
            return windowNavigation;
        }

        throw new InvalidOperationException("Navigation is not available.");
    }
}
