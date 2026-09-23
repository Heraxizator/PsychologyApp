namespace PsychologyApp.Domain.Notifications;

public static class ChatReminderPolicy
{
    public const int InactivityDays = 3;

    public static bool ShouldSchedule(bool remindersEnabled, bool hasCompletedOnboarding) =>
        remindersEnabled && hasCompletedOnboarding;

    public static DateTime? ResolveNextFireLocal(
        bool remindersEnabled,
        bool hasCompletedOnboarding,
        DateTime? lastChatActivityUtc,
        int reminderHour,
        DateTime nowLocal)
    {
        if (!ShouldSchedule(remindersEnabled, hasCompletedOnboarding))
        {
            return null;
        }

        int hour = QuoteReminderPolicy.ClampHour(reminderHour);
        DateTime baselineLocal = lastChatActivityUtc?.ToLocalTime() ?? nowLocal;
        DateTime dueFire = baselineLocal.Date.AddDays(InactivityDays).AddHours(hour);

        return dueFire > nowLocal ? dueFire : nowLocal.Date.AddDays(1).AddHours(hour);
    }
}
