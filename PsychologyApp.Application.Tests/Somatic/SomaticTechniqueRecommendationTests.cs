using PsychologyApp.Application.Models;
using PsychologyApp.Application.Somatic;
using Xunit;

namespace PsychologyApp.Application.Tests.Somatic;

public sealed class SomaticTechniqueRecommendationTests
{
    [Theory]
    [InlineData("headache", TechniqueId.Spin)]
    [InlineData("back pain", TechniqueId.Resize)]
    [InlineData("heart", TechniqueId.Breathing)]
    [InlineData("тревога", TechniqueId.Breathing)]
    [InlineData("усталость", TechniqueId.SmallStep)]
    public void RecommendForQuery_MapsKeywords(string query, TechniqueId expectedFirst)
    {
        IReadOnlyList<TechniqueId> techniques = SomaticTechniqueRecommendation.RecommendForQuery(query);

        Assert.NotEmpty(techniques);
        Assert.Equal(expectedFirst, techniques[0]);
    }

    [Fact]
    public void RecommendForQuery_EmptyQuery_ReturnsDefaults()
    {
        IReadOnlyList<TechniqueId> techniques = SomaticTechniqueRecommendation.RecommendForQuery("");

        Assert.Equal([TechniqueId.Breathing, TechniqueId.Grounding], techniques);
    }
}
