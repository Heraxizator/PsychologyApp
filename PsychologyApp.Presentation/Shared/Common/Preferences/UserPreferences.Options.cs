using System.Globalization;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>The part of the preferences that is plain logic (defaults, keys, normalising, labels, reminder hours); it touches no MAUI API, so it is tested as it is.</summary>
public static partial class UserPreferences
{
    public const string DefaultLanguage = "ru";
    public const string DefaultTheme = "light";
    public const string DefaultColor = "blue";
    public const string DefaultForm = "rounded";
    public const string DefaultSize = "medium";
    public const string DefaultOnboardingConcern = OnboardingConcernKeys.Explore;
    public const int DefaultPracticeReminderHour = 19;
    public const int DefaultQuoteReminderHour = 9;
    public const int DefaultMoodReminderHour = 20;
    public const int DefaultChatReminderHour = 19;

    public static bool IsEnglish(string language) =>
        language.Equals("en", StringComparison.OrdinalIgnoreCase)
        || language.Equals("English", StringComparison.OrdinalIgnoreCase)
        || language.Equals("Английский", StringComparison.OrdinalIgnoreCase);

    public static bool IsDarkTheme(string theme) =>
        NormalizeThemeKey(theme) == "dark";

    public static string NormalizeLanguageKey(string value) => value switch
    {
        "en" or "EN" or "English" or "english" or "Английский" => "en",
        "ru" or "RU" or "Russian" or "russian" or "Русский" => "ru",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultLanguage : value
    };

    public static string NormalizeThemeKey(string value) => value switch
    {
        "dark" or "Dark" or "Тёмная" or "Темная" => "dark",
        "light" or "Light" or "Светлая" => "light",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultTheme : value
    };

    public static string NormalizeColorKey(string value) => value switch
    {
        "blue" or "Blue" or "Синий" => "blue",
        "red" or "Red" or "Красный" => "red",
        "yellow" or "Yellow" or "Желтый" => "yellow",
        "green" or "Green" or "Зелёный" or "Зеленый" => "green",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultColor : value
    };

    public static string NormalizeFormKey(string value) => value switch
    {
        "rounded" or "Rounded" or "Normal" or "С закруглением" => "rounded",
        "square" or "Square" or "Square corners" or "Без закругления" => "square",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultForm : value
    };

    public static string NormalizeSizeKey(string value) => value switch
    {
        "large" or "Large" or "Большой" => "large",
        "medium" or "Medium" or "Средний" => "medium",
        "small" or "Small" or "Малый" => "small",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultSize : value
    };

    public static string GetLanguageLabel(string key, string? language = null)
    {
        string lang = language ?? Current.Language;
        return NormalizeLanguageKey(key) switch
        {
            "en" => "English",
            _ => IsEnglish(lang) ? "Russian" : "Русский"
        };
    }

    public static string GetThemeLabel(string key, string? language = null)
    {
        string lang = language ?? Current.Language;
        return NormalizeThemeKey(key) switch
        {
            "dark" => IsEnglish(lang) ? "Dark" : "Тёмная",
            _ => IsEnglish(lang) ? "Light" : "Светлая"
        };
    }

    public static string GetColorLabel(string key, string? language = null)
    {
        string lang = language ?? Current.Language;
        return NormalizeColorKey(key) switch
        {
            "red" => IsEnglish(lang) ? "Red" : "Красный",
            "yellow" => IsEnglish(lang) ? "Yellow" : "Желтый",
            "green" => IsEnglish(lang) ? "Green" : "Зелёный",
            _ => IsEnglish(lang) ? "Blue" : "Синий"
        };
    }

    public static string GetFormLabel(string key, string? language = null)
    {
        string lang = language ?? Current.Language;
        return NormalizeFormKey(key) switch
        {
            "square" => IsEnglish(lang) ? "Square corners" : "Без закругления",
            _ => IsEnglish(lang) ? "Rounded" : "С закруглением"
        };
    }

    public static string GetSizeLabel(string key, string? language = null)
    {
        string lang = language ?? Current.Language;
        return NormalizeSizeKey(key) switch
        {
            "large" => IsEnglish(lang) ? "Large" : "Большой",
            "small" => IsEnglish(lang) ? "Small" : "Малый",
            _ => IsEnglish(lang) ? "Medium" : "Средний"
        };
    }

    public static IReadOnlyList<int> PracticeReminderHourKeys { get; } =
        Enumerable.Range(PracticeReminderPolicy.MinHour, PracticeReminderPolicy.MaxHour - PracticeReminderPolicy.MinHour + 1).ToArray();

    public static int NormalizePracticeReminderHour(int hour) =>
        PracticeReminderPolicy.ClampHour(hour);

    public static string GetPracticeReminderHourLabel(int hour, string? language = null)
    {
        int normalized = NormalizePracticeReminderHour(hour);
        return $"{normalized:D2}:00";
    }

    public static int ParsePracticeReminderHourKey(string displayOrKey)
    {
        if (int.TryParse(displayOrKey, out int hour))
        {
            return NormalizePracticeReminderHour(hour);
        }

        string digits = new(displayOrKey.TakeWhile(ch => char.IsDigit(ch)).ToArray());
        return int.TryParse(digits, out hour)
            ? NormalizePracticeReminderHour(hour)
            : DefaultPracticeReminderHour;
    }

    public static IReadOnlyList<string> GetPracticeReminderHourOptions(string? language = null) =>
        PracticeReminderHourKeys.Select(hour => GetPracticeReminderHourLabel(hour, language)).ToArray();

