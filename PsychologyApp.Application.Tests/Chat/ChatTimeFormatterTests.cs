using PsychologyApp.Application.Chat;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class ChatTimeFormatterTests
{
    private static readonly DateTime Now = new(2026, 9, 21, 15, 30, 0, DateTimeKind.Utc); // Monday
    private static readonly TimeZoneInfo Utc = TimeZoneInfo.Utc;

    [Theory]
    [InlineData("2026-09-21T09:05:00Z", false, "09:05")]
    [InlineData("2026-09-20T23:00:00Z", false, "вчера")]
    [InlineData("2026-09-20T23:00:00Z", true, "yesterday")]
    [InlineData("2026-09-18T10:00:00Z", false, "пт")]
    [InlineData("2026-09-18T10:00:00Z", true, "Fri")]
    [InlineData("2026-09-01T10:00:00Z", false, "01.09")]
    [InlineData("2026-09-01T10:00:00Z", true, "Sep 1")]
    public void Formats_like_a_messenger(string utc, bool english, string expected) =>
        Assert.Equal(expected, ChatTimeFormatter.Short(DateTime.Parse(utc, null, System.Globalization.DateTimeStyles.AdjustToUniversal), Now, english, Utc));

    [Fact]
    public void Uses_the_local_calendar_day_not_the_utc_one()
    {
        TimeZoneInfo plus5 = TimeZoneInfo.CreateCustomTimeZone("plus5", TimeSpan.FromHours(5), "plus5", "plus5");
        DateTime late = new(2026, 9, 20, 20, 0, 0, DateTimeKind.Utc); // 01:00 on the 21st locally

        Assert.Equal("01:00", ChatTimeFormatter.Short(late, Now, english: false, plus5));
    }

    [Fact]
    public void A_chat_counts_as_a_conversation_only_after_the_person_wrote()
    {
        Assert.False(new ChatSessionDTO().HasConversation());
        Assert.False(new ChatSessionDTO { StateJson = new CompanionState().Serialize() }.HasConversation());
        Assert.True(new ChatSessionDTO { StateJson = (new CompanionState() with { Turns = 1 }).Serialize() }.HasConversation());
    }
}
