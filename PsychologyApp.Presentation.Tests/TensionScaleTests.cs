using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The tension scale: whole steps, a colour that follows the value, words only at the ends.</summary>
public class TensionScaleTests
{
    [Theory]
    [InlineData(0.2, 0)]
    [InlineData(4.5, 5)]
    [InlineData(7.49, 7)]
    [InlineData(-3, 0)]
    [InlineData(42, 10)]
    public void AValueSnapsToTheNearestWholeStepInsideTheScale(double value, int expected) => Assert.Equal(expected, TensionScale.Snap(value));

    [Fact]
    public void TheColourRunsFromCalmThroughAmberToRed()
    {
        Assert.Equal((0x4F, 0xB6, 0xA6), Tuple(TensionScale.ColorAt(0)));
        Assert.Equal((0xE8, 0xA2, 0x5A), Tuple(TensionScale.ColorAt(5)));
        Assert.Equal((0xD9, 0x5A, 0x4E), Tuple(TensionScale.ColorAt(10)));
    }

    [Fact]
    public void EveryStepHasItsOwnColour()
    {
        int distinct = Enumerable.Range(0, 11).Select(v => TensionScale.ColorAt(v)).Distinct().Count();

        Assert.Equal(11, distinct);
    }

    [Theory]
    [InlineData(0, TensionWord.Calm)]
    [InlineData(2, TensionWord.Calm)]
    [InlineData(3, TensionWord.None)]
    [InlineData(7, TensionWord.None)]
    [InlineData(8, TensionWord.Strong)]
    [InlineData(10, TensionWord.Strong)]
    public void AWordIsShownOnlyAtTheEnds(int value, TensionWord expected) => Assert.Equal(expected, TensionScale.WordAt(value));

    private static (int, int, int) Tuple((byte R, byte G, byte B) c) => (c.R, c.G, c.B);
}
