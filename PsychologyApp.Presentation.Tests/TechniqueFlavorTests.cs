using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>Each built-in practice belongs to a group with its own colours, readable in both themes.</summary>
public class TechniqueFlavorTests
{
    [Fact]
    public void EveryBuiltInPracticeBelongsToAGroup()
    {
        foreach (TechniqueId id in Enum.GetValues<TechniqueId>())
        {
            Assert.NotEqual(TechniqueFlavor.None, TechniqueFlavors.For(id));
        }
    }

    [Fact]
    public void TheBreathingPracticesAreForTheBodyAndTheThoughtRecordIsForTheMind()
    {
        Assert.Equal(TechniqueFlavor.Body, TechniqueFlavors.For(TechniqueId.Breathing));
        Assert.Equal(TechniqueFlavor.Body, TechniqueFlavors.For(TechniqueId.Grounding));
        Assert.Equal(TechniqueFlavor.Mind, TechniqueFlavors.For(TechniqueId.ThoughtRecord));
        Assert.Equal(TechniqueFlavor.Heart, TechniqueFlavors.For(TechniqueId.SelfCompassion));
        Assert.Equal(TechniqueFlavor.Action, TechniqueFlavors.For(TechniqueId.SmallStep));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TheIconIsReadableOnItsTileInEveryGroup(bool dark)
    {
        foreach (TechniqueFlavor flavor in new[] { TechniqueFlavor.Body, TechniqueFlavor.Mind, TechniqueFlavor.Heart, TechniqueFlavor.Action })
        {
            (string tile, string icon) = TechniqueFlavors.Colors(flavor, dark)!.Value;
            double ratio = ColorContrast.Ratio(icon, tile);
            Assert.True(ratio >= ColorContrast.LargeTextMinimum, $"{flavor}, dark={dark}: {ratio:F2}");
        }
    }

    [Theory]
    [InlineData(false, "#FFFFFF")]
    [InlineData(true, "#1E1E1E")]
    public void TheTileIsVisibleAgainstTheCardAndGroupsDiffer(bool dark, string card)
    {
        string[] tiles = [.. new[] { TechniqueFlavor.Body, TechniqueFlavor.Mind, TechniqueFlavor.Heart, TechniqueFlavor.Action }
            .Select(f => TechniqueFlavors.Colors(f, dark)!.Value.Tile)];

        Assert.Equal(4, tiles.Distinct().Count());
        Assert.All(tiles, t => Assert.NotEqual(card, t));
    }

    [Fact]
    public void APracticeWithoutAGroupKeepsTheUsualAccent()
    {
        Assert.Null(TechniqueFlavors.Colors(TechniqueFlavor.None, false));
        Assert.Equal(TechniqueFlavor.None, TechniqueFlavors.Parse(null));
        Assert.Equal(TechniqueFlavor.Heart, TechniqueFlavors.Parse("Heart"));
    }
}
