using PsychologyApp.Presentation.Shared.Common;

using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class SettingsPreferencesPresenter
{
    public string DefaultLanguageLabel => UserPreferences.GetLanguageLabel(UserPreferences.DefaultLanguage);
    public string DefaultThemeLabel => UserPreferences.GetThemeLabel(UserPreferences.DefaultTheme);
    public string DefaultColorLabel => UserPreferences.GetColorLabel(UserPreferences.DefaultColor);
    public string DefaultFormLabel => UserPreferences.GetFormLabel(UserPreferences.DefaultForm);
    public string DefaultSizeLabel => UserPreferences.GetSizeLabel(UserPreferences.DefaultSize);

    public void ApplyKeysFromState(
        UserPreferencesState state,
        Action<string> setLanguage,
        Action<string> setTheme,
        Action<string> setColor,
        Action<string> setForm,
        Action<string> setSize,
        Action<bool> setBold,
        Action<bool> setQuestionnaireAutoAdvance,
        Action<bool> setPracticeRemindersEnabled,
        Action<int> setPracticeReminderHour)
    {
        setLanguage(UserPreferences.ParseLanguageKey(state.Language));
        setTheme(UserPreferences.ParseThemeKey(state.Theme));
        setColor(UserPreferences.ParseColorKey(state.Color));
        setForm(UserPreferences.ParseFormKey(state.Form));
        setSize(UserPreferences.ParseSizeKey(state.Size));
        setBold(state.IsBold);
        setQuestionnaireAutoAdvance(state.QuestionnaireAutoAdvance);
        setPracticeRemindersEnabled(state.PracticeRemindersEnabled);
        setPracticeReminderHour(UserPreferences.NormalizePracticeReminderHour(state.PracticeReminderHour));
    }

    public UserPreferencesState BuildState(
        string language,
        string theme,
        string color,
        string form,
        string size,
        bool isBold,
        bool questionnaireAutoAdvance,
        bool practiceRemindersEnabled,
        int practiceReminderHour,
        UserPreferencesState savedState) =>
        new()
        {
            Language = UserPreferences.ParseLanguageKey(language),
            Theme = UserPreferences.ParseThemeKey(theme),
            Color = UserPreferences.ParseColorKey(color),
            Form = UserPreferences.ParseFormKey(form),
            Size = UserPreferences.ParseSizeKey(size),
            IsBold = isBold,
            QuestionnaireAutoAdvance = questionnaireAutoAdvance,
            PracticeRemindersEnabled = practiceRemindersEnabled,
            PracticeReminderHour = UserPreferences.NormalizePracticeReminderHour(practiceReminderHour),
            HasCompletedOnboarding = savedState.HasCompletedOnboarding,
            OnboardingConcern = savedState.OnboardingConcern
        };

    public void ApplyLivePreview(IUserPreferencesStore store, UserPreferencesState state) =>
        store.ApplyPreview(state);
}
