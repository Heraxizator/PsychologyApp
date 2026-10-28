using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

public partial class SettingsViewModel
{
    /// <summary>
    /// Every choice shows at once and is kept; leaving keeps a change still waiting for its save. Leaving used to undo it, so a theme
    /// picked just before going back was lost, and the separate "Apply" button and its dialog only repeated what had happened.
    /// </summary>
    private async Task SaveAndGoBackAsync()
    {
        bool pending = _autoSaveDebounceCts is not null;
        CancelPendingAutoSave();
        if (pending)
        {
            await PersistSettingsAsync(CancellationToken.None);
        }

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
            await _chatReminderCoordinator.SyncAsync();
        }
        finally
        {
            _saveLock.Release();
        }
    }
}
