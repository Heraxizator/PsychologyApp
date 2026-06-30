#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

public sealed class AndroidPracticeReminderScheduler : IPracticeReminderScheduler
{
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

        ActivityCompat.RequestPermissions(activity, [global::Android.Manifest.Permission.PostNotifications], requestCode: 9001);
        return Task.CompletedTask;
    }

    public void Cancel()
    {
        Context context = Platform.AppContext;
        AlarmManager? alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
        PendingIntent pendingIntent = CreateAlarmPendingIntent(context, TechniqueId.Spin, string.Empty, string.Empty);
        alarmManager?.Cancel(pendingIntent);
        pendingIntent.Dispose();

        NotificationManagerCompat.From(context).Cancel(PracticeReminderConstants.NotificationId);
    }

    public void Schedule(DateTime fireLocal, TechniqueId techniqueId, string title, string body)
    {
        Context context = Platform.AppContext;
        EnsureNotificationChannel(context);

        AlarmManager? alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
        if (alarmManager is null)
        {
            return;
        }

        PendingIntent pendingIntent = CreateAlarmPendingIntent(context, techniqueId, title, body);
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
        if (notificationManager?.GetNotificationChannel(PracticeReminderConstants.ChannelId) is not null)
        {
            return;
        }

        var channel = new NotificationChannel(
            PracticeReminderConstants.ChannelId,
            "Practice reminders",
            NotificationImportance.Default)
        {
            Description = "Reminders to practice techniques"
        };
        notificationManager?.CreateNotificationChannel(channel);
    }

    internal static PendingIntent CreateAlarmPendingIntent(Context context, TechniqueId techniqueId, string title, string body)
    {
        Intent intent = new(context, typeof(PracticeReminderAlarmReceiver));
        intent.SetAction(PracticeReminderConstants.ActionReminder);
        intent.PutExtra(PracticeReminderConstants.ExtraTechniqueId, techniqueId.ToString());
        intent.PutExtra(PracticeReminderConstants.ExtraTitle, title);
        intent.PutExtra(PracticeReminderConstants.ExtraBody, body);

        return PendingIntent.GetBroadcast(
            context,
            PracticeReminderConstants.AlarmRequestCode,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }

    internal static PendingIntent CreateTapPendingIntent(Context context, TechniqueId techniqueId)
    {
        Intent intent = new(context, typeof(global::PsychologyApp.Presentation.App.MainActivity));
        intent.SetAction(Intent.ActionMain);
        intent.AddCategory(Intent.CategoryLauncher);
        intent.PutExtra(PracticeReminderConstants.ExtraTechniqueId, techniqueId.ToString());
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);

        return PendingIntent.GetActivity(
            context,
            PracticeReminderConstants.NotificationId,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }
}
#endif
