using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Domain.Exceptions;
using Xunit;

namespace PsychologyApp.Domain.Tests.Colour;

public sealed class ColourMeaningTests
{
    [Fact]
    public void From_AcceptsSupportedVotedMeaning()
    {
        ColourMeaning meaning = ColourMeaning.From(ColourMeaning.BlackVoted.Explaination!, ColourMeaningType.Wanted);

        Assert.Equal(ColourMeaningType.Wanted, meaning.ColorType);
        Assert.Equal(ColourMeaning.BlackVoted.Explaination, meaning.Explaination);
    }

    [Fact]
    public void From_ThrowsForUnsupportedText()
    {
        Assert.Throws<UnsupportedColourMeaningException>(() =>
            ColourMeaning.From("unsupported meaning text", ColourMeaningType.Wanted));
    }

    [Fact]
    public void WellKnownVotedMeanings_HaveWantedType()
    {
        Assert.Equal(ColourMeaningType.Wanted, ColourMeaning.RedVoted.ColorType);
        Assert.Equal(ColourMeaningType.Wanted, ColourMeaning.BlueVoted.ColorType);
    }

    [Fact]
    public void WellKnownUnvotedMeanings_HaveUnwantedType()
    {
        Assert.Equal(ColourMeaningType.Unwanted, ColourMeaning.RedUnvoted.ColorType);
        Assert.Equal(ColourMeaningType.Unwanted, ColourMeaning.GreenUnvoted.ColorType);
    }

    [Fact]
    public void ToString_ReturnsExplanation()
    {
        Assert.Equal(ColourMeaning.YellowVoted.Explaination, ColourMeaning.YellowVoted.ToString());
    }
}
