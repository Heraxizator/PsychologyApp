namespace PsychologyApp.Domain.UserProgress;

/// <summary>
/// Days with a practice or a mood check-in among the last seven, today included. Unlike a streak, a missed day costs nothing: it
/// simply does not count, and the window moves on by itself.
/// </summary>
public static class ActiveDaysCalculator
{
    public const int WeekLength = 7;

    public static int CountInLastWeek(IEnumerable<DateOnly> days, DateOnly today)
    {
        DateOnly first = today.AddDays(-(WeekLength - 1));
        return days.Where(day => day >= first && day <= today).Distinct().Count();
    }
}
