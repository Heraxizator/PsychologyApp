using PsychologyApp.Domain.UserProgress;
using Xunit;

namespace PsychologyApp.Domain.Tests.UserProgress;

public sealed class StreakCalculatorTests
{
    [Fact]
    public void CalculateFromCompletionDates_WhenNoDates_ReturnsZero() =>
        Assert.Equal(0, StreakCalculator.CalculateFromCompletionDates([], new DateOnly(2026, 6, 30)));

    [Fact]
    public void CalculateFromCompletionDates_WhenTodayCompleted_ReturnsOne()
    {
        DateOnly today = new(2026, 6, 30);
        int streak = StreakCalculator.CalculateFromCompletionDates([today], today);
        Assert.Equal(1, streak);
    }

    [Fact]
    public void CalculateFromCompletionDates_CountsConsecutiveDaysFromToday()
    {
        DateOnly today = new(2026, 6, 30);
        IReadOnlyList<DateOnly> dates = [today, today.AddDays(-1), today.AddDays(-2)];

        Assert.Equal(3, StreakCalculator.CalculateFromCompletionDates(dates, today));
    }

    [Fact]
    public void CalculateFromCompletionDates_StopsAtFirstGap()
    {
        DateOnly today = new(2026, 6, 30);
        IReadOnlyList<DateOnly> dates = [today, today.AddDays(-2), today.AddDays(-3)];

        Assert.Equal(1, StreakCalculator.CalculateFromCompletionDates(dates, today));
    }
}
