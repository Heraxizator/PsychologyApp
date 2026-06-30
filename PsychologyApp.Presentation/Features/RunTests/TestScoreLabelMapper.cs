using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class TestScoreLabelMapper
{
    public static string? GetSummary(string? analyzerId, int score) => analyzerId switch
    {
        "heck_hess" => AppStrings.HeckHessScore(score),
        "haer" => AppStrings.HaerScore(score),
        "pochebut" => AppStrings.PochebutScore(score),
        "beck" => AppStrings.BeckScore(score),
        "gad7" => AppStrings.Gad7Score(score),
        "k10" => AppStrings.K10Score(score),
        "who5" => AppStrings.Who5Score(score),
        "phq9" => AppStrings.Phq9Score(score),
        "isi" => AppStrings.IsiScore(score),
        "ess" => AppStrings.EssScore(score),
        "phq15" => AppStrings.Phq15Score(score),
        "scoff" => AppStrings.ScoffScore(score),
        "swls" => AppStrings.SwlsScore(score),
        _ => null
    };

    public static string? GetDetail(string? analyzerId, int score) => analyzerId switch
    {
        "heck_hess" => AppStrings.HeckHessScoreDetail(score),
        "haer" => AppStrings.HaerScoreDetail(score),
        "pochebut" => AppStrings.PochebutScoreDetail(score),
        "beck" => AppStrings.BeckScoreDetail(score),
        "gad7" => AppStrings.Gad7ScoreDetail(score),
        "k10" => AppStrings.K10ScoreDetail(score),
        "who5" => AppStrings.Who5ScoreDetail(score),
        "phq9" => AppStrings.Phq9ScoreDetail(score),
        "isi" => AppStrings.IsiScoreDetail(score),
        "ess" => AppStrings.EssScoreDetail(score),
        "phq15" => AppStrings.Phq15ScoreDetail(score),
        "scoff" => AppStrings.ScoffScoreDetail(score),
        "swls" => AppStrings.SwlsScoreDetail(score),
        _ => null
    };

    public static string? GetRecommendationReason(string? analyzerId, int score) => analyzerId switch
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
}
