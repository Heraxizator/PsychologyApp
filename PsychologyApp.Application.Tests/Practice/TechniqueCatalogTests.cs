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
    public async Task GetAsync_returns_expected_ui_kind_and_page_name(TechniqueId id, TechniqueUiKind kind, string pageName)
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(id);

        Assert.Equal(kind, definition.UiKind);
        Assert.Equal(pageName, definition.PageName);
        Assert.Equal("Практик", definition.ModuleName);
    }

    [Fact]
    public async Task GetAllAsync_contains_fourteen_builtin_techniques()
    {
        IReadOnlyList<BuiltInTechniqueDefinition> all = await CreateCatalog().GetAllAsync();
        Assert.Equal(14, all.Count);
    }

    [Fact]
    public async Task GetBuiltInListEntriesAsync_aligns_with_catalog()
    {
        ITechniqueCatalogService catalog = CreateCatalog();
        IReadOnlyList<BuiltInTechniqueDefinition> all = await catalog.GetAllAsync();
        IReadOnlyList<TechniqueListEntry> entries = await catalog.GetBuiltInListEntriesAsync();

        Assert.Equal(all.Count, entries.Count);

        foreach (TechniqueListEntry entry in entries)
        {
            BuiltInTechniqueDefinition definition = await catalog.GetAsync(entry.TechniqueId);
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
    public async Task All_builtin_techniques_have_theory_sections()
    {
        foreach (BuiltInTechniqueDefinition definition in await CreateCatalog().GetAllAsync())
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
    public async Task Entry_techniques_include_rating_fields(TechniqueId id, EntryFieldKind expectedKind)
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(id);
        Assert.Contains(definition.Entries!, entry => entry.Kind == expectedKind);
    }

    [Fact]
    public async Task Comparison_includes_reflection_field()
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(TechniqueId.Comparison);
        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("изменилось", StringComparison.OrdinalIgnoreCase));
    }
}
