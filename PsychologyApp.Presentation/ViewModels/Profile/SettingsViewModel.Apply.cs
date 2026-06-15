using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Preferences;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class SettingsViewModel
{
    private async Task RevertAndGoBackAsync()
    {
        _userPreferencesStore.ApplyAll();
        await _navigationService.GoBackAsync();
    }

    private async Task ToEndAsync()
    {
        _userPreferencesStore.Save(BuildCurrentState());
        _savedState = _userPreferencesStore.Load();
        _userPreferencesStore.ApplyAll();
        await _dialogService.ShowAsync(AppStrings.SettingsAppliedTitle, AppStrings.SettingsAppliedMessage);
        await _navigationService.GoBackAsync();
    }

    private async Task ReplayOnboardingAsync()
    {
        _userPreferencesStore.ResetOnboardingCompletion();
        await _navigationService.ShowOnboardingAsync();
    }
}
