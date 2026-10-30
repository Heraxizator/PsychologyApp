#if ANDROID
using Android.App;
using Android.Content;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
[IntentFilter([Intent.ActionBootCompleted, Intent.ActionTimeChanged, Intent.ActionTimezoneChanged, ReminderSyncWork.ActionTimeZoneOffsetChanged])]
public sealed class ChatReminderBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null ||
            intent?.Action is not (Intent.ActionBootCompleted
                or Intent.ActionTimeChanged
                or Intent.ActionTimezoneChanged
                or ReminderSyncWork.ActionTimeZoneOffsetChanged
                or ChatReminderConstants.ActionBoot))
        {
            return;
        }

        ReminderSyncWork.Run<IChatReminderCoordinator>(this, coordinator => coordinator.SyncAsync());
    }
}
#endif
