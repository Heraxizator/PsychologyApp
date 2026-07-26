using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Domain.Tests.Tests;

public sealed class TestScoreBandClassifierTests
{
    [Theory]
    [InlineData(9, BeckBand.None)]
    [InlineData(10, BeckBand.Mild)]
    [InlineData(15, BeckBand.Mild)]
    [InlineData(16, BeckBand.Moderate)]
    [InlineData(19, BeckBand.Moderate)]
    [InlineData(20, BeckBand.Marked)]
    [InlineData(29, BeckBand.Marked)]
    [InlineData(30, BeckBand.Severe)]
    public void ClassifyBeck_Boundaries(int score, BeckBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyBeck(score));

    [Theory]
    [InlineData(4, Gad7Band.Minimal)]
    [InlineData(5, Gad7Band.Mild)]
    [InlineData(9, Gad7Band.Mild)]
    [InlineData(10, Gad7Band.Moderate)]
    [InlineData(14, Gad7Band.Moderate)]
    [InlineData(15, Gad7Band.Severe)]
    public void ClassifyGad7_Boundaries(int score, Gad7Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyGad7(score));

    [Theory]
    [InlineData(4, Phq9Band.Minimal)]
    [InlineData(5, Phq9Band.Mild)]
    [InlineData(9, Phq9Band.Mild)]
    [InlineData(10, Phq9Band.Moderate)]
    [InlineData(14, Phq9Band.Moderate)]
    [InlineData(15, Phq9Band.ModeratelySevere)]
    [InlineData(19, Phq9Band.ModeratelySevere)]
    [InlineData(20, Phq9Band.Severe)]
    public void ClassifyPhq9_Boundaries(int score, Phq9Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyPhq9(score));

    [Theory]
    [InlineData(24, BinarySeverityBand.Low)]
    [InlineData(25, BinarySeverityBand.High)]
    public void ClassifyHeckHess_Boundaries(int score, BinarySeverityBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyHeckHess(score));

    [Theory]
    [InlineData(29, BinarySeverityBand.Low)]
    [InlineData(30, BinarySeverityBand.High)]
    public void ClassifyHaer_Boundaries(int score, BinarySeverityBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyHaer(score));

    [Theory]
    [InlineData(10, PochebutBand.Low)]
    [InlineData(11, PochebutBand.Moderate)]
    [InlineData(24, PochebutBand.Moderate)]
    [InlineData(25, PochebutBand.High)]
    public void ClassifyPochebut_Boundaries(int score, PochebutBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyPochebut(score));

    [Theory]
    [InlineData(15, K10Band.Low)]
    [InlineData(16, K10Band.Mild)]
    [InlineData(21, K10Band.Mild)]
    [InlineData(22, K10Band.Moderate)]
    [InlineData(29, K10Band.Moderate)]
    [InlineData(30, K10Band.High)]
    public void ClassifyK10_Boundaries(int score, K10Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyK10(score));

    [Theory]
    [InlineData(12, Who5Band.Low)]
    [InlineData(13, Who5Band.Fair)]
    [InlineData(18, Who5Band.Fair)]
    [InlineData(19, Who5Band.Good)]
    public void ClassifyWho5_Boundaries(int score, Who5Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyWho5(score));

    [Theory]
    [InlineData(7, IsiBand.None)]
    [InlineData(8, IsiBand.Subthreshold)]
    [InlineData(14, IsiBand.Subthreshold)]
    [InlineData(15, IsiBand.Moderate)]
    [InlineData(21, IsiBand.Moderate)]
    [InlineData(22, IsiBand.Severe)]
    public void ClassifyIsi_Boundaries(int score, IsiBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyIsi(score));

    [Theory]
    [InlineData(10, EssBand.Normal)]
    [InlineData(11, EssBand.Mild)]
    [InlineData(12, EssBand.Mild)]
    [InlineData(13, EssBand.Moderate)]
    [InlineData(15, EssBand.Moderate)]
    [InlineData(16, EssBand.Severe)]
    public void ClassifyEss_Boundaries(int score, EssBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyEss(score));

    [Theory]
    [InlineData(4, Phq15Band.Minimal)]
    [InlineData(5, Phq15Band.Low)]
    [InlineData(9, Phq15Band.Low)]
    [InlineData(10, Phq15Band.Moderate)]
    [InlineData(14, Phq15Band.Moderate)]
    [InlineData(15, Phq15Band.High)]
    public void ClassifyPhq15_Boundaries(int score, Phq15Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyPhq15(score));

    [Theory]
    [InlineData(1, ScoffBand.Negative)]
    [InlineData(2, ScoffBand.Positive)]
    public void ClassifyScoff_Boundaries(int score, ScoffBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyScoff(score));

    [Theory]
    [InlineData(9, SwlsBand.ExtremelyDissatisfied)]
    [InlineData(10, SwlsBand.Dissatisfied)]
    [InlineData(19, SwlsBand.SlightlyDissatisfied)]
    [InlineData(20, SwlsBand.Neutral)]
    [InlineData(21, SwlsBand.SlightlySatisfied)]
    [InlineData(30, SwlsBand.Satisfied)]
    [InlineData(31, SwlsBand.ExtremelySatisfied)]
    public void ClassifySwls_Boundaries(int score, SwlsBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifySwls(score));

    [Theory]
    [InlineData(13, Pss10Band.Low)]
    [InlineData(14, Pss10Band.Moderate)]
    [InlineData(26, Pss10Band.Moderate)]
    [InlineData(27, Pss10Band.High)]
    public void ClassifyPss10_Boundaries(int score, Pss10Band expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyPss10(score));

    [Theory]
    [InlineData(2, ScreenBand.Negative)]
    [InlineData(3, ScreenBand.Positive)]
    public void ClassifyPhq2_Boundaries(int score, ScreenBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyPhq2(score));

    [Theory]
    [InlineData(2, ScreenBand.Negative)]
    [InlineData(3, ScreenBand.Positive)]
    public void ClassifyGad2_Boundaries(int score, ScreenBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyGad2(score));

    [Theory]
    [InlineData(7, HadsBand.Normal)]
    [InlineData(8, HadsBand.Borderline)]
    [InlineData(10, HadsBand.Borderline)]
    [InlineData(11, HadsBand.Elevated)]
    public void ClassifyHads_Boundaries(int score, HadsBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyHads(score));

    [Theory]
    [InlineData(14, RsesBand.Low)]
    [InlineData(15, RsesBand.Normal)]
    [InlineData(25, RsesBand.Normal)]
    [InlineData(26, RsesBand.High)]
    public void ClassifyRses_Boundaries(int score, RsesBand expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.ClassifyRses(score));

    [Theory]
    [InlineData(9, RecommendationLane.Low)]
    [InlineData(10, RecommendationLane.High)]
    public void RecommendBeck_Boundaries(int score, RecommendationLane expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.RecommendBeck(score));

    [Theory]
    [InlineData(15, RecommendationLane.Low)]
    [InlineData(16, RecommendationLane.Mid)]
    [InlineData(21, RecommendationLane.Mid)]
    [InlineData(22, RecommendationLane.High)]
    public void RecommendK10_Boundaries(int score, RecommendationLane expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.RecommendK10(score));

    [Theory]
    [InlineData(14, RecommendationLane.High)]
    [InlineData(15, RecommendationLane.Mid)]
    [InlineData(25, RecommendationLane.Mid)]
    [InlineData(26, RecommendationLane.Low)]
    public void RecommendRses_Boundaries(int score, RecommendationLane expected) =>
        Assert.Equal(expected, TestScoreBandClassifier.RecommendRses(score));
}
