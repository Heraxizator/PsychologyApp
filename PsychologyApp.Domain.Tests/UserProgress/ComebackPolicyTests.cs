using PsychologyApp.Domain.UserProgress;
using Xunit;

namespace PsychologyApp.Domain.Tests.UserProgress;

public sealed class ComebackPolicyTests
{
    private static readonly DateOnly Today = new(2026, 10, 27);

    // "Welcome back" to someone who opened the app for the first time read as a mistake.
    [Fact]
    public void Someone_who_has_done_nothing_yet_is_not_welcomed_back() =>
        Assert.False(ComebackPolicy.IsReturning(null, Today));

    [Theory]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(30, true)]
    public void Only_three_days_away_or_more_is_a_return(int daysAway, bool returning) =>
        Assert.Equal(returning, ComebackPolicy.IsReturning(Today.AddDays(-daysAway), Today));
}
