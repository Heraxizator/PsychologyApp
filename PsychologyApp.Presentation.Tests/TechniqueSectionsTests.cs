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

/// <summary>Folded sections: the heading stays, the cards are not in the list.</summary>
public class TechniqueFoldedSectionsTests
{
    private static TechniqueItem Item(string title, string flavor) => new() { Title = title, Flavor = flavor, Active = true };

    private static readonly TechniqueItem[] Items =
    [
        Item("Spin", "Mind"), Item("Breathing", "Body"), Item("Compassion", "Heart"), Item("Small step", "Action"), Item("Grounding", "Body")
    ];

    private static string Title(TechniqueFlavor flavor) => flavor.ToString().ToUpperInvariant();

    private sealed class Command(Action run) : System.Windows.Input.ICommand
    {
        public event EventHandler? CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => run();
    }

    [Fact]
    public void AFoldedSectionKeepsItsHeadingAndCountButHoldsNoCards()
    {
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build(Items, Title, new HashSet<string> { "Body" });

        TechniqueGroup body = sections[0];
        Assert.Equal("Body", body.Key);
        Assert.True(body.IsCollapsed);
        Assert.Empty(body);
        Assert.Equal(2, body.TotalCount);
        Assert.Equal("BODY · 2", body.HeaderText);
        Assert.True(body.HasTitle);
        Assert.Equal(["MIND", "HEART", "ACTION"], sections.Skip(1).Select(s => s.Title));
        Assert.All(sections.Skip(1), s => Assert.False(s.IsCollapsed));
    }

    [Fact]
    public void AnOpenSectionShowsItsPlainHeadingAndEveryCard()
    {
        TechniqueGroup body = TechniqueSections.Build(Items, Title)[0];

        Assert.False(body.IsCollapsed);
        Assert.Equal("BODY", body.HeaderText);
        Assert.Equal(2, body.Count);
        Assert.True(body.CanCollapse);
    }

    [Fact]
    public void TheToggleCommandOfEachSectionKnowsItsKey()
    {
        List<string> pressed = [];
        IReadOnlyList<TechniqueGroup> sections = TechniqueSections.Build(Items, Title, null, key => new Command(() => pressed.Add(key)));

        sections[2].ToggleCommand!.Execute(null);

        Assert.Equal(["Heart"], pressed);
    }

    [Fact]
    public void EverySectionCanBeFoldedAtOnceAndNoneIsLost()
    {
        IReadOnlyList<TechniqueGroup> all = TechniqueSections.Build(Items, Title, new HashSet<string> { "Body", "Mind", "Heart", "Action" });

        Assert.Equal(4, all.Count);
        Assert.All(all, s => Assert.Empty(s));
        Assert.Equal(Items.Length, all.Sum(s => s.TotalCount));
    }
}
