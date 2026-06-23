using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Shared.Common;

public static class OnboardingRecommendation
{
    private static readonly TechniqueId[] ExploreRotation =
    [
        TechniqueId.Spin,
        TechniqueId.Paper,
        TechniqueId.Experience
    ];

    public static TechniqueId ResolveTechnique(string concern) => concern switch
    {
        OnboardingConcernKeys.Anxiety => TechniqueId.Spin,
        OnboardingConcernKeys.Body => TechniqueId.Experience,
        OnboardingConcernKeys.Mood => TechniqueId.Paper,
        OnboardingConcernKeys.Explore => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length],
        _ => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length]
    };
}
