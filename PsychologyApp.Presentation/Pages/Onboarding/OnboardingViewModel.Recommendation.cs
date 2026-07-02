using PsychologyApp.Presentation.Features.Onboarding;
using PsychologyApp.Presentation.Shared.Common;
namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public string RecommendedIconName => _recommendation.IconName;
    public string RecommendedTitle => _recommendation.Title;
    public string RecommendedSubtitle => _recommendation.Subtitle;
    public string RecommendedReason => _recommendation.ReasonText;

    private OnboardingRecommendationResult _recommendation = null!;

    private void RefreshRecommendation() =>
        _recommendation = _onboardingRecommendationResolver.Resolve(SelectedConcern);

    private void NotifyRecommendation()
    {
        RefreshRecommendation();
        Notify(
            nameof(RecommendedIconName),
            nameof(RecommendedTitle),
            nameof(RecommendedSubtitle),
            nameof(RecommendedReason),
            nameof(FinishSubtitle));
    }
}
