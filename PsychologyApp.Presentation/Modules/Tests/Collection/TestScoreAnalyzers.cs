using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Techniques;

namespace PsychologyApp.Presentation.Modules.Tests.Collection;

public static class TestScoreAnalyzers
{
    public static Func<int, string>? Resolve(string analyzerId) => analyzerId switch
    {
        "heck_hess" => AppStrings.HeckHessScore,
        "haer" => AppStrings.HaerScore,
        "pochebut" => AppStrings.PochebutScore,
        "beck" => AppStrings.BeckScore,
        _ => null
    };

    public static TechniqueId? RecommendTechnique(string? analyzerId, int score) => analyzerId switch
    {
        "beck" when score >= 10 => TechniqueId.Spin,
        "beck" => TechniqueId.Paper,
        "heck_hess" when score >= 25 => TechniqueId.Polarity,
        "heck_hess" => TechniqueId.Comparison,
        "pochebut" when score >= 25 => TechniqueId.Resize,
        "pochebut" => TechniqueId.Check,
        "haer" when score >= 29 => TechniqueId.Future,
        _ => TechniqueId.Experience
    };
}
