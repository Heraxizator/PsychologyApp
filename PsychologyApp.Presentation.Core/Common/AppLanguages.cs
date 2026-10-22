namespace PsychologyApp.Presentation.Common;

/// <summary>
/// The languages the app can be shown in and what each of them has of its own. The interface is translated into every one;
/// the content (practices, articles, tests, quotes, the chat) exists in Russian and English only, so any other language
/// shows the English content until its own is written and reviewed.
/// </summary>
public static class AppLanguages
{
    public const string Russian = "ru";
    public const string English = "en";
    public const string French = "fr";
    public const string German = "de";
    public const string Spanish = "es";
    public const string Chinese = "zh";
    public const string Portuguese = "pt";

    /// <summary>In the order the language picker lists them.</summary>
    public static IReadOnlyList<string> All { get; } = [Russian, English, French, German, Spanish, Portuguese, Chinese];

    /// <summary>A stored or displayed language (a key, a name in any of the languages, an older label) as its key; anything unknown is Russian, the default.</summary>
    public static string Normalize(string? value)
    {
        string text = (value ?? string.Empty).Trim();
        if (text.Length == 0)
        {
            return Russian;
        }

        foreach (string key in All)
        {
            if (text.Equals(key, StringComparison.OrdinalIgnoreCase) || text.Equals(NativeName(key), StringComparison.OrdinalIgnoreCase))
            {
                return key;
            }
        }

        return text.ToLowerInvariant() switch
        {
            "english" or "английский" => English,
            "russian" or "русский" => Russian,
            "french" => French,
            "german" => German,
            "spanish" => Spanish,
            "chinese" or "zh-hans" or "zh-cn" => Chinese,
            "portuguese" or "pt-br" or "pt-pt" => Portuguese,
            _ => Russian
        };
    }

    /// <summary>The language's own name, as language pickers show it so that a person can find theirs whatever the interface is in.</summary>
    public static string NativeName(string key) => key switch
    {
        English => "English",
        French => "Français",
        German => "Deutsch",
        Spanish => "Español",
        Chinese => "中文",
        Portuguese => "Português",
        _ => "Русский"
    };

    /// <summary>The culture for dates and numbers.</summary>
    public static string CultureName(string key) => Normalize(key) switch
    {
        Chinese => "zh-Hans",
        Portuguese => "pt-BR",
        string language => language
    };

    /// <summary>The locale speech is recognised in: the person dictates in the language they chose, whatever the content is in.</summary>
    public static string SpeechLocale(string key) => Normalize(key) switch
    {
        English => "en-US",
        French => "fr-FR",
        German => "de-DE",
        Spanish => "es-ES",
        Chinese => "zh-CN",
        Portuguese => "pt-BR",
        _ => "ru-RU"
    };

    /// <summary>The language of the content shown with this interface language: Russian for Russian, English for every other.</summary>
    public static string ContentLanguage(string key) => Normalize(key) == Russian ? Russian : English;

    /// <summary>
    /// Whether the chat can be used in this language. It reads crisis words from a word list of its language; in a language
    /// without one, "I do not want to live" would go unnoticed, so the chat stays closed there instead of answering anyway.
    /// </summary>
    public static bool HasChat(string key) => Normalize(key) is Russian or English;

    /// <summary>A system language the app has, or null: the first start follows the phone when it can.</summary>
    public static string? FromSystem(string twoLetterIsoName)
    {
        string key = twoLetterIsoName.ToLowerInvariant();
        return All.Contains(key) ? key : null;
    }
}
