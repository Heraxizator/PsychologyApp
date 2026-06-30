using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Shared.Common;

public static class UserPreferences
{
    public const string LanguageKey = "Language";
    public const string ThemeKey = "Theme";
    public const string ColorKey = "Color";
    public const string FormKey = "Form";
    public const string SizeKey = "Size";
    public const string IsBoldKey = "IsBold";
    public const string QuestionnaireAutoAdvanceKey = "QuestionnaireAutoAdvance";
    public const string HasCompletedOnboardingKey = "HasCompletedOnboarding";
    public const string OnboardingConcernKey = "OnboardingConcern";
    public const string PendingTechniqueKey = "PendingTechnique";
    public const string PracticeRemindersEnabledKey = "PracticeRemindersEnabled";
    public const string PracticeReminderHourKey = "PracticeReminderHour";

    public const string DefaultLanguage = "ru";
    public const string DefaultTheme = "light";
    public const string DefaultColor = "blue";
    public const string DefaultForm = "rounded";
    public const string DefaultSize = "medium";
    public const int DefaultPracticeReminderHour = 19;

    public static event Action? Changed;

    private static UserPreferencesState? _inMemoryState;

    internal static void UseInMemoryStorage(UserPreferencesState? seed = null) =>
        _inMemoryState = seed ?? new UserPreferencesState();

    internal static void ResetInMemoryStorage() => _inMemoryState = null;

    public static string GetPersistedLanguage() =>
        NormalizeLanguageKey(Load().Language);

    public static UserPreferencesState Load()
    {
        if (_inMemoryState is not null)
        {
            return new UserPreferencesState
            {
                Language = _inMemoryState.Language,
                Theme = _inMemoryState.Theme,
                Color = _inMemoryState.Color,
                Form = _inMemoryState.Form,
                Size = _inMemoryState.Size,
                IsBold = _inMemoryState.IsBold,
                QuestionnaireAutoAdvance = _inMemoryState.QuestionnaireAutoAdvance,
                HasCompletedOnboarding = _inMemoryState.HasCompletedOnboarding,
                OnboardingConcern = _inMemoryState.OnboardingConcern,
                PracticeRemindersEnabled = _inMemoryState.PracticeRemindersEnabled,
                PracticeReminderHour = _inMemoryState.PracticeReminderHour
            };
        }

        return new UserPreferencesState
        {
            Language = NormalizeLanguageKey(Preferences.Get(LanguageKey, DefaultLanguage)),
            Theme = NormalizeThemeKey(Preferences.Get(ThemeKey, DefaultTheme)),
            Color = NormalizeColorKey(Preferences.Get(ColorKey, DefaultColor)),
            Form = NormalizeFormKey(Preferences.Get(FormKey, DefaultForm)),
            Size = NormalizeSizeKey(Preferences.Get(SizeKey, DefaultSize)),
            IsBold = Preferences.Get(IsBoldKey, false),
            QuestionnaireAutoAdvance = Preferences.Get(QuestionnaireAutoAdvanceKey, true),
            HasCompletedOnboarding = Preferences.ContainsKey(HasCompletedOnboardingKey)
                ? Preferences.Get(HasCompletedOnboardingKey, false)
                : Preferences.ContainsKey(LanguageKey)
                  || Preferences.ContainsKey(ThemeKey)
                  || Preferences.ContainsKey(ColorKey),
            OnboardingConcern = Preferences.Get(OnboardingConcernKey, "explore"),
            PracticeRemindersEnabled = Preferences.Get(PracticeRemindersEnabledKey, true),
            PracticeReminderHour = NormalizePracticeReminderHour(Preferences.Get(PracticeReminderHourKey, DefaultPracticeReminderHour))
        };
    }

