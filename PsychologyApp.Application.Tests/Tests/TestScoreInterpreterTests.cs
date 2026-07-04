using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class TestScoreInterpreterTests
{
    [Theory]
    [InlineData("beck", 5, 0)]
    [InlineData("beck", 12, 1)]
    [InlineData("gad7", 12, 2)]
    [InlineData("who5", 20, 2)]
    [InlineData("pss10", 10, 0)]
    [InlineData("pss10", 20, 1)]
    [InlineData("phq2", 4, 1)]
    [InlineData("hads_a", 9, 1)]
    [InlineData("rses", 28, 2)]
    public void GetBandIndex_MatchesThresholds(string analyzerId, int score, int expectedBand) =>
        Assert.Equal(expectedBand, TestScoreInterpreter.GetBandIndex(analyzerId, score));

    [Theory]
    [InlineData("beck", true)]
    [InlineData("pss10", true)]
    [InlineData("rses", true)]
    [InlineData("unknown", false)]
    public void IsKnownAnalyzer_RecognizesCatalogAnalyzers(string analyzerId, bool expected) =>
        Assert.Equal(expected, TestScoreInterpreter.IsKnownAnalyzer(analyzerId));
}
