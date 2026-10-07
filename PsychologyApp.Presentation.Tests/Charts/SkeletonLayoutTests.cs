using PsychologyApp.Presentation.Core.Charts;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Charts;

/// <summary>The shapes of the loading placeholders: they copy the real layout and stay the same between loads.</summary>
public class SkeletonLayoutTests
{
    [Fact]
    public void TheNumberOfGroupsFollowsTheRequestWithinLimits()
    {
        Assert.Equal(4, SkeletonLayout.Build(SkeletonKind.Cards, 4).Count);
        Assert.Single(SkeletonLayout.Build(SkeletonKind.Cards, 0));
        Assert.Equal(SkeletonLayout.MaxCount, SkeletonLayout.Build(SkeletonKind.Rows, 500).Count);
    }

    [Theory]
    [InlineData(SkeletonKind.Cards)]
    [InlineData(SkeletonKind.Rows)]
    [InlineData(SkeletonKind.Bubbles)]
    [InlineData(SkeletonKind.Profile)]
    public void EveryBlockHasASaneSizeAndTheLayoutNeverChanges(SkeletonKind kind)
    {
        IReadOnlyList<SkeletonGroup> first = SkeletonLayout.Build(kind, SkeletonLayout.DefaultCount(kind));
        IReadOnlyList<SkeletonGroup> second = SkeletonLayout.Build(kind, SkeletonLayout.DefaultCount(kind));

        Assert.NotEmpty(first);
        foreach (SkeletonBlock block in first.SelectMany(g => g.Blocks))
        {
            Assert.InRange(block.WidthFraction, 0, 1);
            Assert.InRange(block.Height, 8, 120);
            Assert.True(block.Circle || block.WidthFraction > 0);
        }

        Assert.Equal(first.Count, second.Count);
        Assert.Equal(
            first.SelectMany(g => g.Blocks).Select(b => (b.WidthFraction, b.Height, b.Align)),
            second.SelectMany(g => g.Blocks).Select(b => (b.WidthFraction, b.Height, b.Align)));
    }

    [Fact]
    public void CardsAreBoxedAndRowsStartWithARoundPicture()
    {
        Assert.All(SkeletonLayout.Build(SkeletonKind.Cards, 3), g => Assert.True(g.InCard));
        Assert.All(SkeletonLayout.Build(SkeletonKind.Rows, 3), g => Assert.True(g.LeadingCircle));
    }

    [Fact]
    public void ChatBubblesAlternateBetweenTheTwoSides()
    {
        SkeletonAlign[] sides = [.. SkeletonLayout.Build(SkeletonKind.Bubbles, 6).Select(g => g.Blocks[0].Align)];

        Assert.Equal([SkeletonAlign.Start, SkeletonAlign.End, SkeletonAlign.Start, SkeletonAlign.End, SkeletonAlign.Start, SkeletonAlign.End], sides);
    }

    [Fact]
    public void TheProfileHasACentredRoundPictureAndTwoCards()
    {
        IReadOnlyList<SkeletonGroup> profile = SkeletonLayout.Build(SkeletonKind.Profile, 1);

        SkeletonBlock picture = profile[0].Blocks[0];
        Assert.True(picture.Circle);
        Assert.Equal(SkeletonAlign.Center, picture.Align);
        Assert.Equal(2, profile.Count(g => g.InCard));
    }

    [Fact]
    public void ThePlaceholderIsNeverTheSameColourAsTheSurfaceItSitsOn()
    {
        Assert.NotEqual("#FFFFFF", SkeletonLayout.FillHex(dark: false));
        Assert.NotEqual("#1E1E1E", SkeletonLayout.FillHex(dark: true));
        Assert.True(ColorContrast.Ratio(SkeletonLayout.FillHex(false), "#FFFFFF") is > 1.05 and < 1.4);
        Assert.True(ColorContrast.Ratio(SkeletonLayout.FillHex(true), "#1E1E1E") is > 1.05 and < 1.4);
    }
}
