namespace PsychologyApp.Domain.Notifications;

/// <summary>
/// When the daily mood reminder fires. "Automatic" means the hour the person usually checks in, read from their own
/// recent check-ins; a reminder never fires on a day they already checked in shortly before it.
/// </summary>
public static class MoodReminderPolicy
{
    /// <summary>Fewer check-ins than this do not show a habit: the chosen fallback hour is used.</summary>
    public const int MinCheckIns = 5;

    public static readonly TimeSpan HistoryWindow = TimeSpan.FromDays(30);

    /// <summary>
    /// A check-in this long before the reminder makes it pointless. Shorter than the gap between the journal's morning
    /// and evening check-ins, so a morning entry does not silence the evening reminder.
    /// </summary>
    public static readonly TimeSpan CoveredBefore = TimeSpan.FromHours(3);

    /// <summary>
    /// The hour with the most check-ins (a later hour wins a tie: evening is the more common routine), clamped to the
    /// reminder range; null when there are too few check-ins. The most common hour, not the average: morning and
    /// evening check-ins would average to midday, when the person does neither.
    /// </summary>
    public static int? UsualHour(IReadOnlyCollection<DateTime> checkInsLocal)
    {
        if (checkInsLocal.Count < MinCheckIns)
        {
            return null;
        }

        int hour = checkInsLocal
            .GroupBy(checkIn => checkIn.Hour)
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => group.Key)
            .First()
            .Key;
        return QuoteReminderPolicy.ClampHour(hour);
    }

    public static DateTime NextFireLocal(int hour, IEnumerable<DateTime> checkInsLocal, DateTime nowLocal)
    {
        DateTime today = nowLocal.Date.AddHours(QuoteReminderPolicy.ClampHour(hour));
        bool covered = checkInsLocal.Any(checkIn => checkIn >= today - CoveredBefore && checkIn <= nowLocal);
        return nowLocal < today && !covered ? today : today.AddDays(1);
    }
}
