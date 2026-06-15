using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.ViewModels.Onboarding;

public partial class OnboardingViewModel
{
    private async Task StartPracticeAsync()
    {
        _userPreferencesStore.CompleteOnboarding(SelectedConcern);
        TechniqueId techniqueId = OnboardingRecommendation.ResolveTechnique(SelectedConcern);
        await _onCompleted(techniqueId);
    }

    private async Task CompleteWithoutPracticeAsync()
    {
        _userPreferencesStore.CompleteOnboarding(SelectedConcern);
        await _onCompleted(null);
    }
}
