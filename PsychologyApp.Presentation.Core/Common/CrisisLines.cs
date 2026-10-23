namespace PsychologyApp.Presentation.Common;

/// <summary>
/// The numbers a person in danger can call from where the phone is. The emergency number always exists; a crisis line
/// is listed only for countries where a national, free, round-the-clock one was checked (October 2026), otherwise the
/// screen points to findahelpline.com. The country comes from the phone's network, not from the interface language:
/// a French speaker may be in Canada.
/// </summary>
public static class CrisisLines
{
    /// <param name="Emergency">What to dial for an ambulance or the police.</param>
    /// <param name="LineName">The crisis line under its own name, or null when this country has none listed.</param>
    /// <param name="LineNumber">Digits to dial.</param>
    /// <param name="LineDisplay">The number as people in that country write it.</param>
    public sealed record Lines(string Emergency, string? LineName, string? LineNumber, string? LineDisplay)
    {
        public bool HasLine => LineNumber is not null;
    }

    private const string DefaultEmergency = "112";

    private static readonly Dictionary<string, Lines> ByCountry = new(StringComparer.OrdinalIgnoreCase)
    {
        // The Russian line's name is translated; the others keep the name callers will hear when it answers.
        ["RU"] = new("112", null, "88002000122", "8-800-2000-122"),
        ["US"] = new("911", "Suicide & Crisis Lifeline", "988", "988"),
        ["CA"] = new("911", "Suicide Crisis Helpline", "988", "988"),
        ["GB"] = new("999", "Samaritans", "116123", "116 123"),
        ["IE"] = new("112", "Samaritans", "116123", "116 123"),
        ["FR"] = new("112", "Prévention du suicide", "3114", "3114"),
        ["BE"] = new("112", "Centre de Prévention du Suicide", "080032123", "0800 32 123"),
        ["CH"] = new("112", "La Main Tendue / Die Dargebotene Hand", "143", "143"),
        ["DE"] = new("112", "TelefonSeelsorge", "08001110111", "0800 111 0 111"),
        ["AT"] = new("112", "TelefonSeelsorge", "142", "142"),
        ["ES"] = new("112", "Línea de atención a la conducta suicida", "024", "024"),
        ["MX"] = new("911", "Línea de la Vida", "8009112000", "800 911 2000"),
        ["PT"] = new("112", "SNS 24", "808242424", "808 24 24 24"),
        ["BR"] = new("192", "CVV", "188", "188"),
        ["CN"] = new("120", "心理援助热线", "12356", "12356"),
    };

    /// <summary>The numbers for a two-letter country code; an unknown or missing country gets 112 and no line.</summary>
    public static Lines For(string? country) =>
        country is not null && ByCountry.TryGetValue(country.Trim(), out Lines? lines)
            ? lines with { LineName = lines.LineName ?? AppStrings.CrisisHubHotlineRuName }
            : new Lines(DefaultEmergency, null, null, null);
}
