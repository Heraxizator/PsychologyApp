using PsychologyApp.Presentation.Core.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class OnboardingStepNavigatorTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void GoNext_AdvancesOnlyOnWelcomeAndOverview(int current, int expected) =>
        Assert.Equal(expected, OnboardingStepNavigator.GoNext(current));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    public void GoBack_ReturnsToPreviousStep(int current, int expected) =>
        Assert.Equal(expected, OnboardingStepNavigator.GoBack(current));

    [Fact]
    public void FinishStep_IsLastOnboardingStep() =>
        Assert.Equal(3, OnboardingStepNavigator.FinishStep);
}
