using System.Windows.Input;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Security;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ManageProfile.AppLock;

/// <summary>
/// The screen in front of the app while it is locked. Help is reachable without unlocking (the emergency number and the
/// helpline are public); nothing of the person's own (the safety plan, the journal) is shown before the phone's unlock.
/// </summary>
public sealed class AppLockViewModel : BaseViewModel
{
    private readonly IDeviceAuthenticator _authenticator;
    private readonly TaskCompletionSource _unlocked = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _asking;

    public AppLockViewModel(IDeviceAuthenticator authenticator)
    {
        _authenticator = authenticator;
        UnlockCommand = new AsyncCommand(UnlockAsync);
        CallEmergencyCommand = new AsyncCommand(() => PhoneCall.DialAsync(AppStrings.CrisisHubEmergencyNumber));
        CallHotlineCommand = new AsyncCommand(() => PhoneCall.DialAsync(AppStrings.CrisisHubHotlineRuNumber));
    }

    /// <summary>Completes once the phone's unlock was passed.</summary>
    public Task Unlocked => _unlocked.Task;

    public string TitleText => AppStrings.AppLockTitle;
    public string LeadText => AppStrings.AppLockLead;
    public string UnlockText => AppStrings.AppLockUnlock;
    public string HelpTitle => AppStrings.AppLockHelpTitle;
    public string CallEmergencyText => AppStrings.CrisisHubCallEmergency;
    public string CallHotlineText => AppStrings.CrisisHubCallHotlineRu;

    public ICommand UnlockCommand { get; }
    public ICommand CallEmergencyCommand { get; }
    public ICommand CallHotlineCommand { get; }

    protected override void RefreshLocalizedProperties() =>
        Notify(nameof(TitleText), nameof(LeadText), nameof(UnlockText), nameof(HelpTitle), nameof(CallEmergencyText), nameof(CallHotlineText));

    private async Task UnlockAsync()
    {
        // A second tap while the system prompt is already up would stack another prompt behind it.
        if (_asking)
        {
            return;
        }

        _asking = true;
        try
        {
            // When the phone cannot confirm anyone (its screen lock was removed since, or the system prompt cannot open), there is
            // nothing left to check with, and the person must never be shut out of their own records.
            if (_authenticator.Availability != DeviceUnlockAvailability.Available
                || await _authenticator.AuthenticateAsync(AppStrings.AppLockPromptTitle, AppStrings.AppLockPromptSubtitle)
                    is DeviceUnlockResult.Passed or DeviceUnlockResult.CannotAsk)
            {
                _unlocked.TrySetResult();
            }
        }
        finally
        {
            _asking = false;
        }
    }
}
