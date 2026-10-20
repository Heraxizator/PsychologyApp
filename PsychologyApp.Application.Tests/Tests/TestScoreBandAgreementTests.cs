using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

/// <summary>
/// The cut-offs live twice: the band index stored with a result and the band whose text the person reads ("0-28 — low").
/// A score on the border must land in the same band in both, or one screen contradicts the other.
/// </summary>
public sealed class TestScoreBandAgreementTests
{
    private static readonly Dictionary<string, Func<int, int>> Classifiers = new()
    {
        ["beck"] = score => (int)TestScoreBandClassifier.ClassifyBeck(score),
        ["heck_hess"] = score => (int)TestScoreBandClassifier.ClassifyHeckHess(score),
        ["haer"] = score => (int)TestScoreBandClassifier.ClassifyHaer(score),
        ["pochebut"] = score => (int)TestScoreBandClassifier.ClassifyPochebut(score),
        ["gad7"] = score => (int)TestScoreBandClassifier.ClassifyGad7(score),
        ["k10"] = score => (int)TestScoreBandClassifier.ClassifyK10(score),
        ["who5"] = score => (int)TestScoreBandClassifier.ClassifyWho5(score),
        ["phq9"] = score => (int)TestScoreBandClassifier.ClassifyPhq9(score),
        ["isi"] = score => (int)TestScoreBandClassifier.ClassifyIsi(score),
        ["ess"] = score => (int)TestScoreBandClassifier.ClassifyEss(score),
        ["phq15"] = score => (int)TestScoreBandClassifier.ClassifyPhq15(score),
        ["scoff"] = score => (int)TestScoreBandClassifier.ClassifyScoff(score),
        ["pss10"] = score => (int)TestScoreBandClassifier.ClassifyPss10(score),
        ["phq2"] = score => (int)TestScoreBandClassifier.ClassifyPhq2(score),
        ["gad2"] = score => (int)TestScoreBandClassifier.ClassifyGad2(score),
        ["pcptsd5"] = score => (int)TestScoreBandClassifier.ClassifyPcPtsd5(score),
        ["pcl5"] = score => (int)TestScoreBandClassifier.ClassifyPcl5(score),
        ["hads_a"] = score => (int)TestScoreBandClassifier.ClassifyHads(score),
        ["hads_d"] = score => (int)TestScoreBandClassifier.ClassifyHads(score),
        ["rses"] = score => (int)TestScoreBandClassifier.ClassifyRses(score),
    };

    public static TheoryData<string> Analyzers => [.. Classifiers.Keys];

    [Theory]
    [MemberData(nameof(Analyzers))]
    public void Every_score_falls_in_the_same_band_on_both_sides(string analyzerId)
    {
        ScoreRange range = TestScoreInterpreter.GetScoreRange(analyzerId)!.Value;
        for (int score = range.Min; score <= range.Max; score++)
        {
            Assert.True(
                Classifiers[analyzerId](score) == TestScoreInterpreter.GetBandIndex(analyzerId, score),
                $"{analyzerId}: a score of {score} is band {TestScoreInterpreter.GetBandIndex(analyzerId, score)} in the stored result and band {Classifiers[analyzerId](score)} in the text.");
        }
    }
}
