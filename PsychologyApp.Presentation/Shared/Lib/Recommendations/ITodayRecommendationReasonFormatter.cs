using PsychologyApp.Application.Recommendations;

namespace PsychologyApp.Presentation.Shared.Lib.Recommendations;

/// <summary>
/// Port for formatting today's technique recommendation reason text.
/// Implemented by RunTechniqueSession so Shared notification code stays slice-free.
/// </summary>
public interface ITodayRecommendationReasonFormatter
{
    string Format(TodayRecommendationDecision decision, TodayRecommendationContext context);
}
