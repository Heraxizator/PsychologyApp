using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Shared.Common;

public static partial class UserPreferences
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
    public const string PendingQuoteFeedKey = "PendingQuoteFeed";
    public const string PracticeRemindersEnabledKey = "PracticeRemindersEnabled";
    public const string PracticeReminderHourKey = "PracticeReminderHour";
    public const string QuoteRemindersEnabledKey = "QuoteRemindersEnabled";
    public const string QuoteReminderHourKey = "QuoteReminderHour";
    public const string MoodRemindersEnabledKey = "MoodRemindersEnabled";
    public const string MoodReminderHourKey = "MoodReminderHour";
    public const string ChatRemindersEnabledKey = "ChatRemindersEnabled";
    public const string ChatReminderHourKey = "ChatReminderHour";
    public const string PendingOpenJournalKey = "PendingOpenJournal";
    public const string HasUsedPhysicsSearchKey = "HasUsedPhysicsSearch";

    private static readonly WeakActionEvent ChangedHandlers = new();

    // Weak: view models, technique pages and quote cards subscribe here and never unsubscribe.
    public static event Action? Changed
    {
        add
        {
            if (value is not null)
            {
                ChangedHandlers.Add(value);
            }
        }
        remove
        {
            if (value is not null)
            {
                ChangedHandlers.Remove(value);
            }
        }
    }

    private static UserPreferencesState? _inMemoryState;

    internal static void UseInMemoryStorage(UserPreferencesState? seed = null) =>
        _inMemoryState = seed ?? new UserPreferencesState();

    internal static void ResetInMemoryStorage() => _inMemoryState = null;

    public static string GetPersistedLanguage() =>
        NormalizeLanguageKey(Current.Language);

    public static string OnboardingConcern => Current.OnboardingConcern;

    /// <summary>A copy the caller may change and pass to <see cref="Save"/>.</summary>
    public static UserPreferencesState Load() => Copy(Current);

    // Every localized string reads the language through Load (AppStrings.LanguageProvider), so a screen open asked
    // SharedPreferences for ~20 keys over JNI hundreds of times. All writes go through Save, which refreshes this cache.
    private static UserPreferencesState? _persistedCache;

    private static UserPreferencesState Current => _inMemoryState ?? (_persistedCache ??= ReadPersisted());

    private static UserPreferencesState Copy(UserPreferencesState s) => new()
    {
        Language = s.Language,
        Theme = s.Theme,
        Color = s.Color,
        Form = s.Form,
        Size = s.Size,
        IsBold = s.IsBold,
        QuestionnaireAutoAdvance = s.QuestionnaireAutoAdvance,
        HasCompletedOnboarding = s.HasCompletedOnboarding,
        OnboardingConcern = s.OnboardingConcern,
        PracticeRemindersEnabled = s.PracticeRemindersEnabled,
        PracticeReminderHour = s.PracticeReminderHour,
        QuoteRemindersEnabled = s.QuoteRemindersEnabled,
        QuoteReminderHour = s.QuoteReminderHour,
        MoodRemindersEnabled = s.MoodRemindersEnabled,
        MoodReminderHour = s.MoodReminderHour,
        ChatRemindersEnabled = s.ChatRemindersEnabled,
        ChatReminderHour = s.ChatReminderHour
    };

    private static UserPreferencesState ReadPersisted()
    {
        return new UserPreferencesState
        {
            Language = ResolveLanguagePreference(),
            Theme = ResolveThemePreference(),
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
            PracticeReminderHour = NormalizePracticeReminderHour(Preferences.Get(PracticeReminderHourKey, DefaultPracticeReminderHour)),
            QuoteRemindersEnabled = Preferences.Get(QuoteRemindersEnabledKey, false),
            QuoteReminderHour = NormalizeQuoteReminderHour(Preferences.Get(QuoteReminderHourKey, DefaultQuoteReminderHour)),
            MoodRemindersEnabled = Preferences.Get(MoodRemindersEnabledKey, false),
            MoodReminderHour = NormalizeMoodReminderHour(Preferences.Get(MoodReminderHourKey, DefaultMoodReminderHour)),
            ChatRemindersEnabled = Preferences.Get(ChatRemindersEnabledKey, false),
            ChatReminderHour = NormalizeChatReminderHour(Preferences.Get(ChatReminderHourKey, DefaultChatReminderHour))
        };
    }

    public static void Save(UserPreferencesState state)
    {
        if (_inMemoryState is not null)
        {
            _inMemoryState = state;
            return;
        }

        // Re-read after writing: the stored values are normalized, and the caller may keep mutating its instance.
        _persistedCache = null;

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
        Preferences.Set(QuoteRemindersEnabledKey, state.QuoteRemindersEnabled);
        Preferences.Set(QuoteReminderHourKey, NormalizeQuoteReminderHour(state.QuoteReminderHour));
        Preferences.Set(MoodRemindersEnabledKey, state.MoodRemindersEnabled);
        Preferences.Set(MoodReminderHourKey, NormalizeMoodReminderHour(state.MoodReminderHour));
        Preferences.Set(ChatRemindersEnabledKey, state.ChatRemindersEnabled);
        Preferences.Set(ChatReminderHourKey, NormalizeChatReminderHour(state.ChatReminderHour));
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

    public static void SetPendingQuoteFeed(string feedKey) =>
        Preferences.Set(PendingQuoteFeedKey, feedKey);

    public static string? ConsumePendingQuoteFeed()
    {
        if (!Preferences.ContainsKey(PendingQuoteFeedKey))
        {
            return null;
        }

        string value = Preferences.Get(PendingQuoteFeedKey, string.Empty);
        Preferences.Remove(PendingQuoteFeedKey);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static void CompleteOnboarding(
        string concern,
        bool? practiceRemindersEnabled = null,
        int? practiceReminderHour = null)
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
            OnboardingConcern = NormalizeOnboardingConcernKey(concern),
            PracticeRemindersEnabled = practiceRemindersEnabled ?? current.PracticeRemindersEnabled,
            PracticeReminderHour = practiceReminderHour is int hour
                ? NormalizePracticeReminderHour(hour)
                : current.PracticeReminderHour,
            QuoteRemindersEnabled = current.QuoteRemindersEnabled,
            QuoteReminderHour = current.QuoteReminderHour,
            MoodRemindersEnabled = current.MoodRemindersEnabled,
            MoodReminderHour = current.MoodReminderHour,
            ChatRemindersEnabled = current.ChatRemindersEnabled,
            ChatReminderHour = current.ChatReminderHour
        });
        ChangedHandlers.Raise();
    }

    public static void ApplyAll()
    {
        // A language that follows the system one can change while the process lives on.
        _persistedCache = null;
        AppStrings.LanguageOverride = null;
        AppStrings.LanguageProvider = () => Current.Language;
        UserPreferencesState state = Load();
        ApplyLanguage(state.Language);
        ApplyTheme(state.Theme);
        ApplyAccentColor(state.Color);
        ApplyTypography(state.Size, state.IsBold);
        ApplyForm(state.Form);
        ChangedHandlers.Raise();
    }

    public static void ApplyPreview(UserPreferencesState state)
    {
        AppStrings.LanguageOverride = NormalizeLanguageKey(state.Language);
        ApplyLanguage(state.Language);
        ApplyTheme(state.Theme);
        ApplyAccentColor(state.Color);
        ApplyTypography(state.Size, state.IsBold);
        ApplyForm(state.Form);
        ChangedHandlers.Raise();
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
            PracticeReminderHour = current.PracticeReminderHour,
            QuoteRemindersEnabled = current.QuoteRemindersEnabled,
            QuoteReminderHour = current.QuoteReminderHour,
            MoodRemindersEnabled = current.MoodRemindersEnabled,
            MoodReminderHour = current.MoodReminderHour,
            ChatRemindersEnabled = current.ChatRemindersEnabled,
            ChatReminderHour = current.ChatReminderHour
        });
    }

    public static void ApplyTheme()
    {
        ApplyTheme(Current.Theme);
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
        ApplyLanguage(Current.Language);
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

        // The accent as text: the same colour where it reads well, a deeper or lighter shade where it would not (yellow on white).
        resources["PrimaryTextLight"] = Color.FromArgb(PsychologyApp.Presentation.Core.Charts.ColorContrast.Readable(primary.ToArgbHex(), "#F3F4F6"));
        resources["PrimaryTextDark"] = Color.FromArgb(PsychologyApp.Presentation.Core.Charts.ColorContrast.Readable(primary.ToArgbHex(), "#1E1E1E"));

        // What goes on an accent-coloured button: white on a fill deep enough for it, otherwise dark text on a light accent.
        (string fill, string onFill) = PsychologyApp.Presentation.Core.Charts.ColorContrast.ForFill(primary.ToArgbHex());
        resources["PrimaryFill"] = Color.FromArgb(fill);
        resources["OnPrimary"] = Color.FromArgb(onFill);
    }

    public static void ApplyTypography(string size, bool isBold)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources is not ResourceDictionary resources)
        {
            return;
        }

        (double pageTitle, double section, double body, double caption) = PsychologyApp.Presentation.Common.TypographyScale.For(NormalizeSizeKey(size));
        resources["PageTitleFontSize"] = pageTitle;
        resources["SectionTitleFontSize"] = section;
        resources["BodyFontSize"] = body;
        resources["CaptionFontSize"] = caption;
        resources["NavTitleFontSize"] = section;
        resources["QuoteDisplayFontSize"] = body + 4;
        resources["QuoteHeroFontSize"] = body + 6;
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

    public static bool HasUsedPhysicsSearch => Preferences.Get(HasUsedPhysicsSearchKey, false);

    public static void MarkPhysicsSearchUsed() =>
        Preferences.Set(HasUsedPhysicsSearchKey, true);

    public static void SetPendingOpenJournal() =>
        Preferences.Set(PendingOpenJournalKey, true);

    public static bool ConsumePendingOpenJournal()
    {
        if (!Preferences.ContainsKey(PendingOpenJournalKey))
        {
            return false;
        }

        Preferences.Remove(PendingOpenJournalKey);
        return true;
    }

    private static string ResolveLanguagePreference()
    {
        if (Preferences.ContainsKey(LanguageKey))
        {
            return NormalizeLanguageKey(Preferences.Get(LanguageKey, DefaultLanguage));
        }

        string systemLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return string.Equals(systemLanguage, "en", StringComparison.OrdinalIgnoreCase)
            ? "en"
            : "ru";
    }

    private static string ResolveThemePreference()
    {
        if (Preferences.ContainsKey(ThemeKey))
        {
            return NormalizeThemeKey(Preferences.Get(ThemeKey, DefaultTheme));
        }

        AppTheme requestedTheme = Microsoft.Maui.Controls.Application.Current?.RequestedTheme ?? AppTheme.Light;
        return requestedTheme == AppTheme.Dark ? "dark" : "light";
    }

    private static bool IsRoundedForm(string form) =>
        NormalizeFormKey(form) == "rounded";

    private static (Color primary, Color secondary, Color hover, Color tintLight, Color tintDark) ResolveAccentColors(string color) =>
        NormalizeColorKey(color) switch
        {
            "red" => (
                Color.FromArgb("#E53935"),
                Color.FromArgb("#FFAB91"),
                Color.FromArgb("#C62828"),
                Color.FromArgb("#FFE0E0"),
                Color.FromArgb("#3D1A1A")),
            "yellow" => (
                Color.FromArgb("#F7B548"),
                Color.FromArgb("#FFE5B9"),
                Color.FromArgb("#E6A020"),
                Color.FromArgb("#FFF3D6"),
                Color.FromArgb("#3D331A")),
            "green" => (
                Color.FromArgb("#2E9E5B"),
                Color.FromArgb("#A8E6C1"),
                Color.FromArgb("#1F7A45"),
                Color.FromArgb("#DCEFE4"),
                Color.FromArgb("#1A3D2A")),
            _ => (
                Color.FromArgb("#0085FF"),
                Color.FromArgb("#0085FF"),
                Color.FromArgb("#006ACC"),
                Color.FromArgb("#D6EBFF"),
                Color.FromArgb("#1A2A3D"))
        };

    private static void SetCornerRadiusResources(ResourceDictionary resources, double radius)
    {
        double buttonRadius = radius <= 4 ? 4 : 8;
        double entryRadius = radius;
        resources["UiButtonCornerRadius"] = buttonRadius;
        resources["UiCornerRadiusCompactShape"] = new RoundRectangle { CornerRadius = buttonRadius };
        resources["UiCornerRadiusEntryShape"] = new RoundRectangle { CornerRadius = entryRadius };
    }
}
