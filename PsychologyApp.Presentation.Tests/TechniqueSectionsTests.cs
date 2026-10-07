using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The catalog split into sections by group: fixed order, one heading each, the filter keeps one, never an empty list.</summary>
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
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build(Items, "All", Title);

        Assert.Equal(["BODY", "MIND", "HEART", "ACTION"], sections.Select(s => s.Title));
        Assert.Equal(["Breathing", "Grounding"], sections[0].Select(i => i.Title));
        Assert.Equal(["Spin", "Thought record"], sections[1].Select(i => i.Title));
        Assert.All(sections, s => Assert.False(s.IsCustom));
    }

    [Fact]
    public void AChosenGroupKeepsOnlyItsSection()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build(Items, "Heart", Title);

        TechniqueGroup only = Assert.Single(sections);
        Assert.Equal("HEART", only.Title);
        Assert.Equal(["Compassion"], only.Select(i => i.Title));
    }

    [Fact]
    public void EveryPracticeAppearsExactlyOnceWhateverTheFilter()
    {
        foreach (string key in TechniqueFlavors.FilterKeys)
        {
            IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build(Items, key, Title);
            TechniqueItem[] shown = [.. sections.SelectMany(s => s)];

            Assert.Equal(shown.Length, shown.Distinct().Count());
            Assert.All(shown, item => Assert.Contains(item, Items));
        }

        Assert.Equal(Items.Length, TechniqueSections.Build(Items, "All", Title).SelectMany(s => s).Count());
    }

    [Fact]
    public void AFilterThatWouldLeaveNothingShowsEverything()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build([Item("Spin", "Mind")], "Action", Title);

        Assert.Equal(["Spin"], sections.SelectMany(s => s).Select(i => i.Title));
    }

    [Fact]
    public void APracticeWithoutAGroupStaysVisibleUnderNoHeading()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build([Item("Spin", "Mind"), Item("Mine", null)], "All", Title);

        Assert.Equal(2, sections.Count);
        Assert.Equal(string.Empty, sections[1].Title);
        Assert.False(sections[1].HasTitle);
    }

    [Fact]
    public void NoPracticesGiveNoSections()
    {
        Assert.Empty(TechniqueSections.Build([], "All", Title));
    }
}
