using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string BeckScore(BeckBand band) => band switch
    {
        BeckBand.None => R("BeckScore.BeckBand.None"),
        BeckBand.Mild => R("BeckScore.BeckBand.Mild"),
        BeckBand.Moderate => R("BeckScore.BeckBand.Moderate"),
        BeckBand.Marked => R("BeckScore.BeckBand.Marked"),
        _ => R("BeckScore.default")
    };

    public static string BeckScoreDetail(BeckBand band) => band switch
    {
        BeckBand.None => R("BeckScoreDetail.BeckBand.None"),
        BeckBand.Mild => R("BeckScoreDetail.BeckBand.Mild"),
        BeckBand.Moderate => R("BeckScoreDetail.BeckBand.Moderate"),
        BeckBand.Marked => R("BeckScoreDetail.BeckBand.Marked"),
        _ => R("BeckScoreDetail.default")
    };

    public static string HeckHessScore(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? R("HeckHessScore.1")
            : R("HeckHessScore.2");

    public static string HeckHessScoreDetail(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? R("HeckHessScoreDetail.1")
            : R("HeckHessScoreDetail.2");

    public static string HaerScore(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? R("HaerScore.1")
            : R("HaerScore.2");

    public static string HaerScoreDetail(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? R("HaerScoreDetail.1")
            : R("HaerScoreDetail.2");

    public static string PochebutScore(PochebutBand band) => band switch
    {
        PochebutBand.Low => R("PochebutScore.PochebutBand.Low"),
        PochebutBand.Moderate => R("PochebutScore.PochebutBand.Moderate"),
        _ => R("PochebutScore.default")
    };

    public static string PochebutScoreDetail(PochebutBand band) => band switch
    {
        PochebutBand.Low => R("PochebutScoreDetail.PochebutBand.Low"),
        PochebutBand.Moderate => R("PochebutScoreDetail.PochebutBand.Moderate"),
        _ => R("PochebutScoreDetail.default")
    };

    public static string TestRecommendationReasonBeck(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonBeck.1")
            : R("TestRecommendationReasonBeck.2");

    public static string TestRecommendationReasonHeckHess(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonHeckHess.1")
            : R("TestRecommendationReasonHeckHess.2");

    public static string TestRecommendationReasonHaer(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonHaer.1")
            : R("TestRecommendationReasonHaer.2");

    public static string TestRecommendationReasonPochebut(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonPochebut.1")
            : R("TestRecommendationReasonPochebut.2");

    public static string Gad7Score(Gad7Band band) => band switch
    {
        Gad7Band.Minimal => R("Gad7Score.Gad7Band.Minimal"),
        Gad7Band.Mild => R("Gad7Score.Gad7Band.Mild"),
        Gad7Band.Moderate => R("Gad7Score.Gad7Band.Moderate"),
        _ => R("Gad7Score.default")
    };

    public static string Gad7ScoreDetail(Gad7Band band) => band switch
    {
        Gad7Band.Minimal => R("Gad7ScoreDetail.Gad7Band.Minimal"),
        Gad7Band.Mild => R("Gad7ScoreDetail.Gad7Band.Mild"),
        Gad7Band.Moderate => R("Gad7ScoreDetail.Gad7Band.Moderate"),
        _ => R("Gad7ScoreDetail.default")
    };

    public static string K10Score(K10Band band) => band switch
    {
        K10Band.Low => R("K10Score.K10Band.Low"),
        K10Band.Mild => R("K10Score.K10Band.Mild"),
        K10Band.Moderate => R("K10Score.K10Band.Moderate"),
        _ => R("K10Score.default")
    };

    public static string K10ScoreDetail(K10Band band) => band switch
    {
        K10Band.Low => R("K10ScoreDetail.K10Band.Low"),
        K10Band.Mild => R("K10ScoreDetail.K10Band.Mild"),
        K10Band.Moderate => R("K10ScoreDetail.K10Band.Moderate"),
        _ => R("K10ScoreDetail.default")
    };

    public static string Who5Score(Who5Band band) => band switch
    {
        Who5Band.Low => R("Who5Score.Who5Band.Low"),
        Who5Band.Fair => R("Who5Score.Who5Band.Fair"),
        _ => R("Who5Score.default")
    };

    public static string Who5ScoreDetail(Who5Band band) => band switch
    {
        Who5Band.Low => R("Who5ScoreDetail.Who5Band.Low"),
        Who5Band.Fair => R("Who5ScoreDetail.Who5Band.Fair"),
        _ => R("Who5ScoreDetail.default")
    };

    public static string TestRecommendationReasonGad7(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonGad7.1")
            : R("TestRecommendationReasonGad7.2");

    public static string TestRecommendationReasonK10(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonK10.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonK10.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonK10.default")
    };

    public static string TestRecommendationReasonWho5(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonWho5.1")
            : R("TestRecommendationReasonWho5.2");

    public static string Phq9Score(Phq9Band band) => band switch
    {
        Phq9Band.Minimal => R("Phq9Score.Phq9Band.Minimal"),
        Phq9Band.Mild => R("Phq9Score.Phq9Band.Mild"),
        Phq9Band.Moderate => R("Phq9Score.Phq9Band.Moderate"),
        Phq9Band.ModeratelySevere => R("Phq9Score.Phq9Band.ModeratelySevere"),
        _ => R("Phq9Score.default")
    };

    public static string Phq9ScoreDetail(Phq9Band band) => band switch
    {
        Phq9Band.Minimal => R("Phq9ScoreDetail.Phq9Band.Minimal"),
        Phq9Band.Mild => R("Phq9ScoreDetail.Phq9Band.Mild"),
        Phq9Band.Moderate => R("Phq9ScoreDetail.Phq9Band.Moderate"),
        Phq9Band.ModeratelySevere => R("Phq9ScoreDetail.Phq9Band.ModeratelySevere"),
        _ => R("Phq9ScoreDetail.default")
    };

    public static string IsiScore(IsiBand band) => band switch
    {
        IsiBand.None => R("IsiScore.IsiBand.None"),
        IsiBand.Subthreshold => R("IsiScore.IsiBand.Subthreshold"),
        IsiBand.Moderate => R("IsiScore.IsiBand.Moderate"),
        _ => R("IsiScore.default")
    };

    public static string IsiScoreDetail(IsiBand band) => band switch
    {
        IsiBand.None => R("IsiScoreDetail.IsiBand.None"),
        IsiBand.Subthreshold => R("IsiScoreDetail.IsiBand.Subthreshold"),
        IsiBand.Moderate => R("IsiScoreDetail.IsiBand.Moderate"),
        _ => R("IsiScoreDetail.default")
    };

    public static string EssScore(EssBand band) => band switch
    {
        EssBand.Normal => R("EssScore.EssBand.Normal"),
        EssBand.Mild => R("EssScore.EssBand.Mild"),
        EssBand.Moderate => R("EssScore.EssBand.Moderate"),
        _ => R("EssScore.default")
    };

    public static string EssScoreDetail(EssBand band) => band switch
    {
        EssBand.Normal => R("EssScoreDetail.EssBand.Normal"),
        EssBand.Mild => R("EssScoreDetail.EssBand.Mild"),
        EssBand.Moderate => R("EssScoreDetail.EssBand.Moderate"),
        _ => R("EssScoreDetail.default")
    };

    public static string TestRecommendationReasonPhq9(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonPhq9.1")
            : R("TestRecommendationReasonPhq9.2");

    public static string TestRecommendationReasonIsi(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonIsi.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonIsi.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonIsi.default")
    };

    public static string TestRecommendationReasonEss(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonEss.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonEss.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonEss.default")
    };

    public static string Phq15Score(Phq15Band band) => band switch
    {
        Phq15Band.Minimal => R("Phq15Score.Phq15Band.Minimal"),
        Phq15Band.Low => R("Phq15Score.Phq15Band.Low"),
        Phq15Band.Moderate => R("Phq15Score.Phq15Band.Moderate"),
        _ => R("Phq15Score.default")
    };

    public static string Phq15ScoreDetail(Phq15Band band) => band switch
    {
        Phq15Band.Minimal => R("Phq15ScoreDetail.Phq15Band.Minimal"),
        Phq15Band.Low => R("Phq15ScoreDetail.Phq15Band.Low"),
        Phq15Band.Moderate => R("Phq15ScoreDetail.Phq15Band.Moderate"),
        _ => R("Phq15ScoreDetail.default")
    };

    public static string ScoffScore(ScoffBand band) =>
        band == ScoffBand.Negative
            ? R("ScoffScore.1")
            : R("ScoffScore.2");

    public static string ScoffScoreDetail(ScoffBand band) =>
        band == ScoffBand.Negative
            ? R("ScoffScoreDetail.1")
            : R("ScoffScoreDetail.2");

    public static string SwlsScore(SwlsBand band) => band switch
    {
        SwlsBand.ExtremelyDissatisfied => R("SwlsScore.SwlsBand.ExtremelyDissatisfied"),
        SwlsBand.Dissatisfied => R("SwlsScore.SwlsBand.Dissatisfied"),
        SwlsBand.SlightlyDissatisfied => R("SwlsScore.SwlsBand.SlightlyDissatisfied"),
        SwlsBand.Neutral => R("SwlsScore.SwlsBand.Neutral"),
        SwlsBand.SlightlySatisfied => R("SwlsScore.SwlsBand.SlightlySatisfied"),
        SwlsBand.Satisfied => R("SwlsScore.SwlsBand.Satisfied"),
        _ => R("SwlsScore.default")
    };

    public static string SwlsScoreDetail(SwlsBand band) => band switch
    {
        SwlsBand.ExtremelyDissatisfied => R("SwlsScoreDetail.SwlsBand.ExtremelyDissatisfied"),
        SwlsBand.Dissatisfied => R("SwlsScoreDetail.SwlsBand.Dissatisfied"),
        SwlsBand.SlightlyDissatisfied => R("SwlsScoreDetail.SwlsBand.SlightlyDissatisfied"),
        SwlsBand.Neutral => R("SwlsScoreDetail.SwlsBand.Neutral"),
        SwlsBand.SlightlySatisfied => R("SwlsScoreDetail.SwlsBand.SlightlySatisfied"),
        SwlsBand.Satisfied => R("SwlsScoreDetail.SwlsBand.Satisfied"),
        _ => R("SwlsScoreDetail.default")
    };

    public static string TestRecommendationReasonPhq15(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonPhq15.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonPhq15.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonPhq15.default")
    };

    public static string TestRecommendationReasonScoff(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonScoff.1")
            : R("TestRecommendationReasonScoff.2");

    public static string TestRecommendationReasonSwls(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonSwls.1")
            : R("TestRecommendationReasonSwls.2");

    public static string Pss10Score(Pss10Band band) => band switch
    {
        Pss10Band.Low => R("Pss10Score.Pss10Band.Low"),
        Pss10Band.Moderate => R("Pss10Score.Pss10Band.Moderate"),
        _ => R("Pss10Score.default")
    };

    public static string Pss10ScoreDetail(Pss10Band band) => band switch
    {
        Pss10Band.Low => R("Pss10ScoreDetail.Pss10Band.Low"),
        Pss10Band.Moderate => R("Pss10ScoreDetail.Pss10Band.Moderate"),
        _ => R("Pss10ScoreDetail.default")
    };

    public static string Phq2Score(ScreenBand band) =>
        band == ScreenBand.Negative
            ? R("Phq2Score.1")
            : R("Phq2Score.2");

    public static string Phq2ScoreDetail(ScreenBand band) =>
        band == ScreenBand.Negative
            ? R("Phq2ScoreDetail.1")
            : R("Phq2ScoreDetail.2");

    public static string Gad2Score(ScreenBand band) =>
        band == ScreenBand.Negative
            ? R("Gad2Score.1")
            : R("Gad2Score.2");

    public static string Gad2ScoreDetail(ScreenBand band) =>
        band == ScreenBand.Negative
            ? R("Gad2ScoreDetail.1")
            : R("Gad2ScoreDetail.2");

    public static string HadsAnxietyScore(HadsBand band) => band switch
    {
        HadsBand.Normal => R("HadsAnxietyScore.HadsBand.Normal"),
        HadsBand.Borderline => R("HadsAnxietyScore.HadsBand.Borderline"),
        _ => R("HadsAnxietyScore.default")
    };

    public static string HadsAnxietyScoreDetail(HadsBand band) => band switch
    {
        HadsBand.Normal => R("HadsAnxietyScoreDetail.HadsBand.Normal"),
        HadsBand.Borderline => R("HadsAnxietyScoreDetail.HadsBand.Borderline"),
        _ => R("HadsAnxietyScoreDetail.default")
    };

    public static string HadsDepressionScore(HadsBand band) => band switch
    {
        HadsBand.Normal => R("HadsDepressionScore.HadsBand.Normal"),
        HadsBand.Borderline => R("HadsDepressionScore.HadsBand.Borderline"),
        _ => R("HadsDepressionScore.default")
    };

    public static string HadsDepressionScoreDetail(HadsBand band) => band switch
    {
        HadsBand.Normal => R("HadsDepressionScoreDetail.HadsBand.Normal"),
        HadsBand.Borderline => R("HadsDepressionScoreDetail.HadsBand.Borderline"),
        _ => R("HadsDepressionScoreDetail.default")
    };

    public static string RsesScore(RsesBand band) => band switch
    {
        RsesBand.Low => R("RsesScore.RsesBand.Low"),
        RsesBand.Normal => R("RsesScore.RsesBand.Normal"),
        _ => R("RsesScore.default")
    };

    public static string RsesScoreDetail(RsesBand band) => band switch
    {
        RsesBand.Low => R("RsesScoreDetail.RsesBand.Low"),
        RsesBand.Normal => R("RsesScoreDetail.RsesBand.Normal"),
        _ => R("RsesScoreDetail.default")
    };

    public static string TestRecommendationReasonPss10(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonPss10.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonPss10.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonPss10.default")
    };

    public static string TestRecommendationReasonPhq2(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonPhq2.1")
            : R("TestRecommendationReasonPhq2.2");

    public static string TestRecommendationReasonGad2(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? R("TestRecommendationReasonGad2.1")
            : R("TestRecommendationReasonGad2.2");

    public static string TestRecommendationReasonHadsAnxiety(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonHadsAnxiety.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonHadsAnxiety.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonHadsAnxiety.default")
    };

    public static string TestRecommendationReasonHadsDepression(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonHadsDepression.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonHadsDepression.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonHadsDepression.default")
    };

    public static string TestRecommendationReasonRses(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => R("TestRecommendationReasonRses.RecommendationLane.High"),
        RecommendationLane.Mid => R("TestRecommendationReasonRses.RecommendationLane.Mid"),
        _ => R("TestRecommendationReasonRses.default")
    };
}
