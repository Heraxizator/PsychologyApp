using System.Globalization;
using Microsoft.Maui.Controls.Shapes;

namespace PsychologyApp.Presentation.Infrastructure;

public static class UserPreferences
{
    public const string LanguageKey = "Language";
    public const string ThemeKey = "Theme";
    public const string ColorKey = "Color";
    public const string FormKey = "Form";
    public const string SizeKey = "Size";
    public const string IsBoldKey = "IsBold";

    public const string DefaultLanguage = "ru";
    public const string DefaultTheme = "light";
    public const string DefaultColor = "blue";
    public const string DefaultForm = "rounded";
    public const string DefaultSize = "medium";

    public static event Action? Changed;

    public static UserPreferencesState Load()
    {
        return new UserPreferencesState
        {
            Language = NormalizeLanguageKey(Preferences.Get(LanguageKey, DefaultLanguage)),
            Theme = NormalizeThemeKey(Preferences.Get(ThemeKey, DefaultTheme)),
            Color = NormalizeColorKey(Preferences.Get(ColorKey, DefaultColor)),
            Form = NormalizeFormKey(Preferences.Get(FormKey, DefaultForm)),
            Size = NormalizeSizeKey(Preferences.Get(SizeKey, DefaultSize)),
            IsBold = Preferences.Get(IsBoldKey, false)
        };
    }

    public static void Save(UserPreferencesState state)
    {
        Preferences.Set(LanguageKey, NormalizeLanguageKey(state.Language));
        Preferences.Set(ThemeKey, NormalizeThemeKey(state.Theme));
        Preferences.Set(ColorKey, NormalizeColorKey(state.Color));
        Preferences.Set(FormKey, NormalizeFormKey(state.Form));
        Preferences.Set(SizeKey, NormalizeSizeKey(state.Size));
        Preferences.Set(IsBoldKey, state.IsBold);
    }

    public static void ApplyAll()
    {
        UserPreferencesState state = Load();
        ApplyLanguage(state.Language);
        ApplyTheme(state.Theme);
        ApplyAccentColor(state.Color);
        ApplyTypography(state.Size, state.IsBold);
        ApplyForm(state.Form);
        Changed?.Invoke();
    }

    public static void ApplyTheme()
    {
        ApplyTheme(Load().Theme);
    }

    public static void ApplyTheme(string theme)
    {
        if (Microsoft.Maui.Controls.Application.Current is null)
        {
            return;
        }

        Microsoft.Maui.Controls.Application.Current.UserAppTheme =
            IsDarkTheme(theme) ? AppTheme.Dark : AppTheme.Light;
    }

    public static void ApplyLanguage()
    {
        ApplyLanguage(Load().Language);
    }

    public static void ApplyLanguage(string language)
    {
        string cultureName = IsEnglish(language) ? "en" : "ru";
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    public static void ApplyAccentColor(string color)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        (Color primary, Color secondary) = ResolveAccentColors(NormalizeColorKey(color));
        resources["Primary"] = primary;
        resources["Secondary"] = secondary;
    }

