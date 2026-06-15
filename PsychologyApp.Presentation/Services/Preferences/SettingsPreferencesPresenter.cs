using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Services.Preferences;

public sealed class SettingsPreferencesPresenter
{
    public string DefaultLanguageLabel => UserPreferences.GetLanguageLabel(UserPreferences.DefaultLanguage);
    public string DefaultThemeLabel => UserPreferences.GetThemeLabel(UserPreferences.DefaultTheme);
    public string DefaultColorLabel => UserPreferences.GetColorLabel(UserPreferences.DefaultColor);
    public string DefaultFormLabel => UserPreferences.GetFormLabel(UserPreferences.DefaultForm);
    public string DefaultSizeLabel => UserPreferences.GetSizeLabel(UserPreferences.DefaultSize);

    public void ApplyDisplayLabels(UserPreferencesState state, Action<string> setLanguage, Action<string> setTheme, Action<string> setColor, Action<string> setForm, Action<string> setSize, Action<bool> setBold)
    {
        setLanguage(UserPreferences.GetLanguageLabel(state.Language));
        setTheme(UserPreferences.GetThemeLabel(state.Theme, state.Language));
        setColor(UserPreferences.GetColorLabel(state.Color, state.Language));
        setForm(UserPreferences.GetFormLabel(state.Form, state.Language));
        setSize(UserPreferences.GetSizeLabel(state.Size, state.Language));
        setBold(state.IsBold);
    }

    public UserPreferencesState BuildState(
        string language,
        string theme,
        string color,
        string form,
        string size,
        bool isBold,
        UserPreferencesState savedState) =>
        new()
        {
            Language = UserPreferences.ParseLanguageKey(language),
            Theme = UserPreferences.ParseThemeKey(theme),
            Color = UserPreferences.ParseColorKey(color),
            Form = UserPreferences.ParseFormKey(form),
            Size = UserPreferences.ParseSizeKey(size),
            IsBold = isBold,
            HasCompletedOnboarding = savedState.HasCompletedOnboarding,
            OnboardingConcern = savedState.OnboardingConcern
        };

    public void RefreshLocalizedLabels(
        string language,
        string theme,
        string color,
        string form,
        string size,
        Action<string> setLanguage,
        Action<string> setTheme,
        Action<string> setColor,
        Action<string> setForm,
        Action<string> setSize,
        Action<IReadOnlyList<string>> setLanguageOptions,
        Action<IReadOnlyList<string>> setThemeOptions,
        Action<IReadOnlyList<string>> setColorOptions,
        Action<IReadOnlyList<string>> setFormOptions,
        Action<IReadOnlyList<string>> setSizeOptions)
    {
        string languageKey = UserPreferences.ParseLanguageKey(language);
        string newLanguage = UserPreferences.ParseLanguageKey(language);
        setLanguage(language);
        setTheme(UserPreferences.GetThemeLabel(UserPreferences.ParseThemeKey(theme), newLanguage));
        setColor(UserPreferences.GetColorLabel(UserPreferences.ParseColorKey(color), newLanguage));
        setForm(UserPreferences.GetFormLabel(UserPreferences.ParseFormKey(form), newLanguage));
        setSize(UserPreferences.GetSizeLabel(UserPreferences.ParseSizeKey(size), newLanguage));
        setLanguageOptions(UserPreferences.GetLanguageOptions(languageKey));
        setThemeOptions(UserPreferences.GetThemeOptions(languageKey));
        setColorOptions(UserPreferences.GetColorOptions(languageKey));
        setFormOptions(UserPreferences.GetFormOptions(languageKey));
        setSizeOptions(UserPreferences.GetSizeOptions(languageKey));
    }
}
