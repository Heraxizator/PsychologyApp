using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.ValueObjects;
using Xunit;

namespace PsychologyApp.Domain.Tests.Colour;

public sealed class LuscherScoringTests
{
    [Fact]
    public void CalculateCo_WithEightSelections_ReturnsNonNegativeValue()
    {
        var selections = BuildStandardSelectionOrder();

        int co = LuscherScoring.CalculateCo(selections);

        Assert.True(co >= 0);
    }

    [Fact]
    public void CalculateBk_WithEightSelections_ReturnsPositiveValue()
    {
        var selections = BuildStandardSelectionOrder();

        double bk = LuscherScoring.CalculateBk(selections);

        Assert.True(bk > 0);
    }

    [Fact]
    public void CalculateBk_WhenDenominatorIsZero_ReturnsZero()
    {
        var selections = BuildDenominatorZeroSelections();

        double bk = LuscherScoring.CalculateBk(selections);

        Assert.Equal(0, bk);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(25)]
    public void InterpretCo_ReturnsNonEmptyText(int coValue)
    {
        Assert.False(string.IsNullOrWhiteSpace(LuscherScoring.InterpretCo(coValue)));
    }

    [Theory]
    [InlineData(0.2)]
    [InlineData(0.6)]
    [InlineData(2.5)]
    public void InterpretBk_ReturnsNonEmptyText(double bkValue)
    {
        Assert.False(string.IsNullOrWhiteSpace(LuscherScoring.InterpretBk(bkValue)));
    }

    private static List<(ColourValue, ColourMeaning)> BuildStandardSelectionOrder() =>
    [
        (ColourValue.Blue, ColourMeaning.BlueVoted),
        (ColourValue.Green, ColourMeaning.GreenVoted),
        (ColourValue.Red, ColourMeaning.RedVoted),
        (ColourValue.Yellow, ColourMeaning.YellowVoted),
        (ColourValue.Purple, ColourMeaning.PurpleVoted),
        (ColourValue.Brown, ColourMeaning.BrownVoted),
        (ColourValue.Black, ColourMeaning.BlackVoted),
        (ColourValue.Gray, ColourMeaning.GrayVoted)
    ];

    private static List<(ColourValue, ColourMeaning)> BuildDenominatorZeroSelections()
    {
        List<(ColourValue, ColourMeaning)> selections = [];
        for (int index = 0; index < 8; index++)
        {
            selections.Add((ColourValue.Purple, ColourMeaning.PurpleVoted));
        }

        selections.Add((ColourValue.Green, ColourMeaning.GreenVoted));
        selections.Add((ColourValue.Brown, ColourMeaning.BrownVoted));
        selections.Add((ColourValue.Blue, ColourMeaning.BlueVoted));
        return selections;
    }
}
