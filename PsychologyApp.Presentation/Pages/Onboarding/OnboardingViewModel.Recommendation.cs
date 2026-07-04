using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.Onboarding;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public string RecommendedIconName => _recommendation.IconName;
    public string RecommendedTitle => _recommendation.Title;
    public string RecommendedSubtitle => _recommendation.Subtitle;
    public string RecommendedReason => _recommendation.ReasonText;

    private OnboardingRecommendationResult _recommendation = new()
    {
        TechniqueId = TechniqueId.Spin,
        Concern = OnboardingConcernKeys.Explore,
        IconName = string.Empty,
        Title = string.Empty,
        Subtitle = string.Empty,
        ReasonText = string.Empty
    };

    private void RefreshRecommendation() => RefreshRecommendationAsync().FireAndForget();

    private async Task RefreshRecommendationAsync()
    {
        _recommendation = await _onboardingRecommendationResolver.ResolveAsync(SelectedConcern);
        Notify(
            nameof(RecommendedIconName),
            nameof(RecommendedTitle),
            nameof(RecommendedSubtitle),
            nameof(RecommendedReason),
            nameof(FinishSubtitle));
    }

    private void NotifyRecommendation() => RefreshRecommendation();
}
