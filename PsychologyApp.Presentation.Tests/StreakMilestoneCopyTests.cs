using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
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

    [Theory]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(250)]
    [InlineData(500)]
    [InlineData(1000)]
    public void IsLifetimeMilestone_RecognizesConfiguredTotals(long total)
    {
        Assert.True(AppStrings.IsLifetimeMilestone(total));
        Assert.NotEqual(AppStrings.PracticeCompletedTitle, AppStrings.PracticeLifetimeMilestoneTitle(total));
        Assert.NotEqual(AppStrings.PracticeCompletedBody(0), AppStrings.PracticeLifetimeMilestoneBody(total));
    }

    [Fact]
    public void IsLifetimeMilestone_IgnoresOtherTotals()
    {
        Assert.False(AppStrings.IsLifetimeMilestone(1));
        Assert.False(AppStrings.IsLifetimeMilestone(11));
        Assert.Equal(AppStrings.PracticeCompletedTitle, AppStrings.PracticeLifetimeMilestoneTitle(5));
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
