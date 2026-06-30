using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    private async Task StartPracticeAsync()
    {
        string concern = string.IsNullOrEmpty(SelectedConcern) ? OnboardingConcernKeys.Explore : SelectedConcern;
        _userPreferencesStore.CompleteOnboarding(concern);
        TechniqueId techniqueId = _techniqueRecommendationService.ResolveFromOnboardingConcern(concern);
        await _onCompleted(techniqueId);
    }

    private async Task CompleteWithoutPracticeAsync()
    {
        string concern = string.IsNullOrEmpty(SelectedConcern) ? OnboardingConcernKeys.Explore : SelectedConcern;
        _userPreferencesStore.CompleteOnboarding(concern);
        await _onCompleted(null);
    }
}
