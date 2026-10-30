using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Pages.ManageProfile.AppLock;
using PsychologyApp.Presentation.Shared.Services.Security;

namespace PsychologyApp.Presentation.Features.ManageProfile;

/// <summary>
/// Puts the lock screen in front of the app at start and after an absence (<see cref="AppLockPolicy"/>), and takes it away once the
/// phone's unlock is passed. The switch is kept under its own preference key, apart from the visual settings: saving a theme
/// rewrites those as a whole, and the lock must never be turned off as a side effect.
/// </summary>
public sealed class AppLockCoordinator(IDeviceAuthenticator authenticator, ILogger<AppLockCoordinator> logger) : IAppLock
{
    private const string EnabledKey = "AppLockEnabled";
    private const int WindowWaitStepMs = 50;
    private const int WindowWaitSteps = 60;

    private bool _unlocked;
    private long? _leftAtMs;

    // The check in progress. The start-up check and a return to the app can overlap (both wait for the window), and two of them
    // would put two lock screens on top of each other.
    private Task? _guard;

    public bool IsEnabled => Preferences.Default.Get(EnabledKey, false);

    public DeviceUnlockAvailability Availability => authenticator.Availability;

    public async Task<bool> TryEnableAsync()
    {
        if (Availability != DeviceUnlockAvailability.Available
            || await authenticator.AuthenticateAsync(AppStrings.AppLockEnableTitle, AppStrings.AppLockPromptSubtitle) != DeviceUnlockResult.Passed)
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
            _leftAtMs = SinceBootMs();
        }
    }

    /// <summary>At start and on every return: shows the lock screen when it is due and waits until it is passed.</summary>
    /// <summary>
    /// True while the app's own content must stay out of sight: from the moment the lock is found due (before the first frame of a
    /// start or a return) until it is passed. The shell hides its content then, so nothing flashes before the lock screen.
    /// </summary>
    public bool IsCovering { get; private set; }

    public event EventHandler? CoveringChanged;

    public Task GuardAsync()
    {
        // Runs on the UI thread (the shell's constructor, App.OnResume), so this check and the assignment cannot interleave.
        if (_guard is { IsCompleted: false } running)
        {
            return running;
        }

        // Decided here, synchronously, so the content is covered before the next frame is drawn.
        TimeSpan? awayFor = _leftAtMs is long left ? TimeSpan.FromMilliseconds(SinceBootMs() - left) : null;
        bool due = AppLockPolicy.NeedsUnlock(IsEnabled, _unlocked, awayFor);
        if (due)
        {
            SetCovering(true);
        }

        return _guard = GuardCoreAsync(due);
    }

    private async Task GuardCoreAsync(bool due)
    {
        INavigation? navigation = await WaitForWindowAsync();

        // Applied once there is an activity: at a cold start the shell is built before it.
        ApplyRecentsPreview();
        if (!due)
        {
            _leftAtMs = null;
            return;
        }

        try
        {
            if (navigation is null)
            {
                logger.LogError("No window to show the lock screen in");
                return;
            }

            AppLockPage page = new(new AppLockViewModel(authenticator));
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
            _leftAtMs = null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "The lock screen could not be shown or closed");
        }
        finally
        {
            // Also when the lock screen could not be shown: a blank app would shut the person out of their records.
            SetCovering(false);
        }
    }

    private void SetCovering(bool covering)
    {
        if (IsCovering == covering)
        {
            return;
        }

        IsCovering = covering;
        CoveringChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Time since the phone started, counting deep sleep. The wall clock can be set back by whoever holds the phone, and a plain
    /// monotonic clock stops while the phone sleeps (two hours in a pocket would read as seconds); this one does neither.
    /// </summary>
    private static long SinceBootMs()
    {
#if ANDROID
        return global::Android.OS.SystemClock.ElapsedRealtime();
#else
        return Environment.TickCount64;
#endif
    }

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
            try
            {
                Platform.CurrentActivity?.SetRecentsScreenshotEnabled(!IsEnabled);
            }
            catch (Exception ex)
            {
                // A failed recents-preview update must not block the lock screen or leave the app covered.
                logger.LogWarning(ex, "Could not update the recent-apps preview");
            }
        }
#endif
    }
}
