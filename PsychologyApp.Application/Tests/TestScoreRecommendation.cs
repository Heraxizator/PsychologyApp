using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.TestScoring;

public static class TestScoreRecommendation
{
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
