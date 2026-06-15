using Moq;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
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
    public void HasRecommendation_ReflectsResultInfo()
    {
        TestResultViewModel withRecommendation = CreateViewModel(new TestResultInfo
        {
            Score = 5,
            Interpretation = "Mild",
            RecommendedTechnique = TechniqueId.Paper,
            TestId = "beck",
            AnalyzerId = "beck"
        });

        TestResultViewModel withoutRecommendation = CreateViewModel(new TestResultInfo
        {
            Score = 5,
            Interpretation = "Mild",
            TestId = "beck",
            AnalyzerId = "beck"
        });

        Assert.True(withRecommendation.HasRecommendation);
        Assert.False(withoutRecommendation.HasRecommendation);
    }

    [Fact]
    public async Task RetakeCommand_DelegatesToRetakeOperations()
    {
        Mock<INavigation> navigation = new();
        RetakeTrackingNavigation navigationService = new(navigation.Object);
        FakeTestCatalogService catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            AnalyzerId = "beck",
            Title = "Beck",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire
        });

        TestResultViewModel viewModel = CreateViewModel(
            new TestResultInfo
            {
                Score = 8,
                Interpretation = "Score 8",
                TestId = "beck",
                AnalyzerId = "beck"
            },
            navigationService,
            catalog);

        viewModel.RetakeCommand.Execute(null);
        await Task.Delay(200);

        Assert.True(navigationService.WentToRoot);
    }

    [Fact]
    public async Task TryTechniqueCommand_NavigatesWhenRecommendationPresent()
    {
        TechniqueTrackingNavigation navigationService = new(Mock.Of<INavigation>());

        TestResultViewModel viewModel = CreateViewModel(
            new TestResultInfo
            {
                Score = 8,
                Interpretation = "Score 8",
                RecommendedTechnique = TechniqueId.Polarity,
                TestId = "beck",
                AnalyzerId = "beck"
            },
            navigationService);

        viewModel.TryTechniqueCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(TechniqueId.Polarity, navigationService.LastTechniqueId);
    }

    private static TestResultViewModel CreateViewModel(
        TestResultInfo result,
        INavigationService? navigationService = null,
        ITestCatalogService? catalogService = null) =>
        new(
            navigationService ?? new TestNavigationService(Mock.Of<INavigation>()),
            catalogService ?? new FakeTestCatalogService(),
            new TestRetakeOperations(),
            result);

    private sealed class RetakeTrackingNavigation(INavigation navigation) : TestNavigationService(navigation)
    {
        public bool WentToRoot { get; private set; }

        public override Task GoToRootAsync()
        {
            WentToRoot = true;
            return Task.CompletedTask;
        }
    }

    private sealed class TechniqueTrackingNavigation(INavigation navigation) : TestNavigationService(navigation)
    {
        public TechniqueId? LastTechniqueId { get; private set; }

        public override Task GoToTechniqueAsync(TechniqueId techniqueId)
        {
            LastTechniqueId = techniqueId;
            return Task.CompletedTask;
        }
    }
}