    public static IReadOnlyList<int> QuoteReminderHourKeys { get; } =
        Enumerable.Range(QuoteReminderPolicy.MinHour, QuoteReminderPolicy.MaxHour - QuoteReminderPolicy.MinHour + 1).ToArray();

    public static int NormalizeQuoteReminderHour(int hour) =>
        QuoteReminderPolicy.ClampHour(hour);

    public static int NormalizeMoodReminderHour(int hour) =>
        QuoteReminderPolicy.ClampHour(hour);

    public static int NormalizeChatReminderHour(int hour) =>
        QuoteReminderPolicy.ClampHour(hour);

    public static string GetChatReminderHourLabel(int hour, string? language = null) =>
        GetPracticeReminderHourLabel(NormalizeChatReminderHour(hour), language);

    public static int ParseChatReminderHourKey(string displayOrKey) =>
        ParseQuoteReminderHourKey(displayOrKey);

    public static IReadOnlyList<string> GetChatReminderHourOptions(string? language = null) =>
        QuoteReminderHourKeys.Select(hour => GetChatReminderHourLabel(hour, language)).ToArray();


    public static string GetQuoteReminderHourLabel(int hour, string? language = null)
    {
        int normalized = NormalizeQuoteReminderHour(hour);
        return GetPracticeReminderHourLabel(normalized, language);
    }

    public static int ParseQuoteReminderHourKey(string displayOrKey)
    {
        if (int.TryParse(displayOrKey, out int hour))
        {
            return NormalizeQuoteReminderHour(hour);
        }

        foreach (int key in QuoteReminderHourKeys)
        {
            if (string.Equals(GetQuoteReminderHourLabel(key), displayOrKey, StringComparison.Ordinal))
            {
                return key;
            }
        }

        return DefaultQuoteReminderHour;
    }

    public static IReadOnlyList<string> GetQuoteReminderHourOptions(string? language = null) =>
        QuoteReminderHourKeys.Select(hour => GetQuoteReminderHourLabel(hour, language)).ToArray();

    public static IReadOnlyList<string> LanguageKeys { get; } = ["ru", "en"];

    public static IReadOnlyList<string> ThemeKeys { get; } = ["dark", "light"];

    public static IReadOnlyList<string> ColorKeys { get; } = ["blue", "red", "yellow", "green"];

    public static IReadOnlyList<string> FormKeys { get; } = ["rounded", "square"];

    public static IReadOnlyList<string> SizeKeys { get; } = ["large", "medium", "small"];

    public static IReadOnlyList<string> GetLanguageOptions(string? language = null) =>
        LanguageKeys.Select(key => GetLanguageLabel(key, language)).ToArray();

    public static IReadOnlyList<string> GetThemeOptions(string? language = null) =>
        ThemeKeys.Select(key => GetThemeLabel(key, language)).ToArray();

    public static IReadOnlyList<string> GetColorOptions(string? language = null) =>
        ColorKeys.Select(key => GetColorLabel(key, language)).ToArray();

    public static IReadOnlyList<string> GetFormOptions(string? language = null) =>
        FormKeys.Select(key => GetFormLabel(key, language)).ToArray();

    public static IReadOnlyList<string> GetSizeOptions(string? language = null) =>
        SizeKeys.Select(key => GetSizeLabel(key, language)).ToArray();

    public static string ParseLanguageKey(string displayOrKey) => NormalizeLanguageKey(displayOrKey);

    public static IReadOnlyList<string> OnboardingConcernKeysList { get; } =
    [
        OnboardingConcernKeys.Anxiety,
        OnboardingConcernKeys.Body,
        OnboardingConcernKeys.Mood,
        OnboardingConcernKeys.Explore
    ];

    public static string NormalizeOnboardingConcernKey(string value) => value switch
    {
        OnboardingConcernKeys.Anxiety or "Anxiety" or "Тревога" =>
            OnboardingConcernKeys.Anxiety,
        OnboardingConcernKeys.Body or "Body" or "Body / symptoms" or "Тело / симптомы" =>
            OnboardingConcernKeys.Body,
        OnboardingConcernKeys.Mood or "Mood" or "Настроение" =>
            OnboardingConcernKeys.Mood,
        OnboardingConcernKeys.Explore or "Explore" or "Just exploring" or "Просто попробовать" =>
            OnboardingConcernKeys.Explore,
        _ => OnboardingConcernKeys.Default
    };

    public static string GetOnboardingConcernLabel(string key, string? language = null)
    {
        string ui = language ?? GetPersistedLanguage();
        bool en = IsEnglish(ui);
        return NormalizeOnboardingConcernKey(key) switch
        {
            OnboardingConcernKeys.Anxiety => en ? "Anxiety" : "Тревога",
            OnboardingConcernKeys.Body => en ? "Body / symptoms" : "Тело / симптомы",
            OnboardingConcernKeys.Mood => en ? "Mood" : "Настроение",
            _ => en ? "Just exploring" : "Просто попробовать"
        };
    }

    public static string ParseOnboardingConcernKey(string displayOrKey) =>
        NormalizeOnboardingConcernKey(displayOrKey);

    public static string ParseThemeKey(string displayOrKey) => NormalizeThemeKey(displayOrKey);

    public static string ParseColorKey(string displayOrKey) => NormalizeColorKey(displayOrKey);

    public static string ParseFormKey(string displayOrKey) => NormalizeFormKey(displayOrKey);

    public static string ParseSizeKey(string displayOrKey) => NormalizeSizeKey(displayOrKey);

}
