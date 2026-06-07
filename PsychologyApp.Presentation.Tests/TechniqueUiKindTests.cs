using PsychologyApp.Presentation.Modules.Practice.Techniques;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class TechniqueUiKindTests
{
    [Theory]
    [InlineData(TechniqueId.Spin, TechniqueUiKind.Entry)]
    [InlineData(TechniqueId.Hack, TechniqueUiKind.Entry)]
    [InlineData(TechniqueId.Paper, TechniqueUiKind.Paper)]
    [InlineData(TechniqueId.Polarity, TechniqueUiKind.Polarity)]
    [InlineData(TechniqueId.Copied, TechniqueUiKind.Copied)]
    public void Every_technique_has_expected_ui_kind(TechniqueId id, TechniqueUiKind expected) =>
        Assert.Equal(expected, TechniqueCatalog.Get(id).UiKind);
}
