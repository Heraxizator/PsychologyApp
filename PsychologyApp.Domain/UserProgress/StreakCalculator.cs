namespace PsychologyApp.Domain.UserProgress;

public static class StreakCalculator
{
    public static int CalculateFromCompletionDates(IReadOnlyList<DateOnly> dates, DateOnly today)
    {
        if (dates.Count == 0)
        {
            return 0;
        }

        int streak = 0;
        DateOnly expected = today;

        foreach (DateOnly date in dates)
        {
            if (date == expected)
            {
                streak++;
                expected = expected.AddDays(-1);
                continue;
            }

            if (date < expected)
            {
                break;
            }
        }

        return streak;
    }
}
