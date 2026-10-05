#if ANDROID
using Android.App;
using Android.OS;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>The alarm call shared by every reminder scheduler (practice, quote, mood, chat).</summary>
internal static class AndroidAlarms
{
    /// <summary>
    /// Sets a doze-tolerant but <b>inexact</b> alarm (the system may deliver it some minutes late) and disposes <paramref name="pendingIntent"/>.
    /// A daily "time to practice" reminder does not need to-the-minute delivery, and exact alarms need the SCHEDULE_EXACT_ALARM permission,
    /// which Google Play reserves for alarm and calendar apps and which the user can revoke at any time.
    /// </summary>
    internal static void SetReminderAndDispose(AlarmManager alarmManager, long triggerAtMillis, PendingIntent pendingIntent)
    {
        try
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
            else
            {
                alarmManager.Set(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
            }
        }
        finally
        {
            pendingIntent.Dispose();
        }
    }
}
#endif
