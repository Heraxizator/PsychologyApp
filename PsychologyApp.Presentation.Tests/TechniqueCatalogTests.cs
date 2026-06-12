using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.UI.Components;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class TechniqueCatalogTests
{
    [Theory]
    [InlineData(TechniqueId.Spin, TechniqueUiKind.Entry, "Крутилка")]
    [InlineData(TechniqueId.Polarity, TechniqueUiKind.Polarity, "Полярности")]
    [InlineData(TechniqueId.Paper, TechniqueUiKind.Paper, "Лист бумаги")]
    [InlineData(TechniqueId.Hack, TechniqueUiKind.Entry, "Белое пятно")]
    [InlineData(TechniqueId.Copied, TechniqueUiKind.Copied, "Повтори это")]
    [InlineData(TechniqueId.Observer, TechniqueUiKind.Entry, "Позиция наблюдателя")]
    [InlineData(TechniqueId.Anchor, TechniqueUiKind.Entry, "Якорь ресурса")]
    [InlineData(TechniqueId.Grounding, TechniqueUiKind.Entry, "Заземление 5-4-3-2-1")]
    public void Get_returns_expected_ui_kind_and_page_name(TechniqueId id, TechniqueUiKind kind, string pageName)
    {
        TechniqueDefinition definition = TechniqueCatalog.Get(id);

        Assert.Equal(kind, definition.UiKind);
        Assert.Equal(pageName, definition.PageName);
        Assert.Equal("Практик", definition.ModuleName);
    }

    [Fact]
    public void All_contains_fourteen_builtin_techniques() =>
        Assert.Equal(14, TechniqueCatalog.All.Count);

    [Fact]
    public void ListCatalog_entries_align_with_catalog()
    {
        Assert.Equal(TechniqueCatalog.All.Count, TechniqueListCatalog.GetBuiltIn().Count);

        foreach (TechniqueListEntry entry in TechniqueListCatalog.GetBuiltIn())
        {
            TechniqueDefinition definition = TechniqueCatalog.Get(entry.TechniqueId);
            Assert.Equal(definition.ListTitle, entry.Title);
            Assert.Equal(definition.ListSubtitle, entry.Subtitle);
            Assert.Equal(definition.ListNumber, entry.Number);
            Assert.False(string.IsNullOrWhiteSpace(definition.ListIcon));
            Assert.True(definition.ListDurationMinutes > 0);
            Assert.Equal(definition.ListIcon, entry.Icon);
            Assert.Equal(definition.ListDurationMinutes, entry.DurationMinutes);
        }
    }

    [Fact]
    public void All_builtin_techniques_have_theory_sections()
    {
        foreach (TechniqueDefinition definition in TechniqueCatalog.All)
        {
            Assert.NotNull(definition.TheorySections);
            Assert.Equal(4, definition.TheorySections!.Count);
        }
    }

    [Theory]
    [InlineData(TechniqueId.Spin, EntryFieldKind.Rating0To10)]
    [InlineData(TechniqueId.Future, EntryFieldKind.Rating0To10)]
    [InlineData(TechniqueId.Experience, EntryFieldKind.RatingNeg10To10)]
    [InlineData(TechniqueId.Observer, EntryFieldKind.Rating0To10)]
    [InlineData(TechniqueId.Anchor, EntryFieldKind.Rating0To10)]
    public void Entry_techniques_include_rating_fields(TechniqueId id, EntryFieldKind expectedKind)
    {
        TechniqueDefinition definition = TechniqueCatalog.Get(id);
        Assert.Contains(definition.Entries!, entry => entry.Kind == expectedKind);
    }

    [Fact]
    public void Comparison_includes_reflection_field()
    {
        TechniqueDefinition definition = TechniqueCatalog.Get(TechniqueId.Comparison);
        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("изменилось", StringComparison.OrdinalIgnoreCase));
    }
}
