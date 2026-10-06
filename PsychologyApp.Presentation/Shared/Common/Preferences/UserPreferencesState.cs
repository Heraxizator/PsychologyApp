namespace PsychologyApp.Presentation.Shared.Common;

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
    public bool QuoteRemindersEnabled { get; init; }
    public int QuoteReminderHour { get; init; } = UserPreferences.DefaultQuoteReminderHour;
    public bool MoodRemindersEnabled { get; init; }
    public int MoodReminderHour { get; init; } = UserPreferences.DefaultMoodReminderHour;
    public bool ChatRemindersEnabled { get; init; }
    public int ChatReminderHour { get; init; } = UserPreferences.DefaultChatReminderHour;
}
