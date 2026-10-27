using PsychologyApp.Domain.UserProgress;
using Xunit;

namespace PsychologyApp.Domain.Tests.UserProgress;

public sealed class ActiveDaysCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 10, 27);

    // A gap does not wipe the week out, as it would a streak: the days before it still count.
    [Fact]
    public void Days_around_a_gap_all_count()
    {
        DateOnly[] days = [Today, Today.AddDays(-2), Today.AddDays(-5)];

        Assert.Equal(3, ActiveDaysCalculator.CountInLastWeek(days, Today));
    }

    [Fact]
    public void Several_check_ins_on_one_day_count_once()
    {
        DateOnly[] days = [Today, Today, Today.AddDays(-1)];

        Assert.Equal(2, ActiveDaysCalculator.CountInLastWeek(days, Today));
    }

    [Fact]
    public void Only_the_last_seven_days_count()
    {
        DateOnly[] days = [Today.AddDays(-6), Today.AddDays(-7), Today.AddDays(1)];

        Assert.Equal(1, ActiveDaysCalculator.CountInLastWeek(days, Today));
    }
}
