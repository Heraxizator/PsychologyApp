using PsychologyApp.Domain.Colour;
using Xunit;

namespace PsychologyApp.Domain.Tests.Colour;

public sealed class LuscherInterpretationBandsTests
{
    [Theory]
    [InlineData(0, LuscherCoBand.Stable)]
    [InlineData(5, LuscherCoBand.Stable)]
    [InlineData(6, LuscherCoBand.MildTension)]
    [InlineData(11, LuscherCoBand.MildTension)]
    [InlineData(12, LuscherCoBand.ModerateTension)]
    [InlineData(16, LuscherCoBand.ModerateTension)]
    [InlineData(17, LuscherCoBand.ElevatedTension)]
    [InlineData(22, LuscherCoBand.ElevatedTension)]
    [InlineData(23, LuscherCoBand.HighTension)]
    public void ResolveCo_ReturnsExpectedBand(int coValue, LuscherCoBand expected)
    {
        Assert.Equal(expected, LuscherInterpretationBands.ResolveCo(coValue));
    }

    [Theory]
    [InlineData(0.0, LuscherBkBand.Exhausted)]
    [InlineData(0.4, LuscherBkBand.Exhausted)]
    [InlineData(0.41, LuscherBkBand.Conserving)]
    [InlineData(0.8, LuscherBkBand.Conserving)]
    [InlineData(0.81, LuscherBkBand.Optimal)]
    [InlineData(1.9, LuscherBkBand.Optimal)]
    [InlineData(1.91, LuscherBkBand.Overaroused)]
    public void ResolveBk_ReturnsExpectedBand(double bkValue, LuscherBkBand expected)
    {
        Assert.Equal(expected, LuscherInterpretationBands.ResolveBk(bkValue));
    }
}
