using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

public partial class SettingsViewModel
{
    public string language { get; private set; } = string.Empty;
    public string Language
    {
        get => language;
        set
        {
            string normalized = UserPreferences.ParseLanguageKey(value);
            if (_isSyncingPickers || string.Equals(language, normalized, StringComparison.Ordinal))
            {
                return;
            }

            language = normalized;
            OnPropertyChanged(nameof(Language));
            OnSettingsChanged();
        }
    }

    public string theme { get; private set; } = string.Empty;
    public string Theme
    {
        get => theme;
        set
        {
            string normalized = UserPreferences.ParseThemeKey(value);
            if (_isSyncingPickers || string.Equals(theme, normalized, StringComparison.Ordinal))
            {
                return;
            }

            theme = normalized;
            OnPropertyChanged(nameof(Theme));
            OnSettingsChanged();
        }
    }

    public string color { get; private set; } = string.Empty;
    public string Color
    {
        get => color;
        set
        {
            string normalized = UserPreferences.ParseColorKey(value);
            if (_isSyncingPickers || string.Equals(color, normalized, StringComparison.Ordinal))
            {
                return;
            }

            color = normalized;
            OnPropertyChanged(nameof(Color));
            OnSettingsChanged();
        }
    }

    public string form { get; private set; } = string.Empty;
    public string Form
    {
        get => form;
        set
        {
            string normalized = UserPreferences.ParseFormKey(value);
            if (_isSyncingPickers || string.Equals(form, normalized, StringComparison.Ordinal))
            {
                return;
            }

            form = normalized;
            OnPropertyChanged(nameof(Form));
            OnSettingsChanged();
        }
    }

    public string size { get; private set; } = string.Empty;
    public string Size
    {
        get => size;
        set
        {
            string normalized = UserPreferences.ParseSizeKey(value);
            if (_isSyncingPickers || string.Equals(size, normalized, StringComparison.Ordinal))
            {
                return;
            }

            size = normalized;
            OnPropertyChanged(nameof(Size));
            OnSettingsChanged();
        }
    }

    public bool isThick;
    public bool IsThick
    {
        get => isThick;
        set
        {
            if (isThick != value)
            {
                isThick = value;
                OnPropertyChanged(nameof(IsThick));
                OnSettingsChanged();
            }
        }
    }

    public bool questionnaireAutoAdvance = true;
    public bool QuestionnaireAutoAdvance
    {
        get => questionnaireAutoAdvance;
        set
        {
            if (questionnaireAutoAdvance != value)
            {
                questionnaireAutoAdvance = value;
                OnPropertyChanged(nameof(QuestionnaireAutoAdvance));
                OnSettingsChanged();
            }
        }
    }

    public bool practiceRemindersEnabled = true;
    public bool PracticeRemindersEnabled
    {
        get => practiceRemindersEnabled;
        set
        {
            if (practiceRemindersEnabled != value)
            {
                practiceRemindersEnabled = value;
                OnPropertyChanged(nameof(PracticeRemindersEnabled));
                OnSettingsChanged();
            }
        }
    }

    private int practiceReminderHour = UserPreferences.DefaultPracticeReminderHour;
    public string PracticeReminderHour
    {
        get => UserPreferences.GetPracticeReminderHourLabel(practiceReminderHour);
        set
        {
            int normalized = UserPreferences.ParsePracticeReminderHourKey(value);
            if (_isSyncingPickers || practiceReminderHour == normalized)
            {
                return;
            }

            practiceReminderHour = normalized;
            OnPropertyChanged(nameof(PracticeReminderHour));
            OnSettingsChanged();
        }
    }

    public bool quoteRemindersEnabled;
    public bool QuoteRemindersEnabled
    {
        get => quoteRemindersEnabled;
        set
        {
            if (quoteRemindersEnabled != value)
            {
                quoteRemindersEnabled = value;
                OnPropertyChanged(nameof(QuoteRemindersEnabled));
                OnSettingsChanged();
            }
        }
    }

    private int quoteReminderHour = UserPreferences.DefaultQuoteReminderHour;
    public string QuoteReminderHour
    {
        get => UserPreferences.GetQuoteReminderHourLabel(quoteReminderHour);
        set
        {
            int normalized = UserPreferences.ParseQuoteReminderHourKey(value);
            if (_isSyncingPickers || quoteReminderHour == normalized)
            {
                return;
            }

            quoteReminderHour = normalized;
            OnPropertyChanged(nameof(QuoteReminderHour));
            OnSettingsChanged();
        }
    }

    public bool moodRemindersEnabled;
    public bool MoodRemindersEnabled
    {
        get => moodRemindersEnabled;
        set
        {
            if (moodRemindersEnabled != value)
            {
                moodRemindersEnabled = value;
                OnPropertyChanged(nameof(MoodRemindersEnabled));
                OnSettingsChanged();
            }
        }
    }

    private int moodReminderHour = UserPreferences.DefaultMoodReminderHour;
    public string MoodReminderHour
    {
        get => UserPreferences.GetQuoteReminderHourLabel(moodReminderHour);
        set
        {
            int normalized = UserPreferences.ParseQuoteReminderHourKey(value);
            if (_isSyncingPickers || moodReminderHour == normalized)
            {
                return;
            }

            moodReminderHour = normalized;
            OnPropertyChanged(nameof(MoodReminderHour));
            OnSettingsChanged();
        }
    }

    public bool chatRemindersEnabled;
    public bool ChatRemindersEnabled
    {
        get => chatRemindersEnabled;
        set
        {
            if (chatRemindersEnabled != value)
            {
                chatRemindersEnabled = value;
                OnPropertyChanged(nameof(ChatRemindersEnabled));
                OnSettingsChanged();
            }
        }
    }

    private int chatReminderHour = UserPreferences.DefaultChatReminderHour;
    public string ChatReminderHour
    {
        get => UserPreferences.GetChatReminderHourLabel(chatReminderHour);
        set
        {
            int normalized = UserPreferences.ParseChatReminderHourKey(value);
            if (_isSyncingPickers || chatReminderHour == normalized)
            {
                return;
            }

            chatReminderHour = normalized;
            OnPropertyChanged(nameof(ChatReminderHour));
            OnSettingsChanged();
        }
    }

    private string onboardingConcern = UserPreferences.DefaultOnboardingConcern;
    public string OnboardingConcern
    {
        get => onboardingConcern;
        set
        {
            string normalized = UserPreferences.ParseOnboardingConcernKey(value);
            if (_isSyncingPickers || string.Equals(onboardingConcern, normalized, StringComparison.Ordinal))
            {
                return;
            }

            onboardingConcern = normalized;
            OnPropertyChanged(nameof(OnboardingConcern));
            OnSettingsChanged();
        }
    }

    private UserPreferencesState BuildCurrentState() =>
        _presenter.BuildState(
            Language,
            Theme,
            Color,
            Form,
            Size,
            IsThick,
            QuestionnaireAutoAdvance,
            PracticeRemindersEnabled,
            practiceReminderHour,
            QuoteRemindersEnabled,
            quoteReminderHour,
            MoodRemindersEnabled,
            moodReminderHour,
            ChatRemindersEnabled,
            chatReminderHour,
            OnboardingConcern,
            _savedState);

    private void ApplyLivePreview()
    {
        _isApplyingLivePreview = true;
        try
        {
            _presenter.ApplyLivePreview(_userPreferencesStore, BuildCurrentState());
        }
        finally
        {
            _isApplyingLivePreview = false;
        }
    }

    private void OnSettingsChanged()
    {
        ApplyLivePreview();
        QueueAutoSave();
    }
}
