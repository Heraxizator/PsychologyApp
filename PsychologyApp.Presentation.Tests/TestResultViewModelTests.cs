using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.RunTests.TestResult;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestResultViewModelTests
{
    public TestResultViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task LoadTrend_ShowsImproved_WhenScoreDecreased()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var catalog = new Mock<ITestCatalogService>();
        catalog
            .Setup(c => c.GetByIdAsync("beck", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestDefinition
            {
                TestId = "beck",
                Title = "Beck",
                Subtitle = "Sub",
                Description = "Desc",
                Comment = "Note",
                Algorithm = ["Step"],
                Kind = TestKind.Questionnaire,
                AnalyzerId = "beck",
                Questions = [],
                SingleAnswer = true,
                ScoreDirection = ScoreDirection.LowerIsBetter
            });
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.GetTestResultHistoryAsync("beck", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { Score = 3, Summary = "Mild" },
                new() { Score = 8, Summary = "Moderate" }
            });

        TestResultViewModel viewModel = new(
            navigationService,
            catalog.Object,
            TechniqueCatalogTestHelper.CreateGateway(),
            TestRunTestHelpers.CreateRetakeOperations(),
            progress.Object,
            new TestTrendResolver(catalog.Object),
            new TestResultInfo
            {
                Score = 3,
                Interpretation = "Mild",
                TestId = "beck"
            });

        await Task.Delay(100);

        Assert.True(viewModel.HasTrendBadge);
        Assert.Equal(TestTrendKind.Improved, viewModel.TrendKind);
        Assert.Equal(AppStrings.TestResultImproved, viewModel.TrendText);
    }

    [Fact]
    public async Task LoadTrend_ShowsImproved_WhenScoreIncreased_AndHigherIsBetter()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
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
                AnalyzerId = "who5",
                Questions = [],
                SingleAnswer = true,
                ScoreDirection = ScoreDirection.HigherIsBetter
            });
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.GetTestResultHistoryAsync("who5", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { Score = 20, Summary = "Good" },
                new() { Score = 10, Summary = "Low" }
            });

        TestResultViewModel viewModel = new(
            navigationService,
            catalog.Object,
            TechniqueCatalogTestHelper.CreateGateway(),
            TestRunTestHelpers.CreateRetakeOperations(),
            progress.Object,
            new TestTrendResolver(catalog.Object),
            new TestResultInfo
            {
                Score = 20,
                Interpretation = "Good",
                TestId = "who5"
            });

        await Task.Delay(100);

        Assert.True(viewModel.HasTrendBadge);
        Assert.Equal(TestTrendKind.Improved, viewModel.TrendKind);
        Assert.Equal(AppStrings.TestResultImproved, viewModel.TrendText);
    }

    [Fact]
    public async Task TryTechniqueCommand_NavigatesToTechnique()
    {
        var navigation = new Mock<INavigation>();
        var tracking = new TechniqueTrackingNavigationService(navigation.Object);
        var catalog = new Mock<ITestCatalogService>();
        var progress = new Mock<IUserProgressService>();

        TestResultViewModel viewModel = new(
            tracking,
            catalog.Object,
            TechniqueCatalogTestHelper.CreateGateway(),
            TestRunTestHelpers.CreateRetakeOperations(),
            progress.Object,
            new TestTrendResolver(catalog.Object),
            new TestResultInfo
            {
                Score = 5,
                Interpretation = "Moderate",
                RecommendedTechnique = TechniqueId.Polarity
            });

        viewModel.TryTechniqueCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(TechniqueId.Polarity, tracking.LastTechniqueId);
    }

    private sealed class TechniqueTrackingNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public TechniqueId? LastTechniqueId { get; private set; }

        public override Task GoToTechniqueAsync(TechniqueId techniqueId)
        {
            LastTechniqueId = techniqueId;
            return Task.CompletedTask;
        }
    }
}
