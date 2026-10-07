using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The catalog split into sections by group: fixed order, one heading each, no practice lost.</summary>
public class TechniqueSectionsTests
{
    private static TechniqueItem Item(string title, string? flavor) => new() { Title = title, Flavor = flavor, Active = true };

    private static readonly TechniqueItem[] Items =
    [
        Item("Spin", "Mind"), Item("Breathing", "Body"), Item("Compassion", "Heart"), Item("Small step", "Action"),
        Item("Grounding", "Body"), Item("Thought record", "Mind")
    ];

    private static string Title(TechniqueFlavor flavor) => flavor.ToString().ToUpperInvariant();

    [Fact]
    public void TheSectionsComeInAFixedOrderWithTheirHeadingsAndOnlyTheirOwnPractices()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build(Items, Title);

        Assert.Equal(["BODY", "MIND", "HEART", "ACTION"], sections.Select(s => s.Title));
        Assert.Equal(["Breathing", "Grounding"], sections[0].Select(i => i.Title));
        Assert.Equal(["Spin", "Thought record"], sections[1].Select(i => i.Title));
        Assert.All(sections, s => Assert.False(s.IsCustom));
    }

    [Fact]
    public void EveryPracticeAppearsExactlyOnce()
    {
        TechniqueItem[] shown = [.. TechniqueSections.Build(Items, Title).SelectMany(s => s)];

        Assert.Equal(Items.Length, shown.Length);
        Assert.Equal(shown.Length, shown.Distinct().Count());
    }

    [Fact]
    public void ASectionWithoutPracticesIsLeftOut()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build([Item("Spin", "Mind")], Title);

        Assert.Equal(["MIND"], sections.Select(s => s.Title));
    }

    [Fact]
    public void APracticeWithoutAGroupStaysVisibleUnderNoHeading()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build([Item("Spin", "Mind"), Item("Mine", null)], Title);

        Assert.Equal(2, sections.Count);
        Assert.Equal(string.Empty, sections[1].Title);
        Assert.False(sections[1].HasTitle);
    }

    [Fact]
    public void NoPracticesGiveNoSections()
    {
        Assert.Empty(TechniqueSections.Build([], Title));
    }
}
