using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestScoreAnalyzersTests
{
    public TestScoreAnalyzersTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Theory]
    [InlineData("heck_hess", 24, "0-24")]
    [InlineData("heck_hess", 25, "25-40")]
    [InlineData("haer", 29, "0-28")]
    [InlineData("haer", 30, "29-40")]
    [InlineData("pochebut", 10, "0-10")]
    [InlineData("beck", 9, "0-9")]
    [InlineData("beck", 30, "30-63")]
    [InlineData("gad7", 4, "0-4")]
    [InlineData("gad7", 10, "10-14")]
    [InlineData("k10", 15, "10-15")]
    [InlineData("k10", 30, "30-50")]
    [InlineData("who5", 12, "0-12")]
    [InlineData("who5", 20, "19-25")]
    [InlineData("phq9", 4, "0-4")]
    [InlineData("phq9", 10, "10-14")]
    [InlineData("phq9", 20, "20-27")]
    [InlineData("isi", 7, "0-7")]
    [InlineData("isi", 15, "15-21")]
    [InlineData("isi", 22, "22-28")]
    [InlineData("ess", 10, "0-10")]
    [InlineData("ess", 13, "13-15")]
    [InlineData("ess", 16, "16-24")]
    [InlineData("phq15", 4, "0-4")]
    [InlineData("phq15", 10, "10-14")]
    [InlineData("phq15", 20, "15-30")]
    [InlineData("scoff", 1, "0-1")]
    [InlineData("scoff", 2, "2-5")]
    [InlineData("swls", 12, "10-14")]
    [InlineData("swls", 20, "20")]
    [InlineData("swls", 28, "26-30")]
    [InlineData("unknown", 10, null)]
    public void Resolve_ReturnsExpectedPrefix(string id, int score, string? expectedPrefix)
    {
        Func<int, string>? analyzer = TestScoreAnalyzers.Resolve(id);

        if (expectedPrefix is null)
        {
            Assert.Null(analyzer);
            return;
        }

        Assert.NotNull(analyzer);
        Assert.StartsWith(expectedPrefix, analyzer!(score));
    }
}
