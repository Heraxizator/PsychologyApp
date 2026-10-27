using PsychologyApp.Domain.UserProgress;
using Xunit;

namespace PsychologyApp.Domain.Tests.UserProgress;

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
