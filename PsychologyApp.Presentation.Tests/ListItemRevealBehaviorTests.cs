using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Behaviors;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ListItemRevealBehaviorTests
{
    [Fact]
    public void RevealIndex_DefaultsToNegativeOne()
    {
        ListItemRevealBehavior behavior = new();

        Assert.Equal(-1, behavior.RevealIndex);
    }

    [Fact]
    public void RevealIndex_CanBeSetExplicitly()
    {
        ListItemRevealBehavior behavior = new()
        {
            RevealIndex = 3
        };

        Assert.Equal(3, behavior.RevealIndex);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 1)]
    [InlineData(8, 1)]
    [InlineData(9, 2)]
    [InlineData(20, 2)]
    public void ResolveRevealTier_UsesPremiumLiteAndFadeTiers(int index, int expectedTier)
    {
        Assert.Equal(expectedTier, ListItemRevealBehavior.ResolveRevealTier(index));
    }

    [Fact]
    public void MaxConcurrentListReveals_IsSix()
    {
        Assert.Equal(6, UiAnimations.MaxConcurrentListReveals);
    }
}
