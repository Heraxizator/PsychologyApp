using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TodayRecommendationResolverTests
{
    [Theory]
    [InlineData(OnboardingConcernKeys.Anxiety, TechniqueId.Spin)]
    [InlineData(OnboardingConcernKeys.Body, TechniqueId.Experience)]
    [InlineData(OnboardingConcernKeys.Mood, TechniqueId.Paper)]
    public void Resolve_MapsConcernToTechnique(string concern, TechniqueId expected)
    {
        Mock<INavigationService> navigation = new();

        TodayRecommendationResult result = TodayRecommendationResolver.Resolve(
            concern,
            "3 дн.",
            hasStreak: true,
            navigation.Object);

        Assert.Equal(expected, result.TechniqueId);
        Assert.False(string.IsNullOrWhiteSpace(result.ReasonText));
        Assert.NotNull(result.Item.TapCommand);
    }
}

public sealed class TechniqueListBuilderTests
{
    [Fact]
    public void BuildLayout_GroupsWhenCustomTechniquesExist()
    {
        TechniqueListBuilder builder = new(new Mock<IUserProgressService>().Object);
        List<TechniqueItem> staticItems = [new() { Title = "Static" }];
        List<TechniqueItem> customItems = [new() { Title = "Custom" }];

        TechniqueListLayout layout = builder.BuildLayout(staticItems, customItems, "Mine");

        Assert.True(layout.IsGrouped);
        Assert.Equal(2, layout.Groups.Count);
    }

    [Fact]
    public void MapCustomItems_MapsDtoFields()
    {
        Mock<INavigationService> navigation = new();
        TechniqueListBuilder builder = new(new Mock<IUserProgressService>().Object);

        IReadOnlyList<TechniqueItem> items = builder.MapCustomItems(
        [
            new TechniqueDTO
            {
                TechniqueId = 7,
                Header = "Header",
                Description = "Desc",
                Subject = "Theme",
                Author = "Author",
                Date = "2026-06-13"
            }
        ],
        navigation.Object);

        Assert.Single(items);
        Assert.Equal("Header", items[0].Title);
        Assert.Equal("Desc", items[0].Subtitle);
    }

    [Fact]
    public async Task BuildStaticItemsAsync_UsesBatchProgressQueries()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal)
            {
                [TechniqueId.Spin.ToString()] = DateTime.UtcNow.AddDays(-1)
            });
        progress
            .Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal) { TechniqueId.Paper.ToString() });

        TechniqueListBuilder builder = new(progress.Object);
        IReadOnlyList<TechniqueItem> items = await builder.BuildStaticItemsAsync(Mock.Of<INavigationService>());

        Assert.Equal(TechniqueListCatalog.GetBuiltIn().Count, items.Count);
        progress.Verify(
            p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        progress.Verify(
            p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()),
            Times.Once);
        progress.Verify(
            p => p.GetLastPracticeDateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        progress.Verify(
            p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
