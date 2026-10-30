#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

public sealed class AndroidMoodReminderScheduler : IMoodReminderScheduler
{
    public bool IsSupported => true;

    public Task<bool> RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        AndroidNotificationPermission.RequestIfNeededAsync(cancellationToken);

    public void Cancel()
    {
        Context context = Platform.AppContext;
        AlarmManager? alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
        PendingIntent pendingIntent = CreateAlarmPendingIntent(context, string.Empty, string.Empty);
        alarmManager?.Cancel(pendingIntent);
        pendingIntent.Dispose();

        NotificationManagerCompat.From(context)?.Cancel(MoodReminderConstants.NotificationId);
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

        AndroidAlarms.SetReminderAndDispose(alarmManager, triggerAtMillis, pendingIntent);
    }

    internal static void EnsureNotificationChannel(Context context)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
        {
            return;
        }

        NotificationManager? notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
        var channel = new NotificationChannel(
            MoodReminderConstants.ChannelId,
            AppStrings.MoodReminderTitle,
            NotificationImportance.Default);
        notificationManager?.CreateNotificationChannel(channel);
    }

    internal static PendingIntent CreateAlarmPendingIntent(Context context, string title, string body)
    {
        Intent intent = new(context, typeof(MoodReminderAlarmReceiver));
        intent.SetAction(MoodReminderConstants.ActionReminder);
        intent.PutExtra(MoodReminderConstants.ExtraTitle, title);
        intent.PutExtra(MoodReminderConstants.ExtraBody, body);

        return PendingIntent.GetBroadcast(
            context,
            MoodReminderConstants.AlarmRequestCode,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }

    internal static PendingIntent CreateTapPendingIntent(Context context)
    {
        Intent intent = new(context, typeof(global::PsychologyApp.Presentation.App.MainActivity));
        intent.SetAction(MoodReminderConstants.ActionOpenFromNotification);
        intent.SetPackage(context.PackageName);
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);

        ReminderIntentToken.Put(intent);
        return PendingIntent.GetActivity(
            context,
            MoodReminderConstants.NotificationId,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }
}
#endif
