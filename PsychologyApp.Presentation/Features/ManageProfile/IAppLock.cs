using PsychologyApp.Presentation.Shared.Services.Security;

namespace PsychologyApp.Presentation.Features.ManageProfile;

/// <summary>The switch for "ask for the phone's unlock before showing the app", as the data screen offers it.</summary>
public interface IAppLock
{
    bool IsEnabled { get; }

    DeviceUnlockAvailability Availability { get; }

    /// <summary>Turns the lock on after the person passes the phone's unlock once; false when it could not be confirmed.</summary>
    Task<bool> TryEnableAsync();

    void Disable();
}
