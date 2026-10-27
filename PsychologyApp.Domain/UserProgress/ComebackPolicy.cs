namespace PsychologyApp.Domain.UserProgress;

/// <summary>
/// When the practice tab says "Welcome back". Any activity counts, a practice or a mood check-in: someone who writes in the journal
/// every day has not been away. Someone who has done nothing yet is new, not returning.
/// </summary>
public static class ComebackPolicy
{
    public const int AwayDays = 3;

    public static bool IsReturning(DateOnly? lastActiveDay, DateOnly today) =>
        lastActiveDay is { } last && today.DayNumber - last.DayNumber >= AwayDays;
}
