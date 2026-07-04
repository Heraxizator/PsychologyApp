using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Tests;

public static class TestScoreRecommendation
{
    public static TechniqueId? RecommendTechnique(string? analyzerId, int score) => analyzerId switch
    {
        "beck" when score >= 10 => TechniqueId.Spin,
        "beck" => TechniqueId.ThoughtRecord,
        "heck_hess" when score >= 25 => TechniqueId.Polarity,
        "heck_hess" => TechniqueId.Comparison,
        "pochebut" when score >= 25 => TechniqueId.Resize,
        "pochebut" => TechniqueId.Check,
        "haer" when score >= 29 => TechniqueId.Future,
        "gad7" when score >= 10 => TechniqueId.Polarity,
        "gad7" => TechniqueId.Breathing,
        "k10" when score >= 22 => TechniqueId.Spin,
        "k10" when score >= 16 => TechniqueId.Breathing,
        "k10" => TechniqueId.Paper,
        "who5" when score <= 12 => TechniqueId.SmallStep,
        "who5" => TechniqueId.Experience,
        "phq9" when score >= 10 => TechniqueId.Spin,
        "phq9" => TechniqueId.SmallStep,
        "isi" when score >= 22 => TechniqueId.Spin,
        "isi" when score >= 15 => TechniqueId.Breathing,
        "isi" => TechniqueId.Paper,
        "ess" when score >= 16 => TechniqueId.Spin,
        "ess" when score >= 11 => TechniqueId.Breathing,
        "ess" => TechniqueId.Paper,
        "phq15" when score >= 15 => TechniqueId.Spin,
        "phq15" when score >= 10 => TechniqueId.Experience,
        "phq15" => TechniqueId.Paper,
        "scoff" when score >= 2 => TechniqueId.Spin,
        "scoff" => TechniqueId.SelfCompassion,
        "swls" when score <= 20 => TechniqueId.SmallStep,
        "swls" => TechniqueId.Experience,
        "pss10" when score >= 27 => TechniqueId.Spin,
        "pss10" when score >= 14 => TechniqueId.Breathing,
        "pss10" => TechniqueId.Paper,
        "phq2" when score >= 3 => TechniqueId.SmallStep,
        "phq2" => TechniqueId.Paper,
        "gad2" when score >= 3 => TechniqueId.Breathing,
        "gad2" => TechniqueId.Grounding,
        "hads_a" when score >= 11 => TechniqueId.Spin,
        "hads_a" when score >= 8 => TechniqueId.Breathing,
        "hads_a" => TechniqueId.Grounding,
        "hads_d" when score >= 11 => TechniqueId.SmallStep,
        "hads_d" when score >= 8 => TechniqueId.Paper,
        "hads_d" => TechniqueId.ThoughtRecord,
        "rses" when score <= 14 => TechniqueId.SelfCompassion,
        "rses" when score <= 25 => TechniqueId.SmallStep,
        "rses" => TechniqueId.Experience,
        _ => TechniqueId.Experience
    };
}
