using System.Globalization;

namespace PsychologyApp.Application.Chat;

public static class ChatTimeFormatter
{
    private static readonly string[] RussianWeekdays = ["вс", "пн", "вт", "ср", "чт", "пт", "сб"];

    /// <summary>Messenger-style timestamp: time today, "yesterday", weekday within a week, otherwise the date.</summary>
    public static string Short(DateTime utc, DateTime nowUtc, bool english, TimeZoneInfo? zone = null)
    {
        zone ??= TimeZoneInfo.Local;
        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone);
        DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc), zone);
        int days = (now.Date - local.Date).Days;

        return days switch
        {
            <= 0 => local.ToString("HH:mm", CultureInfo.InvariantCulture),
            1 => english ? "yesterday" : "вчера",
            < 7 => english ? local.ToString("ddd", CultureInfo.InvariantCulture) : RussianWeekdays[(int)local.DayOfWeek],
            _ => english ? local.ToString("MMM d", CultureInfo.InvariantCulture) : local.ToString("dd.MM", CultureInfo.InvariantCulture)
        };
    }
}

public static class ChatSessionExtensions
{
    /// <summary>True once the person has written something. Chats holding only the greeting are not shown in lists.</summary>
    public static bool HasConversation(this ChatSessionDTO session) => CompanionState.Deserialize(session.StateJson).Turns > 0;
}
