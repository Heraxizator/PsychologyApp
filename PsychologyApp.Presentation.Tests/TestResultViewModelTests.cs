using Moq;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestResultViewModelTests
{
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
            new TestResultInfo
            {
                Score = 12,
                Interpretation = "High",
                RecommendedTechnique = TechniqueId.Spin
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
            new TestResultInfo { Score = 3, Interpretation = "Low" });

        Assert.False(viewModel.HasRecommendation);
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
