using PsychologyApp.Presentation.Modules.Practice.Techniques;

namespace PsychologyApp.Presentation.Infrastructure;

public static class OnboardingRecommendation
{
    public static TechniqueId ResolveTechnique(string concern) => concern switch
    {
        "anxiety" => TechniqueId.Spin,
        "body" => TechniqueId.Experience,
        "mood" => TechniqueId.Paper,
        _ => TechniqueId.Spin
    };
}
