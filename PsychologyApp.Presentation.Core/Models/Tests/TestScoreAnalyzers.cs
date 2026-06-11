using PsychologyApp.Application.Models;
using PsychologyApp.Application.TestScoring;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Models.Tests;

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

    public static TechniqueId? RecommendTechnique(string? analyzerId, int score) =>
        TestScoreRecommendation.RecommendTechnique(analyzerId, score);
}
