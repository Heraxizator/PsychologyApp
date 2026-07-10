using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.Services.Clipboard;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuoteFeedCoordinatorLoadTests
{
    public QuoteFeedCoordinatorLoadTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task LoadItemsAsync_SeedsMapsAndDedupesQuotes()
    {
        Mock<IQuotService> quotService = new();
        quotService
            .Setup(s => s.TryLoadSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        quotService
            .Setup(s => s.GetUnreadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO { QuotId = 1, Text = "Alpha", Title = "A", Theme = "wisdom", IsFavourite = false, IsReaded = false },
                new QuotDTO { QuotId = 2, Text = "Alpha", Title = "B", Theme = "wisdom", IsFavourite = false, IsReaded = false },
                new QuotDTO { QuotId = 3, Text = "Beta", Title = "C", Theme = "calm", IsFavourite = false, IsReaded = false }
            ]);
        quotService
            .Setup(s => s.IsAllCaughtUpAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        QuoteFeedCoordinator coordinator = new();
        QuoteItemCommandsFactory factory = CreateQuoteItemFactory(quotService.Object);
        bool failed = false;

        QuoteFeedLoadResult result = await coordinator.LoadItemsAsync(
            quotService.Object,
            factory,
            count: 20,
            resetKnown: true,
            seedNewQuote: true,
            dailyQuoteText: null,
            _ => Task.CompletedTask,
            () => failed = true,
            CancellationToken.None);

        quotService.Verify(s => s.TryLoadSingleAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.False(failed);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Alpha", result.Items[0].Text);
        Assert.Equal("Beta", result.Items[1].Text);
    }

    [Fact]
    public async Task LoadItemsAsync_FreshCoordinators_DoNotShareKnownQuotes()
    {
        Mock<IQuotService> quotService = new();
        quotService
            .Setup(s => s.GetUnreadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO { QuotId = 1, Text = "Shared", Title = "A", Theme = "wisdom", IsFavourite = false, IsReaded = false }
            ]);
        quotService
            .Setup(s => s.IsAllCaughtUpAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        QuoteFeedCoordinator firstCoordinator = new();
        QuoteFeedCoordinator secondCoordinator = new();
        QuoteItemCommandsFactory factory = CreateQuoteItemFactory(quotService.Object);

        await firstCoordinator.LoadItemsAsync(
            quotService.Object,
            factory,
            count: 20,
            resetKnown: true,
            seedNewQuote: false,
            dailyQuoteText: null,
            _ => Task.CompletedTask,
            () => { },
            CancellationToken.None);

        QuoteFeedLoadResult secondLoad = await secondCoordinator.LoadItemsAsync(
            quotService.Object,
            factory,
            count: 20,
            resetKnown: true,
            seedNewQuote: false,
            dailyQuoteText: null,
            _ => Task.CompletedTask,
            () => { },
            CancellationToken.None);

        Assert.Single(secondLoad.Items);
    }

    private static QuoteItemCommandsFactory CreateQuoteItemFactory(IQuotService quotService) =>
        new(
            quotService,
            new QuotesChangeNotifier(),
            Mock.Of<IAppClipboardService>(),
            Mock.Of<IToastService>(),
            Options.Create(new AppSettings()),
            NullLogger<QuoteItemCommandsFactory>.Instance);
}

public sealed class TechniquesListInitializerTests
{
    public TechniquesListInitializerTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task LoadAsync_ReturnsGroupedLayoutWithDashboardSnapshot()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(7);
        progress.Setup(p => p.GetAtRiskStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(DateTime.UtcNow);
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        progress
            .Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal));
        progress
            .Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));

        Mock<ITechniqueService> techniqueService = new();
        techniqueService
            .Setup(s => s.GetTechniquesPageAsync(0, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new TechniqueDTO
                {
                    TechniqueId = 42,
                    Header = "Custom",
                    Description = "Desc",
                    Subject = "Theme",
                    Author = "Me",
                    Date = "Today",
                    Image = string.Empty
                }
            ]);

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState());

        PracticeDashboardLoader dashboardLoader = new(progress.Object, preferences.Object, TechniqueCatalogTestHelper.CreateTodayRecommendationResolver());
        TechniqueListBuilder listBuilder = new(progress.Object, TechniqueCatalogTestHelper.CreateGateway());
        TechniquesListInitializer initializer = new();
        Mock<INavigationService> navigation = new();

        TechniquesInitSnapshot snapshot = await initializer.LoadAsync(
            techniqueService.Object,
            listBuilder,
            dashboardLoader,
            navigation.Object,
            AppStrings.PracticeMyTechniques,
            CancellationToken.None);

        Assert.Equal(7, snapshot.StreakDays);
        Assert.True(snapshot.UiState.IsGrouped);
        Assert.NotEmpty(snapshot.StaticItems);
        Assert.NotEmpty(snapshot.UiState.Groups);
    }
}

public sealed class UserProfileRefreshCoordinatorTests
{
    [Fact]
    public async Task LoadDashboardAsync_ReturnsStatsAndHistory()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.CountTechniqueCompletionsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2L);
        progress.Setup(p => p.CountTestResultsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1L);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        Mock<IQuotService> quotService = new();
        ProfileQuotesLoader quotesLoader = new(quotService.Object, new ProfileQuotesPresenter());
        UserProfileRefreshCoordinator coordinator = new();

        UserProfileRefreshSnapshot? snapshot = await coordinator.LoadDashboardAsync(
            new ProfileStatsLoader(progress.Object),
            new ProfilePracticeHistoryLoader(progress.Object),
            quotesLoader,
            generation: 1,
            () => 1,
            forceQuotesReload: false,
            CancellationToken.None);

        Assert.NotNull(snapshot);
        Assert.Equal("2", snapshot!.Stats.TechniquesCompletedCount);
        Assert.Empty(snapshot.History);
        Assert.True(snapshot.ShouldLoadQuotes);
    }

    [Fact]
    public async Task LoadDashboardAsync_ReturnsNullWhenGenerationIsStale()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.CountTechniqueCompletionsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0L);
        progress.Setup(p => p.CountTestResultsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0L);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        Mock<IQuotService> quotService = new();
        ProfileQuotesLoader quotesLoader = new(quotService.Object, new ProfileQuotesPresenter());
        UserProfileRefreshCoordinator coordinator = new();

        UserProfileRefreshSnapshot? snapshot = await coordinator.LoadDashboardAsync(
            new ProfileStatsLoader(progress.Object),
            new ProfilePracticeHistoryLoader(progress.Object),
            quotesLoader,
            generation: 1,
            () => 2,
            forceQuotesReload: false,
            CancellationToken.None);

        Assert.Null(snapshot);
    }
}
