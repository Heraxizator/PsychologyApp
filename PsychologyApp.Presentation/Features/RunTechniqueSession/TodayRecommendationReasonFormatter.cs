using PsychologyApp.Application.Recommendations;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public static class TodayRecommendationReasonFormatter
{
    public static string Format(TodayRecommendationDecision decision, TodayRecommendationContext context) =>
        decision.Source switch
        {
            TodayRecommendationSource.SessionDraft => AppStrings.TodayRecommendationReasonContinueDraft(),
            TodayRecommendationSource.RecentTest => FormatTestReason(decision, context),
            TodayRecommendationSource.LowMood => AppStrings.TodayRecommendationReasonLowMood(),
            _ => AppStrings.TodayRecommendationReason(context.Concern)
        };

    private static string FormatTestReason(TodayRecommendationDecision decision, TodayRecommendationContext context)
    {
        string testId = decision.TestId ?? context.RecentTestResult?.TestId ?? string.Empty;
        if (context.RecentTestResult is { Score: int score }
            && string.Equals(context.RecentTestResult.TestId, testId, StringComparison.Ordinal))
        {
            string? human = TestScoreLabelMapper.GetRecommendationReason(testId, score);
            if (!string.IsNullOrWhiteSpace(human))
            {
                return human;
            }
        }

        return AppStrings.TodayRecommendationReasonFromTest(testId);
    }
}
