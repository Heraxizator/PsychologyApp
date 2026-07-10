#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

public sealed class AndroidQuoteReminderScheduler : IQuoteReminderScheduler
{
    public bool IsSupported => true;

    public Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
        {
            return Task.CompletedTask;
        }

        Context context = Platform.AppContext;
        if (ContextCompat.CheckSelfPermission(context, global::Android.Manifest.Permission.PostNotifications)
            == global::Android.Content.PM.Permission.Granted)
        {
            return Task.CompletedTask;
        }

        Activity? activity = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView as Activity;
        if (activity is null)
        {
            return Task.CompletedTask;
        }

        ActivityCompat.RequestPermissions(activity, [global::Android.Manifest.Permission.PostNotifications], requestCode: 9002);
        return Task.CompletedTask;
    }

    public void Cancel()
    {
        Context context = Platform.AppContext;
        AlarmManager? alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
        PendingIntent pendingIntent = CreateAlarmPendingIntent(context, string.Empty, string.Empty);
        alarmManager?.Cancel(pendingIntent);
        pendingIntent.Dispose();

        NotificationManagerCompat.From(context)?.Cancel(QuoteReminderConstants.NotificationId);
    }

    public void Schedule(DateTime fireLocal, string title, string body)
    {
        Context context = Platform.AppContext;
        EnsureNotificationChannel(context);

        AlarmManager? alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
        if (alarmManager is null)
        {
            return;
        }

        PendingIntent pendingIntent = CreateAlarmPendingIntent(context, title, body);
        long triggerAtMillis = new DateTimeOffset(DateTime.SpecifyKind(fireLocal, DateTimeKind.Local)).ToUnixTimeMilliseconds();

        try
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else
            {
                alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
        }
        catch (Java.Lang.SecurityException)
        {
            alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
        }
        finally
        {
            pendingIntent.Dispose();
        }
    }

    internal static void EnsureNotificationChannel(Context context)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        {
            return;
        }

        NotificationManager? notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
        if (notificationManager?.GetNotificationChannel(QuoteReminderConstants.ChannelId) is not null)
        {
            return;
        }

        var channel = new NotificationChannel(
            QuoteReminderConstants.ChannelId,
            "Quote reminders",
            NotificationImportance.Default)
        {
            Description = "Daily quote reminders"
        };
        notificationManager?.CreateNotificationChannel(channel);
    }

    internal static PendingIntent CreateAlarmPendingIntent(Context context, string title, string body)
    {
        Intent intent = new(context, typeof(QuoteReminderAlarmReceiver));
        intent.SetAction(QuoteReminderConstants.ActionReminder);
        intent.PutExtra(QuoteReminderConstants.ExtraTitle, title);
        intent.PutExtra(QuoteReminderConstants.ExtraBody, body);

        return PendingIntent.GetBroadcast(
            context,
            QuoteReminderConstants.AlarmRequestCode,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }

    internal static PendingIntent CreateTapPendingIntent(Context context)
    {
        Intent intent = new(context, typeof(global::PsychologyApp.Presentation.App.MainActivity));
        intent.SetAction(QuoteReminderConstants.ActionOpenFromNotification);
        intent.SetPackage(context.PackageName);
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);

        return PendingIntent.GetActivity(
            context,
            QuoteReminderConstants.NotificationId,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }
}
#endif
