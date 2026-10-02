#if ANDROID
using Android.App;
using Android.OS;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>The exact-alarm call shared by every reminder scheduler (practice, quote, mood, chat).</summary>
internal static class AndroidAlarms
{
    /// <summary>
    /// Sets an exact, doze-proof alarm and disposes <paramref name="pendingIntent"/>. From Android 12 the exact-alarm permission can be
    /// revoked and the call then throws a SecurityException; the reminder falls back to an inexact alarm instead of being lost.
    /// </summary>
    internal static void SetExactAndDispose(AlarmManager alarmManager, long triggerAtMillis, PendingIntent pendingIntent)
    {
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
}
#endif
