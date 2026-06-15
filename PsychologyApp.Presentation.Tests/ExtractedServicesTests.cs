using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Models.Clean;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Clean;
using PsychologyApp.Presentation.Services.Physics;
using PsychologyApp.Presentation.Services.Preferences;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Services.Profile;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.ViewModels.Motivator;
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

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object);
        MoodSnapshot snapshot = await loader.LoadMoodSnapshotAsync();

        Assert.Equal(string.Empty, snapshot.TodayMoodDisplay);
        Assert.Equal(0, snapshot.SelectedMoodLevel);
    }

    [Fact]
    public void ResolveTodayRecommendation_UsesOnboardingConcern()
    {
        Mock<IUserProgressService> progress = new();
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Mood });
        Mock<INavigationService> navigation = new();

        PracticeDashboardLoader loader = new(progress.Object, preferences.Object);
        TodayRecommendationResult result = loader.ResolveTodayRecommendation(streakDays: 0, navigation.Object);

        Assert.Equal(TechniqueId.Paper, result.TechniqueId);
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
    public void ShouldShowAllReadEmpty_OnlyForAllFeedWithNoItems()
    {
        QuoteFeedCoordinator coordinator = new();

        Assert.True(coordinator.ShouldShowAllReadEmpty(collectionCount: 0, isDone: true));
        coordinator.TrySwitchFeed(QuoteFeedMode.Favorites);
        Assert.False(coordinator.ShouldShowAllReadEmpty(collectionCount: 0, isDone: true));
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
    public void Build_ReturnsFourTechniquesIncludingRecommended()
    {
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Body });
        Mock<INavigationService> navigation = new();
        ProfileFeaturedTechniquesBuilder builder = new(preferences.Object);

        IReadOnlyList<TechniqueItem> items = builder.Build(navigation.Object);

        Assert.Equal(4, items.Count);
        Assert.All(items, item => Assert.NotNull(item.TapCommand));
    }
}
