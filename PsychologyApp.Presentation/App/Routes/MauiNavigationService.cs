using PsychologyApp.Presentation.Shared.Services.Preferences;
﻿using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Features.RunTests;
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
    private readonly JournalScreenCoordinator _journalScreenCoordinator;
    private readonly IUserPreferencesStore _preferences;
    private readonly TestOpener _testOpener;

    public MauiNavigationService(
        NavigationContext context,
        IPageFactory pageFactory,
        IShellStartupCoordinator shellStartupCoordinator,
        JournalScreenCoordinator journalScreenCoordinator,
        IUserPreferencesStore preferences,
        TestOpener testOpener)
    {
        _testOpener = testOpener;
        _preferences = preferences;
        _navigation = context.Navigation;
        _ownerPage = context.OwnerPage is { } page ? new WeakReference<ContentPage>(page) : null;
        _pageFactory = pageFactory;
        _shellStartupCoordinator = shellStartupCoordinator;
        _journalScreenCoordinator = journalScreenCoordinator;
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
                PrepareSecondaryPage(_pageFactory.CreateTechniquePage(techniqueId, hostNavigation)),
                true);
        });

    public Task GoToCreatedAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateCreatedPage(techniqueId)), false));

    public Task GoToDesignerAsync(long techniqueId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateDesignerPage(techniqueId)), false));

    public Task GoToUserProfileAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateUserPage()), true));

    public Task GoToPracticeHistoryAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreatePracticeHistoryPage()), true));

    public Task GoToJournalAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.ShowTab(appShell.JournalShellTab);
            }

            return Task.CompletedTask;
        });

    public Task GoToJournalDayAsync(DateOnly day) =>
        _journalScreenCoordinator.OpenEditorDayAsync(day, this);

    public Task GoToJournalOverviewAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateJournalOverviewPage()), true));

    public Task GoToJournalTimelineAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateJournalTimelinePage()), true));

    public Task GoToOptionsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateOptionsPage()), true));

    public Task GoToInfoAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateInfoPage()), true));

    public Task GoToDonateAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateDonatePage()), true));

    public Task GoToAliceAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateAlicePage()), true));

    public Task GoToFormAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateFormPage()), true));

    public Task GoToSettingsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateSettingsPage()), true));

    public Task GoToTheoryAsync(string content, TechniqueId? techniqueId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateTheoryPage(content, techniqueId)), false));

    public Task GoToFindProblemAsync(string? description, List<string> algorithm, string? comment, Func<Task> startTest, string? testId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateFindProblemPage(description, algorithm, comment, startTest, testId)), false));

    public Task GoToQuestionPageAsync(List<Question> questions, Func<int, string> scoreAnalyzer, bool singleAnswer, TestSessionInfo? session = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateQuestionPage(questions, scoreAnalyzer, singleAnswer, session)), true));

    public Task GoToLuscherTestAsync(LuscherMode mode) =>
        NavigationCoordinator.RunPushAsync(() => mode switch
        {
            LuscherMode.Standard => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateStandardTestPage()), false),
            LuscherMode.Brief => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateAlternativeTestPage()), false),
            _ => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateAlternativeTestPage()), false)
        });

    public Task GoToStandardTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Standard);

    public Task GoToAlternativeTestAsync() =>
        GoToLuscherTestAsync(LuscherMode.Brief);

    public Task GoToTestHistoryAsync(string testId, string testTitle) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateTestHistoryPage(testId, testTitle)), true));

    public Task GoToTestsListAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateTestsListPage()), true));

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
                Detail = detail,
                CompletedAtUtc = DateTime.UtcNow
            };

            await ResolveNavigation().PushAsync(
                PrepareSecondaryPage(_pageFactory.CreateTestResultPage(result)),
                animated: false);
        });

    public Task GoToPracticeCompletionAsync(int streakDays, string? completedItemKey = null, long? sessionResultId = null) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(
            PrepareSecondaryPage(_pageFactory.CreatePracticeCompletionPage(streakDays, completedItemKey, sessionResultId)),
            true));

    public Task GoToTestsTabAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.ShowTab(appShell.TestsTab);
            }

            return Task.CompletedTask;
        });

    public Task GoToPracticeTabAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.ShowTab(appShell.PracticeShellTab);
            }

            return Task.CompletedTask;
        });

    // The menu's sections open over the current tab and keep the tab bar: they are places of their own, not steps of a task.
    public Task GoToQuotesAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateQuotePage(), true));

    public Task GoToPrayersAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateMusicPlayerPage(), true));

    public Task GoToForYouAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateForYouPage(), true));

    public Task GoToCoursesAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateCoursesPage(), true));

    public Task GoToArticlesAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateArticlesPage(), true));

    public Task GoToBodyAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateBodyCheckPage(), true));

    public Task GoToInsightsAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(_pageFactory.CreateInsightsPage(), true));

    public Task GoToQuotesFavoritesAsync()
    {
        _preferences.SetPendingQuoteFeed("favorites");
        return GoToQuotesAsync();
    }

    public Task ShowOnboardingAsync() =>
        NavigationCoordinator.RunAsync(() => _shellStartupCoordinator.ShowOnboardingAsync(
            ResolveNavigation(),
            CompleteOnboardingWithTechniqueAsync));


    public Task GoToChatListAsync() =>
        NavigationCoordinator.RunAsync(() =>
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.ShowTab(appShell.ChatShellTab);
            }

            return Task.CompletedTask;
        });

    // Every way into a chat (the tab, articles, reminders) passes here: in a language the chat cannot read for crisis words
    // the chat list opens instead, and says why.
    public Task GoToChatAsync(long? sessionId = null, string? topic = null, string? opener = null) =>
        !AppLanguages.HasChat(AppStrings.Language) ? GoToChatListAsync() : NavigationCoordinator.RunPushAsync(() =>
        {
            INavigation hostNavigation = ResolveNavigation();
            return hostNavigation.PushAsync(PrepareSecondaryPage(_pageFactory.CreateChatPage(sessionId, hostNavigation, topic, opener)), true);
        });

    public Task GoToCompanionProfileAsync() =>
        NavigationCoordinator.RunPushAsync(() =>
        {
            INavigation hostNavigation = ResolveNavigation();
            return hostNavigation.PushAsync(PrepareSecondaryPage(_pageFactory.CreateCompanionProfilePage(hostNavigation)), true);
        });

    public Task GoToCrisisHubAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateCrisisHubPage()), true));

    public Task GoToRiskCheckAsync(string source) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateRiskCheckPage(source)), true));

    public Task GoToSafetyPlanAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateSafetyPlanPage()), true));

    public Task GoToExposureAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateExposurePage()), true));

    public Task GoToArticleAsync(string articleId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateArticleReaderPage(articleId)), true));

    public Task GoToTestAsync(string testId) => _testOpener.OpenAsync(testId, this);

    public Task GoToCourseAsync(string courseId) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateCoursePage(courseId)), true));

    public Task GoToCourseDayAsync(string courseId, int day) =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateCourseDayPage(courseId, day)), true));

    public Task GoToDataBackupAsync() =>
        NavigationCoordinator.RunPushAsync(() => ResolveNavigation().PushAsync(PrepareSecondaryPage(_pageFactory.CreateDataBackupPage()), true));

    /// <summary>
    /// A step inside a tab (a practice, a test, a chat, a form): no tab bar and no burger menu. A menu tap closes whatever is open over
    /// the tab, so the menu must not be reachable (also by an edge swipe) from a page with unsaved answers.
    /// </summary>
    internal static ContentPage PrepareSecondaryPage(ContentPage page)
    {
        Shell.SetTabBarIsVisible(page, false);
        Shell.SetFlyoutBehavior(page, FlyoutBehavior.Disabled);
        return page;
    }

    private Task CompleteOnboardingWithTechniqueAsync(TechniqueId? techniqueId)
    {
        if (techniqueId is null)
        {
            return Task.CompletedTask;
        }

        _preferences.SetPendingTechnique(techniqueId.Value);

        if (Shell.Current is AppShell appShell)
        {
            appShell.ShowTab(appShell.PracticeShellTab);
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
