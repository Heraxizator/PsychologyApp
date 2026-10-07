using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The geometry of a screen that grows out of the card that was tapped.</summary>
public class ZoomOriginTests
{
    [Fact]
    public void TheScreenStartsOverTheCardAtTheCardsSize()
    {
        (double scale, double x, double y)? start = ZoomOrigin.Start(20, 400, 340, 120, 400, 800);

        Assert.NotNull(start);
        Assert.Equal(0.85, start!.Value.scale, 6);
        Assert.Equal(-10, start.Value.x, 6);
        Assert.Equal(60, start.Value.y, 6);
    }

    [Fact]
    public void ASmallCardNeverShrinksTheScreenBelowTheFloor()
    {
        var start = ZoomOrigin.Start(10, 10, 20, 20, 400, 800);

        Assert.Equal(ZoomOrigin.MinScale, start!.Value.Scale);
    }

    [Theory]
    [InlineData(0, 0, 100, 100, 0, 800)]
    [InlineData(0, 0, 0, 100, 400, 800)]
    [InlineData(-500, 0, 100, 100, 400, 800)]
    [InlineData(0, 2000, 100, 100, 400, 800)]
    public void ACardThatCannotBePlacedGivesNoStart(double x, double y, double w, double h, double pw, double ph) =>
        Assert.Null(ZoomOrigin.Start(x, y, w, h, pw, ph));

    [Fact]
    public void OnlyARecentTapOpenedTheScreen()
    {
        DateTimeOffset now = new(2026, 10, 7, 12, 0, 0, TimeSpan.Zero);

        Assert.True(ZoomOrigin.IsFresh(now.AddMilliseconds(-300), now));
        Assert.False(ZoomOrigin.IsFresh(now.AddSeconds(-5), now));
        Assert.False(ZoomOrigin.IsFresh(now.AddSeconds(1), now));
    }
}