    public static void Save(UserPreferencesState state)
    {
        if (_inMemoryState is not null)
        {
            _inMemoryState = state;
            return;
        }

        Preferences.Set(LanguageKey, NormalizeLanguageKey(state.Language));
        Preferences.Set(ThemeKey, NormalizeThemeKey(state.Theme));
        Preferences.Set(ColorKey, NormalizeColorKey(state.Color));
        Preferences.Set(FormKey, NormalizeFormKey(state.Form));
        Preferences.Set(SizeKey, NormalizeSizeKey(state.Size));
        Preferences.Set(IsBoldKey, state.IsBold);
        Preferences.Set(QuestionnaireAutoAdvanceKey, state.QuestionnaireAutoAdvance);
        Preferences.Set(HasCompletedOnboardingKey, state.HasCompletedOnboarding);
        Preferences.Set(OnboardingConcernKey, state.OnboardingConcern);
        Preferences.Set(PracticeRemindersEnabledKey, state.PracticeRemindersEnabled);
        Preferences.Set(PracticeReminderHourKey, NormalizePracticeReminderHour(state.PracticeReminderHour));
    }

    public static void SetPendingTechnique(TechniqueId techniqueId) =>
        Preferences.Set(PendingTechniqueKey, techniqueId.ToString());

    public static TechniqueId? ConsumePendingTechnique()
    {
        if (!Preferences.ContainsKey(PendingTechniqueKey))
        {
            return null;
        }

        string value = Preferences.Get(PendingTechniqueKey, string.Empty);
        Preferences.Remove(PendingTechniqueKey);
        return Enum.TryParse(value, out TechniqueId id) ? id : null;
    }

    public static void CompleteOnboarding(string concern)
    {
        UserPreferencesState current = Load();
        Save(new UserPreferencesState
        {
            Language = current.Language,
            Theme = current.Theme,
            Color = current.Color,
            Form = current.Form,
            Size = current.Size,
            IsBold = current.IsBold,
            QuestionnaireAutoAdvance = current.QuestionnaireAutoAdvance,
            HasCompletedOnboarding = true,
            OnboardingConcern = concern,
            PracticeRemindersEnabled = current.PracticeRemindersEnabled,
            PracticeReminderHour = current.PracticeReminderHour
        });
        Changed?.Invoke();
    }

    public static void ApplyAll()
    {
        AppStrings.LanguageOverride = null;
        AppStrings.LanguageProvider = () => Load().Language;
        UserPreferencesState state = Load();
        ApplyLanguage(state.Language);
        ApplyTheme(state.Theme);
        ApplyAccentColor(state.Color);
        ApplyTypography(state.Size, state.IsBold);
        ApplyForm(state.Form);
        Changed?.Invoke();
    }

    public static void ApplyPreview(UserPreferencesState state)
    {
        AppStrings.LanguageOverride = NormalizeLanguageKey(state.Language);
        ApplyLanguage(state.Language);
        ApplyTheme(state.Theme);
        ApplyAccentColor(state.Color);
        ApplyTypography(state.Size, state.IsBold);
        ApplyForm(state.Form);
        Changed?.Invoke();
    }

    public static void ResetOnboardingCompletion()
    {
        UserPreferencesState current = Load();
        Save(new UserPreferencesState
        {
            Language = current.Language,
            Theme = current.Theme,
            Color = current.Color,
            Form = current.Form,
            Size = current.Size,
            IsBold = current.IsBold,
            QuestionnaireAutoAdvance = current.QuestionnaireAutoAdvance,
            HasCompletedOnboarding = false,
            OnboardingConcern = current.OnboardingConcern,
            PracticeRemindersEnabled = current.PracticeRemindersEnabled,
            PracticeReminderHour = current.PracticeReminderHour
        });
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

        (Color primary, Color secondary, Color hover, Color tintLight, Color tintDark) =
            ResolveAccentColors(NormalizeColorKey(color));
        resources["Primary"] = primary;
        resources["Secondary"] = secondary;
        resources["PrimaryHover"] = hover;
        resources["PrimaryTint"] = tintLight;
        resources["PrimaryTintDark"] = tintDark;
    }

