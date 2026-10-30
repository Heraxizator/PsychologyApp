#if ANDROID
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;

namespace PsychologyApp.Presentation.Platforms.Android;

internal static class AndroidNotificationPermission
{
    private static readonly SemaphoreSlim RequestGate = new(1, 1);
    private static bool _deniedThisSession;

    internal static bool CanPost(Context context) =>
        (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu
            || ContextCompat.CheckSelfPermission(context, global::Android.Manifest.Permission.PostNotifications)
                == global::Android.Content.PM.Permission.Granted)
        && NotificationManagerCompat.From(context)?.AreNotificationsEnabled() == true;

    internal static async Task<bool> RequestIfNeededAsync(CancellationToken cancellationToken)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
        {
            return NotificationManagerCompat.From(Platform.AppContext)?.AreNotificationsEnabled() == true;
        }

        await RequestGate.WaitAsync(cancellationToken);
        try
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status == PermissionStatus.Granted)
            {
                _deniedThisSession = false;
                return CanPost(Platform.AppContext);
            }

            // Boot and alarm receivers also sync reminders; they must never open a permission dialog over background work.
            if (_deniedThisSession
                || !MainThread.IsMainThread
                || Platform.CurrentActivity is null)
            {
                return false;
            }

            status = await MainThread.InvokeOnMainThreadAsync(
                () => Permissions.RequestAsync<Permissions.PostNotifications>());
            _deniedThisSession = status != PermissionStatus.Granted;
            return status == PermissionStatus.Granted
                && CanPost(Platform.AppContext);
        }
        finally
        {
            RequestGate.Release();
        }
    }
}
#endif
