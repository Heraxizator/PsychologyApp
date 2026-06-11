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
    public void RecommendTechnique_ReturnsExpected(string analyzerId, int score, TechniqueId expected)
    {
        TechniqueId? result = TestScoreRecommendation.RecommendTechnique(analyzerId, score);

        Assert.Equal(expected, result);
    }
}
