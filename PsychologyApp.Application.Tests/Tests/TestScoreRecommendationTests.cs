using PsychologyApp.Application.Models;
using PsychologyApp.Application.TestScoring;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class TestScoreRecommendationTests
{
    [Theory]
    [InlineData("beck", 10, TechniqueId.Spin)]
    [InlineData("beck", 5, TechniqueId.Paper)]
    [InlineData("heck_hess", 25, TechniqueId.Polarity)]
    [InlineData("haer", 29, TechniqueId.Future)]
    [InlineData("gad7", 10, TechniqueId.Polarity)]
    [InlineData("gad7", 5, TechniqueId.Comparison)]
    [InlineData("k10", 22, TechniqueId.Spin)]
    [InlineData("k10", 16, TechniqueId.Experience)]
    [InlineData("k10", 12, TechniqueId.Paper)]
    [InlineData("who5", 12, TechniqueId.Paper)]
    [InlineData("who5", 20, TechniqueId.Experience)]
    [InlineData("phq9", 10, TechniqueId.Spin)]
    [InlineData("phq9", 5, TechniqueId.Paper)]
    [InlineData("isi", 22, TechniqueId.Spin)]
    [InlineData("isi", 15, TechniqueId.Experience)]
    [InlineData("isi", 10, TechniqueId.Paper)]
    [InlineData("ess", 16, TechniqueId.Spin)]
    [InlineData("ess", 11, TechniqueId.Experience)]
    [InlineData("ess", 8, TechniqueId.Paper)]
    [InlineData("phq15", 15, TechniqueId.Spin)]
    [InlineData("phq15", 10, TechniqueId.Experience)]
    [InlineData("phq15", 5, TechniqueId.Paper)]
    [InlineData("scoff", 2, TechniqueId.Spin)]
    [InlineData("scoff", 1, TechniqueId.Paper)]
    [InlineData("swls", 20, TechniqueId.Paper)]
    [InlineData("swls", 25, TechniqueId.Experience)]
    public void RecommendTechnique_ReturnsExpected(string analyzerId, int score, TechniqueId expected)
    {
        TechniqueId? result = TestScoreRecommendation.RecommendTechnique(analyzerId, score);

        Assert.Equal(expected, result);
    }
}
