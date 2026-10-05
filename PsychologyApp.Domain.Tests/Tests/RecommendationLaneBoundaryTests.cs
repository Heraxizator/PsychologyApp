using Xunit;

namespace PsychologyApp.Domain.Tests.Tests;

/// <summary>
/// The score at which a questionnaire stops being "fine" decides what the person is told to do next. Each case sits on a boundary
/// (the last score of a lane and the first of the next); a mutation test showed these were not covered at all.
/// </summary>
public sealed class RecommendationLaneBoundaryTests
{
    private const RecommendationLane Low = RecommendationLane.Low;
    private const RecommendationLane Mid = RecommendationLane.Mid;
    private const RecommendationLane High = RecommendationLane.High;

    private static RecommendationLane Recommend(string test, int score) => test switch
    {
        "beck" => TestScoreBandClassifier.RecommendBeck(score),
        "heckhess" => TestScoreBandClassifier.RecommendHeckHess(score),
        "haer" => TestScoreBandClassifier.RecommendHaer(score),
        "pochebut" => TestScoreBandClassifier.RecommendPochebut(score),
        "gad7" => TestScoreBandClassifier.RecommendGad7(score),
        "k10" => TestScoreBandClassifier.RecommendK10(score),
        "who5" => TestScoreBandClassifier.RecommendWho5(score),
        "phq9" => TestScoreBandClassifier.RecommendPhq9(score),
        "isi" => TestScoreBandClassifier.RecommendIsi(score),
        "ess" => TestScoreBandClassifier.RecommendEss(score),
        "phq15" => TestScoreBandClassifier.RecommendPhq15(score),
        "scoff" => TestScoreBandClassifier.RecommendScoff(score),
        "swls" => TestScoreBandClassifier.RecommendSwls(score),
        "pss10" => TestScoreBandClassifier.RecommendPss10(score),
        "phq2" => TestScoreBandClassifier.RecommendPhq2(score),
        "gad2" => TestScoreBandClassifier.RecommendGad2(score),
        "hads_a" => TestScoreBandClassifier.RecommendHadsAnxiety(score),
        "hads_d" => TestScoreBandClassifier.RecommendHadsDepression(score),
        "rses" => TestScoreBandClassifier.RecommendRses(score),
        _ => throw new ArgumentOutOfRangeException(nameof(test), test, null)
    };

    [Theory]
    [InlineData("beck", 9, Low)]
    [InlineData("beck", 10, High)]
    [InlineData("heckhess", 24, Low)]
    [InlineData("heckhess", 25, High)]
    [InlineData("haer", 28, Low)]
    [InlineData("haer", 29, High)]
    [InlineData("pochebut", 24, Low)]
    [InlineData("pochebut", 25, High)]
    [InlineData("gad7", 9, Low)]
    [InlineData("gad7", 10, High)]
    [InlineData("k10", 15, Low)]
    [InlineData("k10", 16, Mid)]
    [InlineData("k10", 21, Mid)]
    [InlineData("k10", 22, High)]
    [InlineData("who5", 12, High)]
    [InlineData("who5", 13, Low)]
    [InlineData("phq9", 9, Low)]
    [InlineData("phq9", 10, High)]
    [InlineData("isi", 14, Low)]
    [InlineData("isi", 15, Mid)]
    [InlineData("isi", 21, Mid)]
    [InlineData("isi", 22, High)]
    [InlineData("ess", 10, Low)]
    [InlineData("ess", 11, Mid)]
    [InlineData("ess", 15, Mid)]
    [InlineData("ess", 16, High)]
    [InlineData("phq15", 9, Low)]
    [InlineData("phq15", 10, Mid)]
    [InlineData("phq15", 14, Mid)]
    [InlineData("phq15", 15, High)]
    [InlineData("scoff", 1, Low)]
    [InlineData("scoff", 2, High)]
    [InlineData("swls", 20, High)]
    [InlineData("swls", 21, Low)]
    [InlineData("pss10", 13, Low)]
    [InlineData("pss10", 14, Mid)]
    [InlineData("pss10", 26, Mid)]
    [InlineData("pss10", 27, High)]
    [InlineData("phq2", 2, Low)]
    [InlineData("phq2", 3, High)]
    [InlineData("gad2", 2, Low)]
    [InlineData("gad2", 3, High)]
    [InlineData("hads_a", 7, Low)]
    [InlineData("hads_a", 8, Mid)]
    [InlineData("hads_a", 10, Mid)]
    [InlineData("hads_a", 11, High)]
    [InlineData("hads_d", 7, Low)]
    [InlineData("hads_d", 8, Mid)]
    [InlineData("hads_d", 10, Mid)]
    [InlineData("hads_d", 11, High)]
    [InlineData("rses", 14, High)]
    [InlineData("rses", 15, Mid)]
    [InlineData("rses", 25, Mid)]
    [InlineData("rses", 26, Low)]
    public void The_lane_changes_exactly_at_the_published_cut_off(string test, int score, RecommendationLane expected) =>
        Assert.Equal(expected, Recommend(test, score));

