using PsychologyApp.Domain.Notifications;
using Xunit;

namespace PsychologyApp.Domain.Tests.Notifications;

public sealed class MoodReminderPolicyTests
{
    private static readonly DateTime Day = new(2026, 10, 18);

    [Fact]
    public void UsualHour_IsTheMostCommonHourNotTheAverage()
    {
        DateTime[] checkIns =
        [
            Day.AddHours(9.2), Day.AddDays(-1).AddHours(9.5),
            Day.AddDays(-1).AddHours(21.1), Day.AddDays(-2).AddHours(21.4), Day.AddDays(-3).AddHours(21.9)
        ];

        Assert.Equal(21, MoodReminderPolicy.UsualHour(checkIns));
    }

    [Fact]
    public void UsualHour_NeedsEnoughCheckIns()
    {
        Assert.Null(MoodReminderPolicy.UsualHour([Day.AddHours(21), Day.AddDays(-1).AddHours(21)]));
    }

    [Fact]
    public void NextFire_SkipsTodayWhenAlreadyCheckedInShortlyBefore()
    {
        DateTime fire = MoodReminderPolicy.NextFireLocal(21, [Day.AddHours(19.5)], Day.AddHours(19.6));

        Assert.Equal(Day.AddDays(1).AddHours(21), fire);
    }

    [Fact]
    public void NextFire_MorningCheckInDoesNotSilenceTheEveningReminder()
    {
        DateTime fire = MoodReminderPolicy.NextFireLocal(21, [Day.AddHours(9)], Day.AddHours(12));

        Assert.Equal(Day.AddHours(21), fire);
    }
}
