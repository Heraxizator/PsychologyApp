namespace PsychologyApp.Presentation.Shared.Services.Security;

public enum DeviceUnlockAvailability
{
    Available,

    /// <summary>The phone has no screen lock, so there is nothing to confirm the person with.</summary>
    NoScreenLock,

    /// <summary>This platform or Android version (before 10) cannot show the system prompt the app relies on.</summary>
    Unsupported
}

public enum DeviceUnlockResult
{
    /// <summary>The person passed the phone's unlock.</summary>
    Passed,

    /// <summary>The person cancelled, or the attempts ran out: the app stays closed and offers to try again.</summary>
    NotPassed,

    /// <summary>
    /// The system could not ask at all (the screen lock is gone, the sensor is broken, the prompt failed to open). The lock must
    /// not turn into a wall between the person and their own records, so this lets them in.
    /// </summary>
    CannotAsk
}

/// <summary>
/// The phone's own unlock (fingerprint, face, or the phone's PIN, pattern or password) shown by the system. The app keeps no
/// PIN of its own: there is nothing to forget and nothing to store.
/// </summary>
public interface IDeviceAuthenticator
{
    DeviceUnlockAvailability Availability { get; }

    Task<DeviceUnlockResult> AuthenticateAsync(string title, string subtitle);
}

public sealed class UnavailableDeviceAuthenticator : IDeviceAuthenticator
{
    public DeviceUnlockAvailability Availability => DeviceUnlockAvailability.Unsupported;

    public Task<DeviceUnlockResult> AuthenticateAsync(string title, string subtitle) => Task.FromResult(DeviceUnlockResult.CannotAsk);
}
