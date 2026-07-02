#if ANDROID
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class PracticeReminderAlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent?.Action != PracticeReminderConstants.ActionReminder)
        {
            return;
        }

        string? techniqueValue = intent.GetStringExtra(PracticeReminderConstants.ExtraTechniqueId);
        if (!Enum.TryParse(techniqueValue, out TechniqueId techniqueId))
        {
            techniqueId = TechniqueId.Spin;
        }

        string title = intent.GetStringExtra(PracticeReminderConstants.ExtraTitle)
            ?? "Psychology App";
        string body = intent.GetStringExtra(PracticeReminderConstants.ExtraBody)
            ?? string.Empty;

        ShowNotification(context, techniqueId, title, body);
        ScheduleNextFromCoordinator();
    }

    private static void ShowNotification(Context context, TechniqueId techniqueId, string title, string body)
    {
        AndroidPracticeReminderScheduler.EnsureNotificationChannel(context);

        PendingIntent tapIntent = AndroidPracticeReminderScheduler.CreateTapPendingIntent(context, techniqueId);
        int smallIcon = context.ApplicationInfo?.Icon ?? Resource.Mipmap.logo;
        var builder = new NotificationCompat.Builder(context, PracticeReminderConstants.ChannelId);
        builder.SetContentTitle(title);
        builder.SetContentText(body);
        builder.SetSmallIcon(smallIcon);
        builder.SetAutoCancel(true);
        builder.SetContentIntent(tapIntent);
        builder.SetPriority((int)NotificationPriority.Default);

        Notification notification = builder.Build()!;
        NotificationManagerCompat.From(context)?.Notify(PracticeReminderConstants.NotificationId, notification);
        tapIntent.Dispose();
    }

    private static void ScheduleNextFromCoordinator()
    {
        try
        {
            IServiceProvider? services = MauiApplication.Current?.Services;
            if (services?.GetService<IPracticeReminderCoordinator>() is not IPracticeReminderCoordinator coordinator)
            {
                return;
            }

            _ = coordinator.SyncAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Practice reminder reschedule failed: {ex.Message}");
        }
    }
}
#endif
