using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.Onboarding;
using PsychologyApp.Presentation.Pages.Onboarding;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>First launch: the person chooses a concern, gets one practice recommended, and the choices are stored.</summary>
public sealed class OnboardingViewModelTests
{
    private readonly Mock<IUserPreferencesStore> _preferences = new();
    private readonly Mock<IClinicalCareService> _care = new();
    private readonly Mock<ITechniqueCatalogService> _catalog = new();
    private readonly Mock<ITechniqueRecommendationService> _recommendation = new();
    private readonly List<TechniqueId?> _completed = [];

    private static BuiltInTechniqueDefinition Definition(string title) =>
        new("module", "page", [], "theory", TechniqueUiKind.None, "1", "d", title, "subtitle", "theme", "author", 5, "icon");

    private OnboardingViewModel Create()
    {
        _recommendation.Setup(r => r.ResolveFromOnboardingConcern(OnboardingConcernKeys.Anxiety)).Returns(TechniqueId.Grounding);
        _recommendation.Setup(r => r.ResolveFromOnboardingConcern(It.Is<string>(c => c != OnboardingConcernKeys.Anxiety))).Returns(TechniqueId.Spin);
        _catalog.Setup(c => c.GetAsync(TechniqueId.Grounding, It.IsAny<CancellationToken>())).ReturnsAsync(Definition("Grounding 5-4-3-2-1"));
        _catalog.Setup(c => c.GetAsync(TechniqueId.Spin, It.IsAny<CancellationToken>())).ReturnsAsync(Definition("Spin"));
        _care.Setup(c => c.EnsureProgramAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new PsychologyApp.Application.Models.TherapyProgramStateDTO());
        OnboardingRecommendationResolver resolver = new(_catalog.Object, _recommendation.Object);
        return new OnboardingViewModel(
            new Mock<INavigationService>().Object,
            _preferences.Object,
            resolver,
            _care.Object,
            technique =>
            {
                _completed.Add(technique);
                return Task.CompletedTask;
            });
    }

    [Fact]
    public async Task TheFlowStartsOnTheWelcomeStepAndMovesForwardAndBack()
    {
        OnboardingViewModel viewModel = Create();
        Assert.True(viewModel.IsWelcomeStep);
        Assert.False(viewModel.IsBackVisible);
        Assert.True(viewModel.IsNextFooterVisible);

        viewModel.NextCommand.Execute(null);
        Assert.True(viewModel.IsOverviewStep);
        Assert.True(viewModel.IsBackVisible);

        await Task.Delay(500);
        viewModel.NextCommand.Execute(null);
        Assert.True(viewModel.IsConcernStep);
        Assert.True(viewModel.IsConcernFooterVisible);
        Assert.False(viewModel.IsNextFooterVisible);

        await Task.Delay(500);
        viewModel.NextCommand.Execute(null);
        Assert.True(viewModel.IsConcernStep);

        await Task.Delay(500);
        viewModel.BackCommand.Execute(null);
        Assert.True(viewModel.IsOverviewStep);
    }

    [Fact]
    public void ADoubleTapOnNextMovesOnlyOneStep()
    {
        OnboardingViewModel viewModel = Create();

        viewModel.NextCommand.Execute(null);
        viewModel.NextCommand.Execute(null);

        Assert.Equal(1, viewModel.Step);
    }

    [Theory]
    [InlineData(nameof(OnboardingViewModel.SelectAnxietyCommand), OnboardingConcernKeys.Anxiety, "Grounding 5-4-3-2-1")]
    [InlineData(nameof(OnboardingViewModel.SelectMoodCommand), OnboardingConcernKeys.Mood, "Spin")]
    public async Task ChoosingAConcernShowsTheFinishStepWithThatRecommendation(string command, string concern, string title)
    {
        OnboardingViewModel viewModel = Create();
        System.Windows.Input.ICommand select = (System.Windows.Input.ICommand)typeof(OnboardingViewModel).GetProperty(command)!.GetValue(viewModel)!;

        await VmTestHelpers.RunAsync(select);

        Assert.True(viewModel.IsFinishStep);
        Assert.Equal(concern, viewModel.SelectedConcern);
        Assert.Equal(title, viewModel.RecommendedTitle);
        Assert.Equal("icon", viewModel.RecommendedIconName);
    }

    [Fact]
    public async Task StartingThePracticeStoresTheChoicesAndHandsOverTheRecommendedPractice()
    {
        OnboardingViewModel viewModel = Create();
        await VmTestHelpers.RunAsync(viewModel.SelectAnxietyCommand);
        viewModel.PracticeRemindersEnabled = true;
        viewModel.PracticeReminderHour = "08:00";

        await VmTestHelpers.RunAsync(viewModel.StartPracticeCommand);

        _preferences.Verify(p => p.CompleteOnboarding(OnboardingConcernKeys.Anxiety, true, 8), Times.Once);
        _care.Verify(c => c.EnsureProgramAsync(OnboardingConcernKeys.Anxiety, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal([TechniqueId.Grounding], _completed);
    }

    [Fact]
    public async Task SkippingStoresTheChoicesButStartsNoPractice()
    {
        OnboardingViewModel viewModel = Create();
        viewModel.PracticeRemindersEnabled = false;

        await VmTestHelpers.RunAsync(viewModel.SkipCommand);

        _preferences.Verify(p => p.CompleteOnboarding(OnboardingConcernKeys.Explore, false, It.IsAny<int>()), Times.Once);
        Assert.Equal([null], _completed);
    }

    [Fact]
    public async Task AFailedProgramSeedDoesNotStopTheOnboardingFromCompleting()
    {
        _care.Setup(c => c.EnsureProgramAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));
        OnboardingViewModel viewModel = Create();
        _care.Setup(c => c.EnsureProgramAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));

        await VmTestHelpers.RunAsync(viewModel.SkipCommand);

        _preferences.Verify(p => p.CompleteOnboarding(It.IsAny<string>(), It.IsAny<bool?>(), It.IsAny<int?>()), Times.Once);
        Assert.Single(_completed);
    }

    [Fact]
    public void TheReminderHourIsParsedFromItsLabelAndOffersEveryHour()
    {
        OnboardingViewModel viewModel = Create();

        viewModel.PracticeReminderHour = "09:00";
        Assert.Equal("09:00", viewModel.PracticeReminderHour);

        viewModel.PracticeReminderHour = "03:00";
        Assert.Equal(viewModel.PracticeReminderHourOptions[0], viewModel.PracticeReminderHour);
        Assert.DoesNotContain("03:00", viewModel.PracticeReminderHourOptions);
        Assert.Contains("21:00", viewModel.PracticeReminderHourOptions);
    }

    [Fact]
    public void TheStepLabelCountsFromOne()
    {
        OnboardingViewModel viewModel = Create();

        Assert.Contains("1", viewModel.StepLabel);
        Assert.Contains(OnboardingViewModel.StepCount.ToString(), viewModel.StepLabel);
    }
}
