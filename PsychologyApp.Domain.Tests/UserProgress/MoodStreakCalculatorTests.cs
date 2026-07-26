using PsychologyApp.Domain.UserProgress;
using Xunit;

namespace PsychologyApp.Domain.Tests.UserProgress;

public sealed class MoodStreakCalculatorTests
{
    [Fact]
    public void Calculate_Empty_ReturnsZero()
    {
        Assert.Equal(0, MoodStreakCalculator.Calculate([], DateOnly.Parse("2026-07-26")));
    }

    [Fact]
    public void Calculate_ContinuesFromYesterday_WhenTodayMissing()
    {
        DateOnly today = DateOnly.Parse("2026-07-26");
        HashSet<DateOnly> days = [today.AddDays(-1), today.AddDays(-2)];
        Assert.Equal(2, MoodStreakCalculator.Calculate(days, today));
    }

    [Fact]
    public void Calculate_IncludesToday_WhenPresent()
    {
        DateOnly today = DateOnly.Parse("2026-07-26");
        HashSet<DateOnly> days = [today, today.AddDays(-1)];
        Assert.Equal(2, MoodStreakCalculator.Calculate(days, today));
    }
}

public sealed class MoodCheckInSlotPolicyTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(14, true)]
    [InlineData(15, false)]
    [InlineData(23, false)]
    public void IsMorningLocalHour_UsesCutoff15(int hour, bool expectedMorning)
    {
        Assert.Equal(expectedMorning, MoodCheckInSlotPolicy.IsMorningLocalHour(hour));
        Assert.Equal(!expectedMorning, MoodCheckInSlotPolicy.IsEveningLocalHour(hour));
    }
}
