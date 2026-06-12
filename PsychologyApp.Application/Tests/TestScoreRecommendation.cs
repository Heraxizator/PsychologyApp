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
        "gad7" when score >= 10 => TechniqueId.Polarity,
        "gad7" => TechniqueId.Comparison,
        "k10" when score >= 22 => TechniqueId.Spin,
        "k10" when score >= 16 => TechniqueId.Experience,
        "k10" => TechniqueId.Paper,
        "who5" when score <= 12 => TechniqueId.Paper,
        "who5" => TechniqueId.Experience,
        "phq9" when score >= 10 => TechniqueId.Spin,
        "phq9" => TechniqueId.Paper,
        "isi" when score >= 22 => TechniqueId.Spin,
        "isi" when score >= 15 => TechniqueId.Experience,
        "isi" => TechniqueId.Paper,
        "ess" when score >= 16 => TechniqueId.Spin,
        "ess" when score >= 11 => TechniqueId.Experience,
        "ess" => TechniqueId.Paper,
        "phq15" when score >= 15 => TechniqueId.Spin,
        "phq15" when score >= 10 => TechniqueId.Experience,
        "phq15" => TechniqueId.Paper,
        "scoff" when score >= 2 => TechniqueId.Spin,
        "scoff" => TechniqueId.Paper,
        "swls" when score <= 20 => TechniqueId.Paper,
        "swls" => TechniqueId.Experience,
        _ => TechniqueId.Experience
    };
}
