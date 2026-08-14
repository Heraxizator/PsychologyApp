using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

public partial class SettingsViewModel
{
    private async Task RevertAndGoBackAsync()
    {
        CancelPendingAutoSave();
        _userPreferencesStore.ApplyAll();
        await _navigationService.GoBackAsync();
    }

    private async Task ToEndAsync()
    {
        CancelPendingAutoSave();
        await PersistSettingsAsync(CancellationToken.None);
        await _dialogService.ShowAsync(AppStrings.SettingsAppliedTitle, AppStrings.SettingsAppliedMessage);
        await _navigationService.GoBackAsync();
    }

    private async Task ReplayOnboardingAsync()
    {
        _userPreferencesStore.ResetOnboardingCompletion();
        await _navigationService.ShowOnboardingAsync();
    }

    private void QueueAutoSave()
    {
        CancelPendingAutoSave();
        CancellationTokenSource debounce = new();
        _autoSaveDebounceCts = debounce;
        _ = AutoSaveAsync(debounce.Token);
    }

    private void CancelPendingAutoSave()
    {
        _autoSaveDebounceCts?.Cancel();
        _autoSaveDebounceCts?.Dispose();
        _autoSaveDebounceCts = null;
    }

    private async Task AutoSaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(AutoSaveDebounceDelay, cancellationToken);
            await PersistSettingsAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Debounce canceled by next change or explicit manual save.
        }
    }

    private async Task PersistSettingsAsync(CancellationToken cancellationToken)
    {
        await _saveLock.WaitAsync(cancellationToken);
        try
        {
            _userPreferencesStore.Save(BuildCurrentState());
            _savedState = _userPreferencesStore.Load();
            _userPreferencesStore.ApplyAll();
            await _languageContentReloader.EnsureReloadedAsync();
            await _practiceReminderCoordinator.SyncAsync();
            await _quoteReminderCoordinator.SyncAsync();
            await _moodReminderCoordinator.SyncAsync();
        }
        finally
        {
            _saveLock.Release();
        }
    }
}
