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
            ApplyLivePreview();
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
            ApplyLivePreview();
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
            ApplyLivePreview();
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
            ApplyLivePreview();
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
            ApplyLivePreview();
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
                ApplyLivePreview();
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
                ApplyLivePreview();
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
                ApplyLivePreview();
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
            ApplyLivePreview();
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
                ApplyLivePreview();
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
            ApplyLivePreview();
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
            ApplyLivePreview();
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
}
