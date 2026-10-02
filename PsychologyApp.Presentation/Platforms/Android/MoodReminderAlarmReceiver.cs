#if ANDROID
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
public sealed class MoodReminderAlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent?.Action != MoodReminderConstants.ActionReminder)
        {
            return;
        }

        string title = intent.GetStringExtra(MoodReminderConstants.ExtraTitle)
            ?? AppStrings.MoodReminderTitle;
        string body = intent.GetStringExtra(MoodReminderConstants.ExtraBody)
            ?? AppStrings.MoodReminderBody;

        AndroidMoodReminderScheduler.EnsureNotificationChannel(context);
        PendingIntent tapIntent = AndroidMoodReminderScheduler.CreateTapPendingIntent(context);
        int smallIcon = context.ApplicationInfo?.Icon ?? Resource.Mipmap.logo;

        var builder = new NotificationCompat.Builder(context, MoodReminderConstants.ChannelId);
        builder.SetContentTitle(title);
        builder.SetContentText(body);
        builder.SetSmallIcon(smallIcon);
        builder.SetAutoCancel(true);
        builder.SetContentIntent(tapIntent);

        Notification? notification = builder.Build();
        NotificationManagerCompat.From(context)?.Notify(MoodReminderConstants.NotificationId, notification);

        ReminderSyncWork.Run<IMoodReminderCoordinator>(this, coordinator => coordinator.SyncAsync());
    }
}
#endif
