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

    private static readonly string[] RussianMonthsGenitive =
        ["января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря"];

    private static readonly string[] EnglishMonths =
        ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    /// <summary>Clock time of a message inside its bubble ("14:05"), in the person's time zone.</summary>
    public static string Clock(DateTime utc, TimeZoneInfo? zone = null) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone ?? TimeZoneInfo.Local)
            .ToString("HH:mm", CultureInfo.InvariantCulture);

    /// <summary>Label of the divider between days: "Today", "Yesterday" or "21 September". Month names are spelled out so it works without culture data.</summary>
    public static string DayLabel(DateTime utc, DateTime nowUtc, bool english, TimeZoneInfo? zone = null)
    {
        zone ??= TimeZoneInfo.Local;
        DateTime local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone);
        DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc), zone);
        int days = (now.Date - local.Date).Days;

        return days switch
        {
            <= 0 => english ? "Today" : "Сегодня",
            1 => english ? "Yesterday" : "Вчера",
            _ => english
                ? $"{EnglishMonths[local.Month - 1]} {local.Day}"
                : $"{local.Day} {RussianMonthsGenitive[local.Month - 1]}"
        };
    }

    /// <summary>True when both instants fall on the same calendar day in the person's time zone.</summary>
    public static bool SameDay(DateTime firstUtc, DateTime secondUtc, TimeZoneInfo? zone = null)
    {
        zone ??= TimeZoneInfo.Local;
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(firstUtc, DateTimeKind.Utc), zone).Date
            == TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(secondUtc, DateTimeKind.Utc), zone).Date;
    }
}

public static class ChatSessionExtensions
{
    /// <summary>True once the person has written something. Chats holding only the greeting are not shown in lists.</summary>
    public static bool HasConversation(this ChatSessionDTO session) => CompanionState.Deserialize(session.StateJson).Turns > 0;
}
