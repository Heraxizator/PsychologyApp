#if ANDROID
using Android.App;
using Android.Content;
using Android.Hardware.Biometrics;
using Android.OS;
using PsychologyApp.Presentation.Shared.Services.Security;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// The system BiometricPrompt with the phone's own PIN, pattern or password allowed as well, so a person without a fingerprint
/// sensor (or with wet hands) is never stuck. It is part of the platform since Android 10, so no extra library is needed.
/// </summary>
public sealed class AndroidDeviceAuthenticator : IDeviceAuthenticator
{
    public DeviceUnlockAvailability Availability
    {
        get
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                return DeviceUnlockAvailability.Unsupported;
            }

            KeyguardManager? keyguard = Platform.AppContext.GetSystemService(Context.KeyguardService) as KeyguardManager;
            return keyguard?.IsDeviceSecure == true ? DeviceUnlockAvailability.Available : DeviceUnlockAvailability.NoScreenLock;
        }
    }

    public Task<bool> AuthenticateAsync(string title, string subtitle)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29) || Platform.CurrentActivity is not Activity activity)
        {
            return Task.FromResult(false);
        }

        TaskCompletionSource<bool> result = new(TaskCreationOptions.RunContinuationsAsynchronously);
        BiometricPrompt.Builder builder = new BiometricPrompt.Builder(activity)
            .SetTitle(title)
            .SetSubtitle(subtitle);
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            builder.SetAllowedAuthenticators((int)(BiometricManagerAuthenticators.BiometricWeak | BiometricManagerAuthenticators.DeviceCredential));
        }
        else
        {
#pragma warning disable CA1422 // The replacement, SetAllowedAuthenticators, exists only from Android 11; this branch is Android 10.
            builder.SetDeviceCredentialAllowed(true);
#pragma warning restore CA1422
        }

        builder.Build().Authenticate(new CancellationSignal(), activity.MainExecutor!, new Callback(result));
        return result.Task;
    }

    private sealed class Callback(TaskCompletionSource<bool> result) : BiometricPrompt.AuthenticationCallback
    {
        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult? authenticationResult) =>
            result.TrySetResult(true);

        // Cancelled, too many attempts, no credential: the lock screen stays and offers to try again.
        public override void OnAuthenticationError(BiometricErrorCode errorCode, Java.Lang.ICharSequence? errString) =>
            result.TrySetResult(false);

        // A finger that was not recognised: the system prompt stays open for another try, so nothing is decided yet.
        public override void OnAuthenticationFailed()
        {
        }
    }
}
#endif