    public static void ApplyTypography(string size, bool isBold)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        (double pageTitle, double section, double body) = ResolveFontSizes(NormalizeSizeKey(size));
        resources["PageTitleFontSize"] = pageTitle;
        resources["SectionTitleFontSize"] = section;
        resources["BodyFontSize"] = body;
        resources["CaptionFontSize"] = body;
        resources["NavTitleFontSize"] = section;
        resources["BodyFontFamily"] = isBold ? "InterSemiBold" : "InterRegular";
    }

    public static void ApplyForm(string form)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        double radius = IsRoundedForm(form) ? 12 : 4;
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

    public static string GetLanguageLabel(string key, string? language = null)
    {
        string lang = language ?? Load().Language;
        return NormalizeLanguageKey(key) switch
        {
            "en" => "English",
            _ => IsEnglish(lang) ? "Russian" : "Русский"
        };
    }

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

    public static string ParseThemeKey(string displayOrKey) => NormalizeThemeKey(displayOrKey);

    public static string ParseColorKey(string displayOrKey) => NormalizeColorKey(displayOrKey);

    public static string ParseFormKey(string displayOrKey) => NormalizeFormKey(displayOrKey);

    public static string ParseSizeKey(string displayOrKey) => NormalizeSizeKey(displayOrKey);

    private static bool IsRoundedForm(string form) =>
        NormalizeFormKey(form) == "rounded";

    private static (Color primary, Color secondary, Color hover, Color tintLight, Color tintDark) ResolveAccentColors(string color) =>
        NormalizeColorKey(color) switch
        {
            "red" => (
                Color.FromArgb("#E53935"),
                Color.FromArgb("#FFAB91"),
                Color.FromArgb("#C62828"),
                Color.FromArgb("#FFEBEE"),
                Color.FromArgb("#3D1A1A")),
            "yellow" => (
                Color.FromArgb("#F7B548"),
                Color.FromArgb("#FFE5B9"),
                Color.FromArgb("#E6A020"),
                Color.FromArgb("#FFF8E6"),
                Color.FromArgb("#3D331A")),
            "green" => (
                Color.FromArgb("#2E9E5B"),
                Color.FromArgb("#A8E6C1"),
                Color.FromArgb("#1F7A45"),
                Color.FromArgb("#EBF3F0"),
                Color.FromArgb("#1A3D2A")),
            _ => (
                Color.FromArgb("#0085FF"),
                Color.FromArgb("#0085FF"),
                Color.FromArgb("#006ACC"),
                Color.FromArgb("#E6F2FF"),
                Color.FromArgb("#1A2A3D"))
        };

    private static (double pageTitle, double section, double body) ResolveFontSizes(string size) =>
        NormalizeSizeKey(size) switch
        {
            "large" => (22, 20, 16),
            "small" => (18, 16, 12),
            _ => (20, 18, 14)
        };

    private static void SetCornerRadiusResources(ResourceDictionary resources, double radius)
    {
        double buttonRadius = radius <= 4 ? 4 : 8;
        double entryRadius = radius;
        resources["UiCornerRadius"] = radius;
        resources["UiButtonCornerRadius"] = buttonRadius;
        resources["UiCornerRadiusShape"] = new RoundRectangle { CornerRadius = radius };
        resources["UiCornerRadiusCompactShape"] = new RoundRectangle { CornerRadius = buttonRadius };
        resources["UiCornerRadiusEntryShape"] = new RoundRectangle { CornerRadius = entryRadius };
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
    public bool QuestionnaireAutoAdvance { get; init; } = true;
    public bool HasCompletedOnboarding { get; init; }
    public string OnboardingConcern { get; init; } = "explore";
    public bool PracticeRemindersEnabled { get; init; } = true;
    public int PracticeReminderHour { get; init; } = UserPreferences.DefaultPracticeReminderHour;
}
