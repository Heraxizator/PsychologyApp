using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Practice;

public class TechniqueCatalogTests
{
    private static ITechniqueCatalogService CreateCatalog() =>
        new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => "ru"));

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
        BuiltInTechniqueDefinition definition = CreateCatalog().Get(id);

        Assert.Equal(kind, definition.UiKind);
        Assert.Equal(pageName, definition.PageName);
        Assert.Equal("Практик", definition.ModuleName);
    }

    [Fact]
    public void GetAll_contains_fourteen_builtin_techniques() =>
        Assert.Equal(14, CreateCatalog().GetAll().Count);

    [Fact]
    public void List_entries_align_with_catalog()
    {
        ITechniqueCatalogService catalog = CreateCatalog();
        Assert.Equal(catalog.GetAll().Count, catalog.GetBuiltInListEntries().Count);

        foreach (TechniqueListEntry entry in catalog.GetBuiltInListEntries())
        {
            BuiltInTechniqueDefinition definition = catalog.Get(entry.TechniqueId);
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
        foreach (BuiltInTechniqueDefinition definition in CreateCatalog().GetAll())
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
        BuiltInTechniqueDefinition definition = CreateCatalog().Get(id);
        Assert.Contains(definition.Entries!, entry => entry.Kind == expectedKind);
    }

    [Fact]
    public void Comparison_includes_reflection_field()
    {
        BuiltInTechniqueDefinition definition = CreateCatalog().Get(TechniqueId.Comparison);
        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("изменилось", StringComparison.OrdinalIgnoreCase));
    }
}
