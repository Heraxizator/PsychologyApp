namespace PsychologyApp.Domain.Notifications;

public static class PracticeReminderPolicy
{
    public const int MinHour = 8;
    public const int MaxHour = 22;
    public const int DefaultHour = 19;

    public static int ClampHour(int hour) => Math.Clamp(hour, MinHour, MaxHour);

    public static bool ShouldSchedule(bool remindersEnabled, bool hasCompletedOnboarding) =>
        remindersEnabled && hasCompletedOnboarding;

    public static bool PracticedToday(DateTime? lastPracticeUtc, DateTime nowLocal)
    {
        if (lastPracticeUtc is null)
        {
            return false;
        }

        DateOnly last = DateOnly.FromDateTime(lastPracticeUtc.Value.ToLocalTime());
        DateOnly today = DateOnly.FromDateTime(nowLocal);
        return last == today;
    }

    public static DateTime? ResolveNextFireLocal(
        bool remindersEnabled,
        bool hasCompletedOnboarding,
        DateTime? lastPracticeUtc,
        int reminderHour,
        DateTime nowLocal)
    {
        if (!ShouldSchedule(remindersEnabled, hasCompletedOnboarding))
        {
            return null;
        }

        int hour = ClampHour(reminderHour);
        DateTime todayFire = nowLocal.Date.AddHours(hour);

        if (PracticedToday(lastPracticeUtc, nowLocal))
        {
            return todayFire.AddDays(1);
        }

        if (nowLocal < todayFire)
        {
            return todayFire;
        }

        return todayFire.AddDays(1);
    }
}
