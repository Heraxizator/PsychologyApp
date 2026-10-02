using System.Globalization;

namespace PsychologyApp.Infrastructure.Data.Sql;

/// <summary>
/// The one place that decides how instants are stored: ISO-8601 round-trip text in UTC ("…Z", fixed width), so that
/// plain string comparison and <c>substr</c> in SQL order and group by UTC time. A local-kind value written with a bare
/// <c>ToString("O")</c> would carry an offset and silently break both.
/// </summary>
internal static class SqliteTime
{
    public static string ToIso(DateTime value) =>
        value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    public static string? ToIso(DateTime? value) => value is null ? null : ToIso(value.Value);

    // Always a UTC-kind value, whether the text ends in "Z", carries an offset, or has neither (read as UTC).
    public static DateTime FromIso(string value) =>
        DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

    /// <summary>
    /// Local calendar days (newest first, distinct) of the given UTC instants stored as ISO text. Only the first 16 characters
    /// ("yyyy-MM-ddTHH:mm") are read, so the query can stay a cheap DISTINCT while the day boundary still follows the user's clock.
    /// </summary>
    public static IReadOnlyList<DateOnly> ToLocalDays(IEnumerable<string> utcMinutePrefixes, TimeZoneInfo zone) =>
        utcMinutePrefixes
            .Select(prefix => DateTime.ParseExact(prefix, "yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal))
            .Select(utc => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utc, zone)))
            .Distinct()
            .OrderByDescending(day => day)
            .ToList();
}
