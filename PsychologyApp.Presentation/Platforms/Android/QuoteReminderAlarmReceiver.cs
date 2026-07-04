#if ANDROID
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
public sealed class QuoteReminderAlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent?.Action != QuoteReminderConstants.ActionReminder)
        {
            return;
        }

        string title = intent.GetStringExtra(QuoteReminderConstants.ExtraTitle)
            ?? AppStrings.QuoteReminderTitle;
        string body = intent.GetStringExtra(QuoteReminderConstants.ExtraBody)
            ?? AppStrings.QuoteReminderBody;

        AndroidQuoteReminderScheduler.EnsureNotificationChannel(context);
        PendingIntent tapIntent = AndroidQuoteReminderScheduler.CreateTapPendingIntent(context);
        int smallIcon = context.ApplicationInfo?.Icon ?? Resource.Mipmap.logo;

        var builder = new NotificationCompat.Builder(context, QuoteReminderConstants.ChannelId);
        builder.SetContentTitle(title);
        builder.SetContentText(body);
        builder.SetSmallIcon(smallIcon);
        builder.SetAutoCancel(true);
        builder.SetContentIntent(tapIntent);

        Notification? notification = builder.Build();
        NotificationManagerCompat.From(context)?.Notify(QuoteReminderConstants.NotificationId, notification);

        RescheduleNext(context);
    }

    private static void RescheduleNext(Context context)
    {
        try
        {
            IServiceProvider? services = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services;
            if (services?.GetService<IQuoteReminderCoordinator>() is not IQuoteReminderCoordinator coordinator)
            {
                return;
            }

            coordinator.SyncAsync().GetAwaiter().GetResult();
        }
        catch
        {
        }
    }
}
#endif
