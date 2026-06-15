using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Profile;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Services.Preferences;
using PsychologyApp.Presentation.Services.Profile;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuoteFeedLoaderTests
{
    public QuoteFeedLoaderTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task LoadItemsAsync_SeedsMapsAndDedupesQuotes()
    {
        Mock<IQuotService> quotService = new();
        quotService
            .Setup(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        quotService
            .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO { QuotId = 1, Text = "Alpha", Title = "A", IsFavourite = false, IsReaded = false },
                new QuotDTO { QuotId = 2, Text = "Alpha", Title = "B", IsFavourite = false, IsReaded = false },
                new QuotDTO { QuotId = 3, Text = "Beta", Title = "C", IsFavourite = false, IsReaded = false }
            ]);

        QuoteFeedCoordinator coordinator = new();
        QuoteFeedLoader loader = new();
        QuoteItemCommandsFactory factory = CreateQuoteItemFactory(quotService.Object);
        bool failed = false;

        IReadOnlyList<QuoteItem> items = await loader.LoadItemsAsync(
            coordinator,
            quotService.Object,
            factory,
            count: 20,
            resetKnown: true,
            seedNewQuote: true,
            _ => Task.CompletedTask,
            () => failed = true,
            CancellationToken.None);

        quotService.Verify(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.False(failed);
        Assert.Equal(2, items.Count);
        Assert.Equal("Alpha", items[0].Text);
        Assert.Equal("Beta", items[1].Text);
    }

    [Fact]
    public async Task LoadItemsAsync_FreshCoordinators_DoNotShareKnownQuotes()
    {
        Mock<IQuotService> quotService = new();
        quotService
            .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO { QuotId = 1, Text = "Shared", Title = "A", IsFavourite = false, IsReaded = false }
            ]);

        QuoteFeedCoordinator firstCoordinator = new();
        QuoteFeedCoordinator secondCoordinator = new();
        QuoteFeedLoader loader = new();
        QuoteItemCommandsFactory factory = CreateQuoteItemFactory(quotService.Object);

        await loader.LoadItemsAsync(
            firstCoordinator,
            quotService.Object,
            factory,
            count: 20,
            resetKnown: true,
            seedNewQuote: false,
            _ => Task.CompletedTask,
            () => { },
            CancellationToken.None);

        IReadOnlyList<QuoteItem> secondLoad = await loader.LoadItemsAsync(
            secondCoordinator,
            quotService.Object,
            factory,
            count: 20,
            resetKnown: true,
            seedNewQuote: false,
            _ => Task.CompletedTask,
            () => { },
            CancellationToken.None);

        Assert.Single(secondLoad);
    }

    private static QuoteItemCommandsFactory CreateQuoteItemFactory(IQuotService quotService) =>
        new(
            quotService,
            new QuotesChangeNotifier(),
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
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        progress
            .Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal));
        progress
            .Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));

        Mock<ITechniqueService> techniqueService = new();
        techniqueService
            .Setup(s => s.GetTechniquesListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
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

        PracticeDashboardLoader dashboardLoader = new(progress.Object, preferences.Object);
        TechniqueListBuilder listBuilder = new(progress.Object);
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
