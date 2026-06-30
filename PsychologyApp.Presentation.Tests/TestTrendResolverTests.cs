using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Features.RunTests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestTrendResolverTests
{
    [Fact]
    public async Task LoadLatestTrendAsync_UsesHigherIsBetter_FromCatalog()
    {
        var catalog = new Mock<ITestCatalogService>();
        catalog
            .Setup(c => c.GetByIdAsync("who5", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestDefinition
            {
                TestId = "who5",
                Title = "WHO-5",
                Subtitle = "Sub",
                Description = "Desc",
                Comment = "Note",
                Algorithm = ["Step"],
                Kind = TestKind.Questionnaire,
                ScoreDirection = ScoreDirection.HigherIsBetter
            });

        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.GetTestResultHistoryAsync("who5", 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { Score = 20 },
                new() { Score = 10 }
            });

        TestTrendResolver resolver = new(new TestTrendService(catalog.Object));
        TestTrendSnapshot? snapshot = await resolver.LoadLatestTrendAsync("who5", progress.Object);

        Assert.NotNull(snapshot);
        Assert.Equal(TestTrendKind.Improved, snapshot!.Kind);
    }

    [Fact]
    public void BuildChartPoints_OrdersOldestToNewest()
    {
        TestTrendResolver resolver = new(new TestTrendService(new Mock<ITestCatalogService>().Object));
        IReadOnlyList<TestResultDTO> history =
        [
            new() { Score = 3, CompletedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Score = 1, CompletedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        ];

        IReadOnlyList<TestScoreChartPoint> points = resolver.BuildChartPoints(history);

        Assert.Equal(2, points.Count);
        Assert.Equal(1, points[0].Score);
        Assert.Equal(3, points[1].Score);
    }
}
