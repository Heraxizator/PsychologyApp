using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Application.Tests;

public readonly record struct ScoreRange(int Min, int Max);

public static class TestScoreInterpreter
{
    public static ScoreRange? GetScoreRange(string? analyzerId) => analyzerId switch
    {
        "beck" => new ScoreRange(0, 63),
        "heck_hess" => new ScoreRange(0, 40),
        "haer" => new ScoreRange(0, 40),
        "pochebut" => new ScoreRange(0, 40),
        "gad7" => new ScoreRange(0, 21),
        "k10" => new ScoreRange(10, 50),
        "who5" => new ScoreRange(0, 25),
        "phq9" => new ScoreRange(0, 27),
        "isi" => new ScoreRange(0, 28),
        "ess" => new ScoreRange(0, 24),
        "phq15" => new ScoreRange(0, 30),
        "scoff" => new ScoreRange(0, 5),
        "swls" => new ScoreRange(5, 35),
        "pss10" => new ScoreRange(0, 40),
        "phq2" => new ScoreRange(0, 6),
        "gad2" => new ScoreRange(0, 6),
        "hads_a" => new ScoreRange(0, 21),
        "hads_d" => new ScoreRange(0, 21),
        "rses" => new ScoreRange(0, 30),
        "pcptsd5" => new ScoreRange(0, 5),
        "pcl5" => new ScoreRange(0, 80),
        _ => null
    };
    public static bool IsKnownAnalyzer(string analyzerId) => analyzerId switch
    {
        "heck_hess" or "haer" or "pochebut" or "beck" or "gad7" or "k10" or "who5"
            or "phq9" or "isi" or "ess" or "phq15" or "scoff" or "swls"
            or "pss10" or "phq2" or "gad2" or "hads_a" or "hads_d" or "rses" or "pcptsd5" or "pcl5" => true,
        _ => false
    };

    /// <summary>
    /// The band stored with a result. The cut-offs are the ones the result text is chosen by (TestScoreBandClassifier), so the
    /// stored band and the words the person reads can never disagree on a border score.
    /// </summary>
    public static int GetBandIndex(string? analyzerId, int score) => analyzerId switch
    {
        "beck" => (int)TestScoreBandClassifier.ClassifyBeck(score),
        "heck_hess" => (int)TestScoreBandClassifier.ClassifyHeckHess(score),
        "haer" => (int)TestScoreBandClassifier.ClassifyHaer(score),
        "pochebut" => (int)TestScoreBandClassifier.ClassifyPochebut(score),
        "gad7" => (int)TestScoreBandClassifier.ClassifyGad7(score),
        "k10" => (int)TestScoreBandClassifier.ClassifyK10(score),
        "who5" => (int)TestScoreBandClassifier.ClassifyWho5(score),
        "phq9" => (int)TestScoreBandClassifier.ClassifyPhq9(score),
        "isi" => (int)TestScoreBandClassifier.ClassifyIsi(score),
        "ess" => (int)TestScoreBandClassifier.ClassifyEss(score),
        "phq15" => (int)TestScoreBandClassifier.ClassifyPhq15(score),
        "scoff" => (int)TestScoreBandClassifier.ClassifyScoff(score),
        // The stored band of SWLS is coarser than its seven texts: dissatisfied (up to 20), neutral to slightly satisfied, satisfied.
        "swls" => score switch
        {
            <= 20 => 0,
            <= 25 => 1,
            _ => 2
        },
        "pss10" => (int)TestScoreBandClassifier.ClassifyPss10(score),
        "phq2" => (int)TestScoreBandClassifier.ClassifyPhq2(score),
        "gad2" => (int)TestScoreBandClassifier.ClassifyGad2(score),
        "pcptsd5" => (int)TestScoreBandClassifier.ClassifyPcPtsd5(score),
        "pcl5" => (int)TestScoreBandClassifier.ClassifyPcl5(score),
        "hads_a" or "hads_d" => (int)TestScoreBandClassifier.ClassifyHads(score),
        "rses" => (int)TestScoreBandClassifier.ClassifyRses(score),
        _ => -1
    };
}
