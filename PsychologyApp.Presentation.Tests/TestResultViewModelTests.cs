using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Models.Practice.Techniques;
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
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.GetTestResultHistoryAsync("beck", 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { Score = 3, Summary = "Mild" },
                new() { Score = 8, Summary = "Moderate" }
            });

        TestResultViewModel viewModel = new(
            navigationService,
            catalog.Object,
            new TestRetakeOperations(),
            progress.Object,
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
    public async Task TryTechniqueCommand_NavigatesToTechnique()
    {
        var navigation = new Mock<INavigation>();
        var tracking = new TechniqueTrackingNavigationService(navigation.Object);
        var catalog = new Mock<ITestCatalogService>();
        var progress = new Mock<IUserProgressService>();

        TestResultViewModel viewModel = new(
            tracking,
            catalog.Object,
            new TestRetakeOperations(),
            progress.Object,
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
