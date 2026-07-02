using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.Onboarding;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    private async Task StartPracticeAsync()
    {
        OnboardingRecommendationResult recommendation = _onboardingRecommendationResolver.Resolve(SelectedConcern);
        _userPreferencesStore.CompleteOnboarding(recommendation.Concern);
        await _onCompleted(recommendation.TechniqueId);
    }

    private async Task CompleteWithoutPracticeAsync()
    {
        OnboardingRecommendationResult recommendation = _onboardingRecommendationResolver.Resolve(SelectedConcern);
        _userPreferencesStore.CompleteOnboarding(recommendation.Concern);
        await _onCompleted(null);
    }
}
