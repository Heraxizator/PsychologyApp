using PsychologyApp.Domain.Colour;
using PsychologyApp.Domain.Colour.ValueObjects;
using Xunit;

namespace PsychologyApp.Domain.Tests.Colour;

public sealed class LuscherScoringTests
{
    [Fact]
    public void CalculateCoBetweenPasses_WithIdenticalPasses_ReturnsZero()
    {
        var pass = BuildStandardSelectionOrder();

        int co = LuscherScoring.CalculateCoBetweenPasses(pass, pass);

        Assert.Equal(0, co);
    }

    [Fact]
    public void CalculateCoBetweenPasses_WithReversedPass_ReturnsThirtyTwo()
    {
        List<(ColourValue, ColourMeaning)> first = BuildStandardSelectionOrder();
        List<(ColourValue, ColourMeaning)> second = first.AsEnumerable().Reverse().ToList();

        int co = LuscherScoring.CalculateCoBetweenPasses(first, second);

        Assert.Equal(32, co);
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
