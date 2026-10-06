using System.Text.RegularExpressions;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed partial class TestScoreLabelCompletenessTests
{
    private static readonly string[] AnalyzerIds =
    [
        "heck_hess", "haer", "pochebut", "beck", "gad7", "k10", "who5", "phq9", "isi", "ess",
        "phq15", "scoff", "swls", "pss10", "phq2", "gad2", "hads_a", "hads_d", "rses"
    ];

    // A missing resource returns its key ("BeckScore.BeckBand.None"), which would reach the person as a label.
    [GeneratedRegex(@"^[A-Za-z0-9_]+(\.[A-Za-z0-9_]+)+$")]
    private static partial Regex LooksLikeAKey();

    [Theory]
    [InlineData("ru")]
    [InlineData("en")]
    public void EveryScoreOfEveryQuestionnaireHasReadableSummaryAndDetail(string language)
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            AppStrings.LanguageOverride = language;
            foreach (string id in AnalyzerIds)
            {
                for (int score = 0; score <= 120; score++)
                {
                    foreach (string? text in new[] { TestScoreLabelMapper.GetSummary(id, score), TestScoreLabelMapper.GetDetail(id, score), TestScoreLabelMapper.GetRecommendationReason(id, score) })
                    {
                        Assert.False(string.IsNullOrWhiteSpace(text), $"{id} {score} {language}: empty");
                        Assert.False(LooksLikeAKey().IsMatch(text!), $"{id} {score} {language}: shows a resource key: {text}");
                    }
                }
            }
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }

    [Fact]
    public void AnUnknownQuestionnaireHasNoLabel()
    {
        Assert.Null(TestScoreLabelMapper.GetSummary("nope", 5));
        Assert.Null(TestScoreLabelMapper.GetDetail(null, 5));
    }
}