    [Theory]
    [InlineData(9, BeckBand.None)]
    [InlineData(10, BeckBand.Mild)]
    [InlineData(15, BeckBand.Mild)]
    [InlineData(16, BeckBand.Moderate)]
    [InlineData(19, BeckBand.Moderate)]
    [InlineData(20, BeckBand.Marked)]
    [InlineData(29, BeckBand.Marked)]
    [InlineData(30, BeckBand.Severe)]
    public void Beck_bands_change_exactly_at_the_published_cut_offs(int score, BeckBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyBeck(score));

    [Theory]
    [InlineData(4, Phq9Band.Minimal)]
    [InlineData(5, Phq9Band.Mild)]
    [InlineData(9, Phq9Band.Mild)]
    [InlineData(10, Phq9Band.Moderate)]
    [InlineData(14, Phq9Band.Moderate)]
    [InlineData(15, Phq9Band.ModeratelySevere)]
    [InlineData(19, Phq9Band.ModeratelySevere)]
    [InlineData(20, Phq9Band.Severe)]
    public void Phq9_bands_change_exactly_at_the_published_cut_offs(int score, Phq9Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyPhq9(score));

    [Theory]
    [InlineData(4, Gad7Band.Minimal)]
    [InlineData(5, Gad7Band.Mild)]
    [InlineData(9, Gad7Band.Mild)]
    [InlineData(10, Gad7Band.Moderate)]
    [InlineData(14, Gad7Band.Moderate)]
    [InlineData(15, Gad7Band.Severe)]
    public void Gad7_bands_change_exactly_at_the_published_cut_offs(int score, Gad7Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyGad7(score));

    [Theory]
    [InlineData(7, IsiBand.None)]
    [InlineData(8, IsiBand.Subthreshold)]
    [InlineData(14, IsiBand.Subthreshold)]
    [InlineData(15, IsiBand.Moderate)]
    [InlineData(21, IsiBand.Moderate)]
    [InlineData(22, IsiBand.Severe)]
    public void Isi_bands_change_exactly_at_the_published_cut_offs(int score, IsiBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyIsi(score));

    [Theory]
    [InlineData(10, EssBand.Normal)]
    [InlineData(11, EssBand.Mild)]
    [InlineData(12, EssBand.Mild)]
    [InlineData(13, EssBand.Moderate)]
    [InlineData(15, EssBand.Moderate)]
    [InlineData(16, EssBand.Severe)]
    public void Ess_bands_change_exactly_at_the_published_cut_offs(int score, EssBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyEss(score));

    [Theory]
    [InlineData(9, SwlsBand.ExtremelyDissatisfied)]
    [InlineData(10, SwlsBand.Dissatisfied)]
    [InlineData(14, SwlsBand.Dissatisfied)]
    [InlineData(15, SwlsBand.SlightlyDissatisfied)]
    [InlineData(19, SwlsBand.SlightlyDissatisfied)]
    [InlineData(20, SwlsBand.Neutral)]
    [InlineData(21, SwlsBand.SlightlySatisfied)]
    [InlineData(25, SwlsBand.SlightlySatisfied)]
    [InlineData(26, SwlsBand.Satisfied)]
    [InlineData(30, SwlsBand.Satisfied)]
    [InlineData(31, SwlsBand.ExtremelySatisfied)]
    public void Swls_bands_change_exactly_at_the_published_cut_offs(int score, SwlsBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifySwls(score));

    [Theory]
    [InlineData(7, HadsBand.Normal)]
    [InlineData(8, HadsBand.Borderline)]
    [InlineData(10, HadsBand.Borderline)]
    [InlineData(11, HadsBand.Elevated)]
    public void Hads_bands_change_exactly_at_the_published_cut_offs(int score, HadsBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyHads(score));
}
