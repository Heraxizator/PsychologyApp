using PsychologyApp.Presentation.Modules.Practice.Techniques;

namespace PsychologyApp.Presentation.Infrastructure;

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
        "anxiety" => TechniqueId.Spin,
        "body" => TechniqueId.Experience,
        "mood" => TechniqueId.Paper,
        "explore" => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length],
        _ => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length]
    };
}
