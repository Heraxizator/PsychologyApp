#if ANDROID
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
public sealed class ChatReminderAlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent?.Action != ChatReminderConstants.ActionReminder)
        {
            return;
        }

        string title = intent.GetStringExtra(ChatReminderConstants.ExtraTitle)
            ?? AppStrings.ChatReminderTitle;
        string body = intent.GetStringExtra(ChatReminderConstants.ExtraBody)
            ?? AppStrings.ChatReminderBody;

        AndroidChatReminderScheduler.EnsureNotificationChannel(context);
        PendingIntent tapIntent = AndroidChatReminderScheduler.CreateTapPendingIntent(context);
        int smallIcon = context.ApplicationInfo?.Icon ?? Resource.Mipmap.logo;

        var builder = new NotificationCompat.Builder(context, ChatReminderConstants.ChannelId);
        builder.SetContentTitle(title);
        builder.SetContentText(body);
        builder.SetSmallIcon(smallIcon);
        builder.SetAutoCancel(true);
        builder.SetContentIntent(tapIntent);

        Notification? notification = builder.Build();
        NotificationManagerCompat.From(context)?.Notify(ChatReminderConstants.NotificationId, notification);

        RescheduleNext();
    }

    private static void RescheduleNext()
    {
        try
        {
            IServiceProvider? services = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services;
            if (services?.GetService<IChatReminderCoordinator>() is not IChatReminderCoordinator coordinator)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await coordinator.SyncAsync().ConfigureAwait(false);
                }
                catch
                {
                }
            });
        }
        catch
        {
        }
    }
}
#endif
