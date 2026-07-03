using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

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
        await _languageContentReloader.EnsureReloadedAsync();
        await _practiceReminderCoordinator.SyncAsync();
        await _dialogService.ShowAsync(AppStrings.SettingsAppliedTitle, AppStrings.SettingsAppliedMessage);
        await _navigationService.GoBackAsync();
    }

    private async Task ReplayOnboardingAsync()
    {
        _userPreferencesStore.ResetOnboardingCompletion();
        await _navigationService.ShowOnboardingAsync();
    }
}
