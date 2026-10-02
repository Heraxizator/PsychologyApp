#if ANDROID
using Android.App;
using Android.Content;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter([Intent.ActionBootCompleted])]
public sealed class PracticeReminderBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent?.Action != Intent.ActionBootCompleted)
        {
            return;
        }

        ReminderSyncWork.Run<IPracticeReminderCoordinator>(this, coordinator => coordinator.SyncAsync());
    }
}
#endif
