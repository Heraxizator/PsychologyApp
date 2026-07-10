using PsychologyApp.Domain.Practice;
using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Features.SearchPhysics;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageProfile.Index;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;
using System.Collections.ObjectModel;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PracticeDashboardLoaderTests
{
    [Fact]
    public async Task LoadMoodSnapshot_ReturnsEmptyWhenNoMoods()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentMoodsAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Anxiety });

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object, TechniqueCatalogTestHelper.CreateTodayRecommendationResolver());
        MoodSnapshot snapshot = await loader.LoadMoodSnapshotAsync();

        Assert.Equal(string.Empty, snapshot.TodayMoodDisplay);
        Assert.Equal(0, snapshot.SelectedMoodLevel);
    }

    [Fact]
    public async Task LoadWeeklyInsight_HidesWhenNoWeekData()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Mock<IUserPreferencesStore> preferences = new();

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object, TechniqueCatalogTestHelper.CreateTodayRecommendationResolver());
        WeeklyInsightSnapshot insight = await loader.LoadWeeklyInsightAsync();

        Assert.False(insight.HasInsight);
    }

    [Fact]
    public async Task LoadWeeklyInsight_ShowsPracticeCount_WhenCompletionsThisWeek()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new CompletionDTO
                {
                    ItemKey = "Spin",
                    PageName = "Spin",
                    CompletedAt = DateTime.UtcNow
                }
            ]);
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        Mock<IUserPreferencesStore> preferences = new();

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object, TechniqueCatalogTestHelper.CreateTodayRecommendationResolver());
        WeeklyInsightSnapshot insight = await loader.LoadWeeklyInsightAsync();

        Assert.True(insight.HasInsight);
        Assert.Contains("1", insight.DisplayText, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LoadLastTechniqueName_ReturnsPageName()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new CompletionDTO { ItemKey = "Spin", PageName = "Вращение", CompletedAt = DateTime.UtcNow }
            ]);
        Mock<IUserPreferencesStore> preferences = new();

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object, TechniqueCatalogTestHelper.CreateTodayRecommendationResolver());
        string? name = await loader.LoadLastTechniqueNameAsync();

        Assert.Equal("Вращение", name);
    }

    [Fact]
    public async Task ResolveTodayRecommendation_UsesOnboardingConcern()
    {
        Mock<IUserProgressService> progress = new();
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Mood });
        Mock<INavigationService> navigation = new();

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object, TechniqueCatalogTestHelper.CreateTodayRecommendationResolver());
        TodayRecommendationResult result = await loader.ResolveTodayRecommendationAsync(streakDays: 0, navigation.Object);

        Assert.Equal(TechniqueId.SmallStep, result.TechniqueId);
    }
}

public sealed class QuoteFeedCoordinatorTests
{
    [Fact]
    public void TrySwitchFeed_ReturnsTrueWhenModeChanges()
    {
        QuoteFeedCoordinator coordinator = new();

        Assert.True(coordinator.TrySwitchFeed(QuoteFeedMode.Favorites));
        Assert.False(coordinator.TrySwitchFeed(QuoteFeedMode.Favorites));
    }

    [Fact]
    public async Task ShouldShowAllReadEmptyAsync_OnlyForAllFeedWithNoItems()
    {
        QuoteFeedCoordinator coordinator = new();
        Mock<IQuotService> quotService = new();
        quotService.Setup(s => s.IsAllCaughtUpAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        Assert.True(await coordinator.ShouldShowAllReadEmptyAsync(collectionCount: 0, isDone: true, quotService.Object, CancellationToken.None));
        coordinator.TrySwitchFeed(QuoteFeedMode.Favorites);
        Assert.False(await coordinator.ShouldShowAllReadEmptyAsync(collectionCount: 0, isDone: true, quotService.Object, CancellationToken.None));
    }

    [Fact]
    public async Task FetchQuotesAsync_ForYou_DoesNotFallbackToUnreadLatest()
    {
        QuoteFeedCoordinator coordinator = new();
        coordinator.SetFeedMode(QuoteFeedMode.ForYou);
        Mock<IQuotService> quotService = new();
        quotService
            .Setup(s => s.GetUnreadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PsychologyApp.Application.Models.QuotDTO>());
        quotService
            .Setup(s => s.EnsureThemedQuotesInFeedAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        IReadOnlyList<PsychologyApp.Application.Models.QuotDTO> result =
            await coordinator.FetchQuotesAsync(quotService.Object, 20, CancellationToken.None);

        Assert.Empty(result);
        quotService.Verify(
            s => s.EnsureThemedQuotesInFeedAsync(It.IsAny<IReadOnlyList<string>>(), 20, It.IsAny<CancellationToken>()),
            Times.Once);
        quotService.Verify(
            s => s.GetUnreadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

public sealed class PhysicsSearchUiStateTests
{
    [Fact]
    public void Resolve_ShowsFilteringWhileSearching()
    {
        PhysicsSearchUiSnapshot snapshot = PhysicsSearchUiState.Resolve(
            isDone: true,
            searchText: "stress",
            isSearching: true,
            resultCount: 0);

        Assert.True(snapshot.IsSearchFilteringVisible);
        Assert.False(snapshot.IsSearchResultsListVisible);
    }
}

public sealed class MusicPlaylistPresenterTests
{
    [Fact]
    public void Filter_AppliesCategoryAndSearch()
    {
        MusicPlaylistPresenter presenter = new();
        List<Audio> items =
        [
            new() { Name = "Alpha", Category = "Core", URL = "a" },
            new() { Name = "Beta", Category = "Sleep", URL = "b" }
        ];

        ObservableCollection<Audio> filtered = presenter.Filter(items, "Sleep", query: string.Empty);

        Assert.Single(filtered);
        Assert.Equal("Beta", filtered[0].Name);
    }
}

public sealed class ProfileFeaturedTechniquesBuilderTests
{
    [Fact]
    public async Task Build_ReturnsFourTechniquesIncludingRecommended()
    {
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Body });
        Mock<INavigationService> navigation = new();
        ProfileFeaturedTechniquesBuilder builder = new(
            preferences.Object,
            TechniqueCatalogTestHelper.CreateGateway(),
            TechniqueCatalogTestHelper.CreateRecommendationService());

        IReadOnlyList<TechniqueItem> items = await builder.BuildAsync(navigation.Object);

        Assert.Equal(4, items.Count);
        Assert.All(items, item => Assert.NotNull(item.TapCommand));
    }
}
