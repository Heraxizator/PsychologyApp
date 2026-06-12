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
        "gad7" => AppStrings.Gad7Score,
        "k10" => AppStrings.K10Score,
        "who5" => AppStrings.Who5Score,
        "phq9" => AppStrings.Phq9Score,
        "isi" => AppStrings.IsiScore,
        "ess" => AppStrings.EssScore,
        "phq15" => AppStrings.Phq15Score,
        "scoff" => AppStrings.ScoffScore,
        "swls" => AppStrings.SwlsScore,
        _ => null
    };

    public static Func<int, string>? ResolveDetail(string analyzerId) => analyzerId switch
    {
        "heck_hess" => AppStrings.HeckHessScoreDetail,
        "haer" => AppStrings.HaerScoreDetail,
        "pochebut" => AppStrings.PochebutScoreDetail,
        "beck" => AppStrings.BeckScoreDetail,
        "gad7" => AppStrings.Gad7ScoreDetail,
        "k10" => AppStrings.K10ScoreDetail,
        "who5" => AppStrings.Who5ScoreDetail,
        "phq9" => AppStrings.Phq9ScoreDetail,
        "isi" => AppStrings.IsiScoreDetail,
        "ess" => AppStrings.EssScoreDetail,
        "phq15" => AppStrings.Phq15ScoreDetail,
        "scoff" => AppStrings.ScoffScoreDetail,
        "swls" => AppStrings.SwlsScoreDetail,
        _ => null
    };

    public static string? ResolveRecommendationReason(string? analyzerId, int score) => analyzerId switch
    {
        "beck" => AppStrings.TestRecommendationReasonBeck(score),
        "heck_hess" => AppStrings.TestRecommendationReasonHeckHess(score),
        "haer" => AppStrings.TestRecommendationReasonHaer(score),
        "pochebut" => AppStrings.TestRecommendationReasonPochebut(score),
        "gad7" => AppStrings.TestRecommendationReasonGad7(score),
        "k10" => AppStrings.TestRecommendationReasonK10(score),
        "who5" => AppStrings.TestRecommendationReasonWho5(score),
        "phq9" => AppStrings.TestRecommendationReasonPhq9(score),
        "isi" => AppStrings.TestRecommendationReasonIsi(score),
        "ess" => AppStrings.TestRecommendationReasonEss(score),
        "phq15" => AppStrings.TestRecommendationReasonPhq15(score),
        "scoff" => AppStrings.TestRecommendationReasonScoff(score),
        "swls" => AppStrings.TestRecommendationReasonSwls(score),
        _ => null
    };

    public static TechniqueId? RecommendTechnique(string? analyzerId, int score) =>
        TestScoreRecommendation.RecommendTechnique(analyzerId, score);
}