    public static void ApplyTypography(string size, bool isBold)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        (double section, double body) = ResolveFontSizes(NormalizeSizeKey(size));
        resources["SectionTitleFontSize"] = section;
        resources["BodyFontSize"] = body;
        resources["NavTitleFontSize"] = section + 1;
        resources["BodyFontFamily"] = isBold ? "RobotoSemiBold" : "RobotoRegular";
    }

    public static void ApplyForm(string form)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        double radius = IsRoundedForm(form) ? 30 : 4;
        SetCornerRadiusResources(resources, radius);
    }

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
        "square" or "Square" or "Без закругления" => "square",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultForm : value
    };

    public static string NormalizeSizeKey(string value) => value switch
    {
        "large" or "Large" or "Большой" => "large",
        "medium" or "Medium" or "Средний" => "medium",
        "small" or "Small" or "Малый" => "small",
        _ => string.IsNullOrWhiteSpace(value) ? DefaultSize : value
    };

    public static string GetLanguageLabel(string key, string? language = null) =>
        NormalizeLanguageKey(key) switch
        {
            "en" => "English",
            _ => "Русский"
        };

    public static string GetThemeLabel(string key, string? language = null)
    {
        string lang = language ?? Load().Language;
        return NormalizeThemeKey(key) switch
        {
            "dark" => IsEnglish(lang) ? "Dark" : "Тёмная",
            _ => IsEnglish(lang) ? "Light" : "Светлая"
        };
    }

    public static string GetColorLabel(string key, string? language = null)
    {
        string lang = language ?? Load().Language;
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
        string lang = language ?? Load().Language;
        return NormalizeFormKey(key) switch
        {
            "square" => IsEnglish(lang) ? "Square corners" : "Без закругления",
            _ => IsEnglish(lang) ? "Rounded" : "С закруглением"
        };
    }

    public static string GetSizeLabel(string key, string? language = null)
    {
        string lang = language ?? Load().Language;
        return NormalizeSizeKey(key) switch
        {
            "large" => IsEnglish(lang) ? "Large" : "Большой",
            "small" => IsEnglish(lang) ? "Small" : "Малый",
            _ => IsEnglish(lang) ? "Medium" : "Средний"
        };
    }

    public static IReadOnlyList<string> GetLanguageOptions(string? language = null) =>
        [GetLanguageLabel("ru", language), GetLanguageLabel("en", language)];

    public static IReadOnlyList<string> GetThemeOptions(string? language = null) =>
        [GetThemeLabel("dark", language), GetThemeLabel("light", language)];

    public static IReadOnlyList<string> GetColorOptions(string? language = null) =>
        new[] { "blue", "red", "yellow", "green" }.Select(key => GetColorLabel(key, language)).ToArray();

    public static IReadOnlyList<string> GetFormOptions(string? language = null) =>
        [GetFormLabel("rounded", language), GetFormLabel("square", language)];

    public static IReadOnlyList<string> GetSizeOptions(string? language = null) =>
        [GetSizeLabel("large", language), GetSizeLabel("medium", language), GetSizeLabel("small", language)];

    public static string ParseLanguageKey(string displayOrKey) => NormalizeLanguageKey(displayOrKey);

    public static string ParseThemeKey(string displayOrKey) => NormalizeThemeKey(displayOrKey);

    public static string ParseColorKey(string displayOrKey) => NormalizeColorKey(displayOrKey);

    public static string ParseFormKey(string displayOrKey) => NormalizeFormKey(displayOrKey);

    public static string ParseSizeKey(string displayOrKey) => NormalizeSizeKey(displayOrKey);

    private static bool IsRoundedForm(string form) =>
        NormalizeFormKey(form) == "rounded";

    private static (Color primary, Color secondary) ResolveAccentColors(string color) =>
        NormalizeColorKey(color) switch
        {
            "red" => (Color.FromArgb("#E53935"), Color.FromArgb("#FFAB91")),
            "yellow" => (Color.FromArgb("#F7B548"), Color.FromArgb("#FFE5B9")),
            "green" => (Color.FromArgb("#2E9E5B"), Color.FromArgb("#A8E6C1")),
            _ => (Color.FromArgb("#0085FF"), Color.FromArgb("#96d1ff"))
        };

    private static (double section, double body) ResolveFontSizes(string size) =>
        NormalizeSizeKey(size) switch
        {
            "large" => (18, 16),
            "small" => (14, 12),
            _ => (16, 14)
        };

    private static void SetCornerRadiusResources(ResourceDictionary resources, double radius)
    {
        resources["UiCornerRadius"] = radius;
        resources["UiCornerRadiusShape"] = new RoundRectangle { CornerRadius = radius };
        resources["UiCornerRadiusCompactShape"] = new RoundRectangle { CornerRadius = radius <= 4 ? 9 : radius + 5 };
        resources["UiCornerRadiusEntryShape"] = new RoundRectangle { CornerRadius = radius <= 4 ? 4 : Math.Min(radius, 25) };
    }
}

public sealed class UserPreferencesState
{
    public string Language { get; init; } = UserPreferences.DefaultLanguage;
    public string Theme { get; init; } = UserPreferences.DefaultTheme;
    public string Color { get; init; } = UserPreferences.DefaultColor;
    public string Form { get; init; } = UserPreferences.DefaultForm;
    public string Size { get; init; } = UserPreferences.DefaultSize;
    public bool IsBold { get; init; }
}
