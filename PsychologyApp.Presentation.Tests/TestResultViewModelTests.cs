using Moq;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestResultViewModelTests
{
    private readonly Mock<ITestCatalogService> _catalogService = new();

    public TestResultViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task FinishCommand_NavigatesToRoot()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);

        TestResultViewModel viewModel = new(
            navigation.Object,
            navigationService,
            _catalogService.Object,
            new TestResultInfo { Score = 5, Interpretation = "Mild" });

        viewModel.FinishCommand.Execute(null);
        await Task.Delay(50);

        navigation.Verify(n => n.PopToRootAsync(true), Times.Once);
    }

    [Fact]
    public async Task TryTechniqueCommand_WithRecommendation_NavigatesToTechnique()
    {
        var navigation = new Mock<INavigation>();
        var trackingNavigation = new TechniqueTrackingNavigationService(navigation.Object);

        TestResultViewModel viewModel = new(
            navigation.Object,
            trackingNavigation,
            _catalogService.Object,
            new TestResultInfo
            {
                Score = 12,
                Interpretation = "High",
                RecommendedTechnique = TechniqueId.Spin,
                AnalyzerId = "beck"
            });

        viewModel.TryTechniqueCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(TechniqueId.Spin, trackingNavigation.LastTechniqueId);
    }

    [Fact]
    public void HasRecommendation_IsFalse_WhenNoTechniqueRecommended()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);

        TestResultViewModel viewModel = new(
            navigation.Object,
            navigationService,
            _catalogService.Object,
            new TestResultInfo { Score = 3, Interpretation = "Low" });

        Assert.False(viewModel.HasRecommendation);
    }

    [Fact]
    public void Recommendation_includes_technique_title_when_present()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);

        TestResultViewModel viewModel = new(
            navigation.Object,
            navigationService,
            _catalogService.Object,
            new TestResultInfo
            {
                Score = 12,
                Interpretation = "10-15",
                RecommendedTechnique = TechniqueId.Spin,
                AnalyzerId = "beck"
            });

        Assert.Contains("Крутилка", viewModel.RecommendedTechniqueTitle);
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
