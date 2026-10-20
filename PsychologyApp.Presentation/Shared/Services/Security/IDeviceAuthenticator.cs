namespace PsychologyApp.Presentation.Shared.Services.Security;

public enum DeviceUnlockAvailability
{
    Available,

    /// <summary>The phone has no screen lock, so there is nothing to confirm the person with.</summary>
    NoScreenLock,

    /// <summary>This platform or Android version (before 10) cannot show the system prompt the app relies on.</summary>
    Unsupported
}

/// <summary>
/// The phone's own unlock (fingerprint, face, or the phone's PIN, pattern or password) shown by the system. The app keeps no
/// PIN of its own: there is nothing to forget and nothing to store.
/// </summary>
public interface IDeviceAuthenticator
{
    DeviceUnlockAvailability Availability { get; }

    /// <summary>True once the person has passed the phone's unlock; false when they cancelled it or the system refused.</summary>
    Task<bool> AuthenticateAsync(string title, string subtitle);
}

public sealed class UnavailableDeviceAuthenticator : IDeviceAuthenticator
{
    public DeviceUnlockAvailability Availability => DeviceUnlockAvailability.Unsupported;

    public Task<bool> AuthenticateAsync(string title, string subtitle) => Task.FromResult(false);
}
