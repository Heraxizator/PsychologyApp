using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class StreakMilestoneCopyTests
{
    public StreakMilestoneCopyTests()
    {
        AppStrings.LanguageOverride = "en";
    }

    [Theory]
    [InlineData(3)]
    [InlineData(7)]
    [InlineData(14)]
    [InlineData(30)]
    public void IsStreakMilestone_RecognizesConfiguredDays(int streak)
    {
        Assert.True(AppStrings.IsStreakMilestone(streak));
        Assert.NotEqual(AppStrings.PracticeCompletedTitle, AppStrings.PracticeMilestoneTitle(streak));
        Assert.NotEqual(AppStrings.PracticeCompletedBody(streak), AppStrings.PracticeMilestoneBody(streak));
    }

    [Fact]
    public void IsStreakMilestone_IgnoresOtherDays()
    {
        Assert.False(AppStrings.IsStreakMilestone(1));
        Assert.False(AppStrings.IsStreakMilestone(8));
        Assert.Equal(AppStrings.PracticeCompletedTitle, AppStrings.PracticeMilestoneTitle(5));
    }

    [Fact]
    public void ComebackBannerWithTechnique_IncludesName()
    {
        string text = AppStrings.ComebackBannerWithTechnique("Grounding");
        Assert.Contains("Grounding", text, StringComparison.Ordinal);
    }

    [Fact]
    public void PracticeMoodDelta_IncludesBothLevels()
    {
        string text = AppStrings.PracticeMoodDelta(2, 4);
        Assert.Contains("2", text, StringComparison.Ordinal);
        Assert.Contains("4", text, StringComparison.Ordinal);
    }
}
