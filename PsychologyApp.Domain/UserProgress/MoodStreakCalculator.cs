namespace PsychologyApp.Domain.UserProgress;

/// <summary>
/// Consecutive calendar days with at least one mood check-in.
/// If today has no check-in yet, the streak may still count from yesterday.
/// </summary>
public static class MoodStreakCalculator
{
    public static int Calculate(IReadOnlyCollection<DateOnly> daysWithMood, DateOnly today)
    {
        if (daysWithMood.Count == 0)
        {
            return 0;
        }

        int streak = 0;
        DateOnly cursor = today;
        if (!daysWithMood.Contains(today))
        {
            cursor = today.AddDays(-1);
        }

        while (daysWithMood.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }
}
