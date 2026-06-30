using PsychologyApp.Application.Models;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Recommendations;

public sealed class TechniqueRecommendationServiceTests
{
    private readonly TechniqueRecommendationService _service = new();

    [Theory]
    [InlineData(OnboardingConcernKeys.Anxiety, TechniqueId.Spin)]
    [InlineData(OnboardingConcernKeys.Body, TechniqueId.Experience)]
    [InlineData(OnboardingConcernKeys.Mood, TechniqueId.Paper)]
    public void ResolveFromOnboardingConcern_MapsKnownConcerns(string concern, TechniqueId expected) =>
        Assert.Equal(expected, _service.ResolveFromOnboardingConcern(concern));

    [Fact]
    public void RecommendForSomaticQuery_ReturnsHeadacheTechniques()
    {
        IReadOnlyList<TechniqueId> techniques = _service.RecommendForSomaticQuery("headache");

        Assert.Contains(TechniqueId.Spin, techniques);
        Assert.Contains(TechniqueId.Paper, techniques);
    }
}
