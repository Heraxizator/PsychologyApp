namespace PsychologyApp.Domain.Notifications;

public static class QuoteReminderPolicy
{
    public const int MinHour = 8;
    public const int MaxHour = 22;
    public const int DefaultHour = 9;

    public static int ClampHour(int hour) => Math.Clamp(hour, MinHour, MaxHour);

    public static bool ShouldSchedule(bool remindersEnabled, bool hasCompletedOnboarding) =>
        remindersEnabled && hasCompletedOnboarding;

    public static DateTime? ResolveNextFireLocal(
        bool remindersEnabled,
        bool hasCompletedOnboarding,
        int reminderHour,
        DateTime nowLocal)
    {
        if (!ShouldSchedule(remindersEnabled, hasCompletedOnboarding))
        {
            return null;
        }

        int hour = ClampHour(reminderHour);
        DateTime todayFire = nowLocal.Date.AddHours(hour);
        return nowLocal < todayFire ? todayFire : todayFire.AddDays(1);
    }
}
