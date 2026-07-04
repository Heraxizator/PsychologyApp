using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class LuscherScoreRecommendationTests
{
    [Theory]
    [InlineData(25, TechniqueId.Spin)]
    [InlineData(20, TechniqueId.Breathing)]
    [InlineData(15, TechniqueId.Grounding)]
    [InlineData(5, TechniqueId.Experience)]
    public void RecommendTechnique_ReturnsExpected(int coValue, TechniqueId expected) =>
        Assert.Equal(expected, LuscherScoreRecommendation.RecommendTechnique(coValue));
}
