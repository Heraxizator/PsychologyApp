using PsychologyApp.Application.Recommendations;
using PsychologyApp.Presentation.Shared.Lib.Recommendations;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class TodayRecommendationReasonFormatterAdapter : ITodayRecommendationReasonFormatter
{
    public string Format(TodayRecommendationDecision decision, TodayRecommendationContext context) =>
        TodayRecommendationReasonFormatter.Format(decision, context);
}
