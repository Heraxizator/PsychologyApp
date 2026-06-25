using Moq;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.Onboarding;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Tests.Stubs;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class OnboardingViewModelTests
{
    private static OnboardingViewModel CreateViewModel()
    {
        Mock<INavigationService> navigation = new();
        return new OnboardingViewModel(
            navigation.Object,
            new InMemoryUserPreferencesStore(),
            _ => Task.CompletedTask);
    }

    [Fact]
    public void SelectAnxietyCommand_SetsPrimaryVariant()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        viewModel.SelectAnxietyCommand.Execute(null);

        Assert.Equal(OnboardingConcernKeys.Anxiety, viewModel.SelectedConcern);
        Assert.Equal("Primary", viewModel.ConcernAnxietyVariant);
        Assert.Equal(3, viewModel.Step);
    }

    [Fact]
    public void SelectMoodCommand_AdvancesToFinishStep()
    {
        OnboardingViewModel viewModel = CreateViewModel();
        viewModel.Step = 2;

        viewModel.SelectMoodCommand.Execute(null);

        Assert.Equal(OnboardingConcernKeys.Mood, viewModel.SelectedConcern);
        Assert.True(viewModel.IsFinishStep);
    }

    [Fact]
    public void NextCommand_AdvancesWelcomeAndOverviewSteps()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        viewModel.NextCommand.Execute(null);
        Assert.Equal(1, viewModel.Step);
        Assert.True(viewModel.IsOverviewStep);

        viewModel.NextCommand.Execute(null);
        Assert.Equal(2, viewModel.Step);
        Assert.True(viewModel.IsConcernStep);
    }

    [Fact]
    public void BackCommand_OnConcernStep_ReturnsToOverview()
    {
        OnboardingViewModel viewModel = CreateViewModel();
        viewModel.Step = 2;

        viewModel.BackCommand.Execute(null);

        Assert.Equal(1, viewModel.Step);
        Assert.True(viewModel.IsOverviewStep);
    }

    [Fact]
    public void RecommendedTitle_AfterAnxietySelection_IsSpinTitle()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        viewModel.SelectAnxietyCommand.Execute(null);

        TechniqueDefinition spin = TechniqueCatalog.Get(TechniqueId.Spin);
        Assert.Equal(spin.ListTitle, viewModel.RecommendedTitle);
        Assert.False(string.IsNullOrWhiteSpace(viewModel.RecommendedReason));
    }

    [Fact]
    public void StepLabel_UsesOneBasedIndex()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.Contains("1", viewModel.StepLabel);

        viewModel.Step = 1;
        Assert.Contains("2", viewModel.StepLabel);
    }

    [Fact]
    public void AppTagline_And_ConcernFooterHint_AreNotEmpty()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.False(string.IsNullOrWhiteSpace(viewModel.AppTagline));
        Assert.False(string.IsNullOrWhiteSpace(viewModel.ConcernFooterHint));
    }

    [Fact]
    public void IsConcernFooterVisible_OnlyOnConcernStep()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.False(viewModel.IsConcernFooterVisible);

        viewModel.Step = 1;
        Assert.False(viewModel.IsConcernFooterVisible);

        viewModel.Step = 2;
        Assert.True(viewModel.IsConcernFooterVisible);

        viewModel.Step = 3;
        Assert.False(viewModel.IsConcernFooterVisible);
    }

    [Fact]
    public void IsStepLabelOnIndicatorVisible_OnlyOnWelcomeStep()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.True(viewModel.IsStepLabelOnIndicatorVisible);

        viewModel.Step = 1;
        Assert.False(viewModel.IsStepLabelOnIndicatorVisible);
    }

    [Fact]
    public void ValueStripLabels_AreNotEmpty()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.False(string.IsNullOrWhiteSpace(viewModel.ValueOfflineLabel));
        Assert.False(string.IsNullOrWhiteSpace(viewModel.ValueNoJudgmentLabel));
        Assert.False(string.IsNullOrWhiteSpace(viewModel.ValueOnDeviceLabel));
    }

    [Fact]
    public void ConcernHints_AreNotEmpty()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.False(string.IsNullOrWhiteSpace(viewModel.ConcernAnxietyHint));
        Assert.False(string.IsNullOrWhiteSpace(viewModel.ConcernBodyHint));
        Assert.False(string.IsNullOrWhiteSpace(viewModel.ConcernMoodHint));
        Assert.False(string.IsNullOrWhiteSpace(viewModel.ConcernExploreHint));
    }

    [Fact]
    public void StartLabel_UsesTryItNowCopy()
    {
        OnboardingViewModel viewModel = CreateViewModel();

        Assert.Equal(AppStrings.OnboardingStart, viewModel.StartLabel);
    }
}
