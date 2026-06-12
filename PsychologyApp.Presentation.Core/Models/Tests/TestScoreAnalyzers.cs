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

    public static Func<int, string>? ResolveDetail(string analyzerId) => analyzerId switch
    {
        "heck_hess" => AppStrings.HeckHessScoreDetail,
        "haer" => AppStrings.HaerScoreDetail,
        "pochebut" => AppStrings.PochebutScoreDetail,
        "beck" => AppStrings.BeckScoreDetail,
        _ => null
    };

    public static string? ResolveRecommendationReason(string? analyzerId, int score) => analyzerId switch
    {
        "beck" => AppStrings.TestRecommendationReasonBeck(score),
        "heck_hess" => AppStrings.TestRecommendationReasonHeckHess(score),
        "haer" => AppStrings.TestRecommendationReasonHaer(score),
        "pochebut" => AppStrings.TestRecommendationReasonPochebut(score),
        _ => null
    };

    public static TechniqueId? RecommendTechnique(string? analyzerId, int score) =>
        TestScoreRecommendation.RecommendTechnique(analyzerId, score);
}
