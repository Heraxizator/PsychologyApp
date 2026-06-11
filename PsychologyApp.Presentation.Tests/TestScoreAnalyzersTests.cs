using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
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
