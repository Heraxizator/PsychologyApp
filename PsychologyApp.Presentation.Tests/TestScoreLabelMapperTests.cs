using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.RunTests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestScoreLabelMapperTests
{
    public TestScoreLabelMapperTests()
    {
        UserPreferences.SetLanguage("ru");
    }

    [Theory]
    [InlineData("beck", 0)]
    [InlineData("heck_hess", 1)]
    [InlineData("gad7", 1)]
    [InlineData("who5", 1)]
    [InlineData("phq9", 1)]
    [InlineData("isi", 1)]
    [InlineData("ess", 1)]
    [InlineData("phq15", 1)]
    [InlineData("scoff", 1)]
    [InlineData("swls", 1)]
    public void GetSummary_ReturnsLocalizedText(string id, int score)
    {
        string? summary = TestScoreLabelMapper.GetSummary(id, score);
        Assert.False(string.IsNullOrWhiteSpace(summary));
    }

    [Fact]
    public void GetSummary_ReturnsNull_ForUnknownAnalyzer() =>
        Assert.Null(TestScoreLabelMapper.GetSummary("unknown", 5));
}
