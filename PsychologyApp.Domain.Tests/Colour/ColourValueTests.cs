using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Domain.Exceptions;
using Xunit;

namespace PsychologyApp.Domain.Tests.Colour;

public sealed class ColourValueTests
{
    [Theory]
    [InlineData("#000000")]
    [InlineData("#FF0000")]
    [InlineData("#964B00")]
    [InlineData("#FFFF00")]
    [InlineData("#00FF00")]
    [InlineData("#0000FF")]
    [InlineData("#FF00FF")]
    [InlineData("#888888")]
    public void From_AcceptsSupportedColourCodes(string code)
    {
        ColourValue colour = ColourValue.From(code);

        Assert.Equal(code, colour.Code);
    }

    [Fact]
    public void From_ThrowsForUnsupportedCode()
    {
        Assert.Throws<UnsupportedColourValueException>(() => ColourValue.From("#FFFFFF"));
    }

    [Fact]
    public void WellKnownInstances_HaveExpectedCodes()
    {
        Assert.Equal("#000000", ColourValue.Black.Code);
        Assert.Equal("#FF0000", ColourValue.Red.Code);
        Assert.Equal("#00FF00", ColourValue.Green.Code);
    }

    [Fact]
    public void Equals_ComparesByCode()
    {
        ColourValue left = ColourValue.From("#FF0000");
        ColourValue right = ColourValue.Red;

        Assert.Equal(left, right);
    }

    [Fact]
    public void ImplicitConversion_ToStringReturnsCode()
    {
        string code = ColourValue.Blue;

        Assert.Equal("#0000FF", code);
    }
}
