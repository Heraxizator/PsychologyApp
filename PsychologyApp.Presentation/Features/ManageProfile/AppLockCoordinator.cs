using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Pages.ManageProfile.AppLock;
using PsychologyApp.Presentation.Shared.Services.Security;

namespace PsychologyApp.Presentation.Features.ManageProfile;

/// <summary>
/// Puts the lock screen in front of the app at start and after an absence (<see cref="AppLockPolicy"/>), and takes it away once the
/// phone's unlock is passed. The switch is kept under its own preference key, apart from the visual settings: saving a theme
/// rewrites those as a whole, and the lock must never be turned off as a side effect.
/// </summary>
public sealed class AppLockCoordinator(IDeviceAuthenticator authenticator, TimeProvider time, ILogger<AppLockCoordinator> logger) : IAppLock
{
    private const string EnabledKey = "AppLockEnabled";

    private bool _unlocked;
    private DateTime? _leftAtUtc;
    private AppLockPage? _shown;

    public bool IsEnabled => Preferences.Default.Get(EnabledKey, false);

    public DeviceUnlockAvailability Availability => authenticator.Availability;

    public async Task<bool> TryEnableAsync()
    {
        if (Availability != DeviceUnlockAvailability.Available
            || !await authenticator.AuthenticateAsync(AppStrings.AppLockEnableTitle, AppStrings.AppLockPromptSubtitle))
        {
            return false;
        }

        Preferences.Default.Set(EnabledKey, true);
        _unlocked = true;
        ApplyRecentsPreview();
        return true;
    }

    public void Disable()
    {
        Preferences.Default.Set(EnabledKey, false);
        ApplyRecentsPreview();
    }

    /// <summary>The app went to the background.</summary>
    public void OnLeft()
    {
        if (_unlocked)
        {
            _leftAtUtc = time.GetUtcNow().UtcDateTime;
        }
    }

    /// <summary>At start and on every return: shows the lock screen when it is due and waits until it is passed.</summary>
    public async Task GuardAsync()
    {
        ApplyRecentsPreview();
        if (_shown is not null)
        {
            return;
        }

        if (!AppLockPolicy.NeedsUnlock(IsEnabled, _unlocked, _leftAtUtc, time.GetUtcNow().UtcDateTime))
        {
            _leftAtUtc = null;
            return;
        }

        INavigation? navigation = await WaitForWindowAsync();
        if (navigation is null)
        {
            logger.LogError("No window to show the lock screen in");
            return;
        }

        AppLockPage page = new(new AppLockViewModel(authenticator));
        _shown = page;
        try
        {
            await navigation.PushModalAsync(page, animated: false);
            await page.Unlocked;
            // Nothing is expected above it: onboarding and the safety gate start only after the lock is passed.
            if (navigation.ModalStack.Count > 0 && ReferenceEquals(navigation.ModalStack[^1], page))
            {
                await navigation.PopModalAsync(animated: false);
            }
            else
            {
                logger.LogWarning("The lock screen was not on top when it was passed");
            }

            _unlocked = true;
            _leftAtUtc = null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "The lock screen could not be shown or closed");
        }
        finally
        {
            _shown = null;
        }
    }

    private const int WindowWaitStepMs = 50;
    private const int WindowWaitSteps = 60;

    /// <summary>
    /// The start-up check runs from the shell's constructor, before the shell is in a window. Giving up there would quietly leave
    /// the app open, so it waits (up to three seconds) for the window to be ready.
    /// </summary>
    private static async Task<INavigation?> WaitForWindowAsync()
    {
        for (int step = 0; step < WindowWaitSteps; step++)
        {
            if (Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page is { Handler: not null } page)
            {
                return page.Navigation;
            }

            await Task.Delay(WindowWaitStepMs);
        }

        return null;
    }

    /// <summary>With the lock on, the recent-apps list shows no picture of the last screen (Android 13+; screenshots still work).</summary>
    private void ApplyRecentsPreview()
    {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            Platform.CurrentActivity?.SetRecentsScreenshotEnabled(!IsEnabled);
        }
#endif
    }
}
