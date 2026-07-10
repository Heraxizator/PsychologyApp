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

    /// <summary>
    /// Days that would be lost if the user skips today. Zero when already practiced today
    /// or when there is no consecutive run ending yesterday.
    /// </summary>
    public static int CalculateAtRiskDays(IReadOnlyList<DateOnly> datesDescending, DateOnly today)
    {
        if (datesDescending.Count == 0)
        {
            return 0;
        }

        if (datesDescending[0] == today)
        {
            return 0;
        }

        return CalculateFromCompletionDates(datesDescending, today.AddDays(-1));
    }

    public static int CalculateIdleDays(DateOnly? lastPracticeLocal, DateOnly today)
    {
        if (lastPracticeLocal is null)
        {
            return int.MaxValue;
        }

        return Math.Max(0, today.DayNumber - lastPracticeLocal.Value.DayNumber);
    }
}
