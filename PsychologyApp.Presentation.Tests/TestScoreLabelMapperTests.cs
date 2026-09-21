using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class TestScoreLabelMapperTests
{
    public TestScoreLabelMapperTests()
    {
        AppStrings.LanguageOverride = AppStrings.DefaultLanguage;
    }

    [Fact]
    public void GetSummary_Beck_UsesDomainBands()
    {
        string? low = TestScoreLabelMapper.GetSummary("beck", 9);
        string? mild = TestScoreLabelMapper.GetSummary("beck", 10);

        Assert.Equal(AppStrings.BeckScore(TestScoreBandClassifier.ClassifyBeck(9)), low);
        Assert.Equal(AppStrings.BeckScore(TestScoreBandClassifier.ClassifyBeck(10)), mild);
        Assert.NotEqual(low, mild);
    }

    [Fact]
    public void GetSummary_UnknownAnalyzer_ReturnsNull()
    {
        Assert.Null(TestScoreLabelMapper.GetSummary("unknown", 5));
    }

    [Fact]
    public void TestLastResultDated_IncludesDateAndSummary()
    {
        string text = AppStrings.TestLastResultDated("mild", "26.07.2026");
        Assert.Contains("mild", text, StringComparison.Ordinal);
        Assert.Contains("26.07.2026", text, StringComparison.Ordinal);
    }
}
