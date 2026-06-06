using PsychologyApp.Presentation.Modules.Practice.Techniques;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class TechniqueCatalogTests
{
    [Theory]
    [InlineData(TechniqueId.Spin, TechniqueUiKind.Entry, "Крутилка")]
    [InlineData(TechniqueId.Polarity, TechniqueUiKind.Polarity, "Полярности")]
    [InlineData(TechniqueId.Paper, TechniqueUiKind.Paper, "Лист бумаги")]
    [InlineData(TechniqueId.Hack, TechniqueUiKind.None, "Белое пятно")]
    [InlineData(TechniqueId.Copied, TechniqueUiKind.Copied, "Повтори это")]
    public void Get_returns_expected_ui_kind_and_page_name(TechniqueId id, TechniqueUiKind kind, string pageName)
    {
        TechniqueDefinition definition = TechniqueCatalog.Get(id);

        Assert.Equal(kind, definition.UiKind);
        Assert.Equal(pageName, definition.PageName);
        Assert.Equal("Практик", definition.ModuleName);
    }

    [Fact]
    public void All_contains_eleven_builtin_techniques() =>
        Assert.Equal(11, TechniqueCatalog.All.Count);

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
        }
    }
}
