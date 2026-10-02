#if ANDROID
using Android.App;
using Android.Content;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
public sealed class ChatReminderBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null ||
            intent?.Action is not (Intent.ActionBootCompleted or ChatReminderConstants.ActionBoot))
        {
            return;
        }

        ReminderSyncWork.Run<IChatReminderCoordinator>(this, coordinator => coordinator.SyncAsync());
    }
}
#endif
