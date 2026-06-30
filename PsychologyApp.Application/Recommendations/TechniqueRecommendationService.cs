using PsychologyApp.Application.Models;
using PsychologyApp.Application.Somatic;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Recommendations;

public interface ITechniqueRecommendationService
{
    TechniqueId ResolveFromOnboardingConcern(string concern);

    IReadOnlyList<TechniqueId> RecommendForSomaticQuery(string query);
}

public sealed class TechniqueRecommendationService : ITechniqueRecommendationService
{
    private static readonly TechniqueId[] ExploreRotation =
    [
        TechniqueId.Spin,
        TechniqueId.Paper,
        TechniqueId.Experience
    ];

    public TechniqueId ResolveFromOnboardingConcern(string concern) => concern switch
    {
        OnboardingConcernKeys.Anxiety => TechniqueId.Spin,
        OnboardingConcernKeys.Body => TechniqueId.Experience,
        OnboardingConcernKeys.Mood => TechniqueId.Paper,
        OnboardingConcernKeys.Explore => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length],
        _ => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length]
    };

    public IReadOnlyList<TechniqueId> RecommendForSomaticQuery(string query) =>
        SomaticTechniqueRecommendation.RecommendForQuery(query);
}
