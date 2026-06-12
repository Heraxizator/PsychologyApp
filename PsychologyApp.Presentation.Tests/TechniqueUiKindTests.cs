using PsychologyApp.Presentation.Models.Practice.Techniques;
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
    [InlineData(TechniqueId.Observer, TechniqueUiKind.Entry)]
    [InlineData(TechniqueId.Anchor, TechniqueUiKind.Entry)]
    [InlineData(TechniqueId.Grounding, TechniqueUiKind.Entry)]
    public void Every_technique_has_expected_ui_kind(TechniqueId id, TechniqueUiKind expected) =>
        Assert.Equal(expected, TechniqueCatalog.Get(id).UiKind);
}
