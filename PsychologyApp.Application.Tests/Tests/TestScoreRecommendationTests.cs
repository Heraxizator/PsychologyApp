using PsychologyApp.Application.Models;
using PsychologyApp.Application.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class TestScoreRecommendationTests
{
    [Theory]
    [InlineData("beck", 10, TechniqueId.Spin)]
    [InlineData("beck", 5, TechniqueId.ThoughtRecord)]
    [InlineData("heck_hess", 25, TechniqueId.Polarity)]
    [InlineData("haer", 29, TechniqueId.Future)]
    [InlineData("gad7", 10, TechniqueId.Polarity)]
    [InlineData("gad7", 5, TechniqueId.Breathing)]
    [InlineData("k10", 22, TechniqueId.Spin)]
    [InlineData("k10", 16, TechniqueId.Breathing)]
    [InlineData("k10", 12, TechniqueId.Paper)]
    [InlineData("who5", 12, TechniqueId.SmallStep)]
    [InlineData("who5", 20, TechniqueId.Experience)]
    [InlineData("phq9", 10, TechniqueId.Spin)]
    [InlineData("phq9", 5, TechniqueId.SmallStep)]
    [InlineData("isi", 22, TechniqueId.Spin)]
    [InlineData("isi", 15, TechniqueId.Breathing)]
    [InlineData("isi", 10, TechniqueId.Paper)]
    [InlineData("ess", 16, TechniqueId.Spin)]
    [InlineData("ess", 11, TechniqueId.Breathing)]
    [InlineData("ess", 8, TechniqueId.Paper)]
    [InlineData("phq15", 15, TechniqueId.Spin)]
    [InlineData("phq15", 10, TechniqueId.Experience)]
    [InlineData("phq15", 5, TechniqueId.Paper)]
    [InlineData("scoff", 2, TechniqueId.Spin)]
    [InlineData("scoff", 1, TechniqueId.SelfCompassion)]
    [InlineData("swls", 20, TechniqueId.SmallStep)]
    [InlineData("swls", 25, TechniqueId.Experience)]
    [InlineData("pss10", 30, TechniqueId.Spin)]
    [InlineData("pss10", 20, TechniqueId.Breathing)]
    [InlineData("pss10", 10, TechniqueId.Paper)]
    [InlineData("phq2", 4, TechniqueId.SmallStep)]
    [InlineData("phq2", 1, TechniqueId.Paper)]
    [InlineData("gad2", 4, TechniqueId.Breathing)]
    [InlineData("gad2", 1, TechniqueId.Grounding)]
    [InlineData("hads_a", 12, TechniqueId.Spin)]
    [InlineData("hads_a", 9, TechniqueId.Breathing)]
    [InlineData("hads_a", 5, TechniqueId.Grounding)]
    [InlineData("hads_d", 12, TechniqueId.SmallStep)]
    [InlineData("hads_d", 9, TechniqueId.Paper)]
    [InlineData("hads_d", 5, TechniqueId.ThoughtRecord)]
    [InlineData("rses", 10, TechniqueId.SelfCompassion)]
    [InlineData("rses", 20, TechniqueId.SmallStep)]
    [InlineData("rses", 28, TechniqueId.Experience)]
    public void RecommendTechnique_ReturnsExpected(string analyzerId, int score, TechniqueId expected)
    {
        TechniqueId? result = TestScoreRecommendation.RecommendTechnique(analyzerId, score);

        Assert.Equal(expected, result);
    }
}
