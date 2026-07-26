namespace PsychologyApp.Domain.Tests;

public enum BeckBand { None, Mild, Moderate, Marked, Severe }
public enum BinarySeverityBand { Low, High }
public enum PochebutBand { Low, Moderate, High }
public enum Gad7Band { Minimal, Mild, Moderate, Severe }
public enum K10Band { Low, Mild, Moderate, High }
public enum Who5Band { Low, Fair, Good }
public enum Phq9Band { Minimal, Mild, Moderate, ModeratelySevere, Severe }
public enum IsiBand { None, Subthreshold, Moderate, Severe }
public enum EssBand { Normal, Mild, Moderate, Severe }
public enum Phq15Band { Minimal, Low, Moderate, High }
public enum ScoffBand { Negative, Positive }
public enum SwlsBand
{
    ExtremelyDissatisfied,
    Dissatisfied,
    SlightlyDissatisfied,
    Neutral,
    SlightlySatisfied,
    Satisfied,
    ExtremelySatisfied
}
public enum Pss10Band { Low, Moderate, High }
public enum ScreenBand { Negative, Positive }
public enum HadsBand { Normal, Borderline, Elevated }
public enum RsesBand { Low, Normal, High }

public enum RecommendationLane { Low, Mid, High }

public static class TestScoreBandClassifier
{
    public static BeckBand ClassifyBeck(int score) => score switch
    {
        <= 9 => BeckBand.None,
        <= 15 => BeckBand.Mild,
        <= 19 => BeckBand.Moderate,
        <= 29 => BeckBand.Marked,
        _ => BeckBand.Severe
    };

    public static BinarySeverityBand ClassifyHeckHess(int score) =>
        score <= 24 ? BinarySeverityBand.Low : BinarySeverityBand.High;

    public static BinarySeverityBand ClassifyHaer(int score) =>
        score <= 29 ? BinarySeverityBand.Low : BinarySeverityBand.High;

    public static PochebutBand ClassifyPochebut(int score) => score switch
    {
        <= 10 => PochebutBand.Low,
        <= 24 => PochebutBand.Moderate,
        _ => PochebutBand.High
    };

    public static Gad7Band ClassifyGad7(int score) => score switch
    {
        <= 4 => Gad7Band.Minimal,
        <= 9 => Gad7Band.Mild,
        <= 14 => Gad7Band.Moderate,
        _ => Gad7Band.Severe
    };

    public static K10Band ClassifyK10(int score) => score switch
    {
        <= 15 => K10Band.Low,
        <= 21 => K10Band.Mild,
        <= 29 => K10Band.Moderate,
        _ => K10Band.High
    };

    public static Who5Band ClassifyWho5(int score) => score switch
    {
        <= 12 => Who5Band.Low,
        <= 18 => Who5Band.Fair,
        _ => Who5Band.Good
    };

    public static Phq9Band ClassifyPhq9(int score) => score switch
    {
        <= 4 => Phq9Band.Minimal,
        <= 9 => Phq9Band.Mild,
        <= 14 => Phq9Band.Moderate,
        <= 19 => Phq9Band.ModeratelySevere,
        _ => Phq9Band.Severe
    };

    public static IsiBand ClassifyIsi(int score) => score switch
    {
        <= 7 => IsiBand.None,
        <= 14 => IsiBand.Subthreshold,
        <= 21 => IsiBand.Moderate,
        _ => IsiBand.Severe
    };

    public static EssBand ClassifyEss(int score) => score switch
    {
        <= 10 => EssBand.Normal,
        <= 12 => EssBand.Mild,
        <= 15 => EssBand.Moderate,
        _ => EssBand.Severe
    };

    public static Phq15Band ClassifyPhq15(int score) => score switch
    {
        <= 4 => Phq15Band.Minimal,
        <= 9 => Phq15Band.Low,
        <= 14 => Phq15Band.Moderate,
        _ => Phq15Band.High
    };

    public static ScoffBand ClassifyScoff(int score) =>
        score <= 1 ? ScoffBand.Negative : ScoffBand.Positive;

    public static SwlsBand ClassifySwls(int score) => score switch
    {
        <= 9 => SwlsBand.ExtremelyDissatisfied,
        <= 14 => SwlsBand.Dissatisfied,
        <= 19 => SwlsBand.SlightlyDissatisfied,
        20 => SwlsBand.Neutral,
        <= 25 => SwlsBand.SlightlySatisfied,
        <= 30 => SwlsBand.Satisfied,
        _ => SwlsBand.ExtremelySatisfied
    };

    public static Pss10Band ClassifyPss10(int score) => score switch
    {
        <= 13 => Pss10Band.Low,
        <= 26 => Pss10Band.Moderate,
        _ => Pss10Band.High
    };

    public static ScreenBand ClassifyPhq2(int score) =>
        score <= 2 ? ScreenBand.Negative : ScreenBand.Positive;

    public static ScreenBand ClassifyGad2(int score) =>
        score <= 2 ? ScreenBand.Negative : ScreenBand.Positive;

    public static HadsBand ClassifyHads(int score) => score switch
    {
        <= 7 => HadsBand.Normal,
        <= 10 => HadsBand.Borderline,
        _ => HadsBand.Elevated
    };

    public static RsesBand ClassifyRses(int score) => score switch
    {
        <= 14 => RsesBand.Low,
        <= 25 => RsesBand.Normal,
        _ => RsesBand.High
    };

    public static RecommendationLane RecommendBeck(int score) =>
        score >= 10 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendHeckHess(int score) =>
        score >= 25 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendHaer(int score) =>
        score >= 29 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendPochebut(int score) =>
        score >= 25 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendGad7(int score) =>
        score >= 10 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendK10(int score) => score switch
    {
        >= 22 => RecommendationLane.High,
        >= 16 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendWho5(int score) =>
        score <= 12 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendPhq9(int score) =>
        score >= 10 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendIsi(int score) => score switch
    {
        >= 22 => RecommendationLane.High,
        >= 15 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendEss(int score) => score switch
    {
        >= 16 => RecommendationLane.High,
        >= 11 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendPhq15(int score) => score switch
    {
        >= 15 => RecommendationLane.High,
        >= 10 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendScoff(int score) =>
        score >= 2 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendSwls(int score) =>
        score <= 20 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendPss10(int score) => score switch
    {
        >= 27 => RecommendationLane.High,
        >= 14 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendPhq2(int score) =>
        score >= 3 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendGad2(int score) =>
        score >= 3 ? RecommendationLane.High : RecommendationLane.Low;

    public static RecommendationLane RecommendHadsAnxiety(int score) => score switch
    {
        >= 11 => RecommendationLane.High,
        >= 8 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendHadsDepression(int score) => score switch
    {
        >= 11 => RecommendationLane.High,
        >= 8 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };

    public static RecommendationLane RecommendRses(int score) => score switch
    {
        <= 14 => RecommendationLane.High,
        <= 25 => RecommendationLane.Mid,
        _ => RecommendationLane.Low
    };
}
