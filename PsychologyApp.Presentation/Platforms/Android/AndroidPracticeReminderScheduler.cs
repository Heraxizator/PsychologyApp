#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

public sealed class AndroidPracticeReminderScheduler : IPracticeReminderScheduler
{
    public bool IsSupported => true;

    public Task<bool> RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        AndroidNotificationPermission.RequestIfNeededAsync(cancellationToken);

    public void Cancel()
    {
        Context context = Platform.AppContext;
        AlarmManager? alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
        PendingIntent pendingIntent = CreateAlarmPendingIntent(context, TechniqueId.Spin, string.Empty, string.Empty);
        alarmManager?.Cancel(pendingIntent);
        pendingIntent.Dispose();

        NotificationManagerCompat.From(context)?.Cancel(PracticeReminderConstants.NotificationId);
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
            PracticeReminderConstants.ChannelId,
            AppStrings.PracticeReminderTitle,
            NotificationImportance.Default);
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
        intent.SetAction(PracticeReminderConstants.ActionOpenFromNotification);
        intent.SetPackage(context.PackageName);
        intent.PutExtra(PracticeReminderConstants.ExtraTechniqueId, techniqueId.ToString());
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);

        ReminderIntentToken.Put(intent);
        return PendingIntent.GetActivity(
            context,
            PracticeReminderConstants.NotificationId,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }
}
#endif
