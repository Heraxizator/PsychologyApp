using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Pages.ProfileSettings;

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
            _savedState);

    private void ApplyLivePreview()
    {
        _isApplyingLivePreview = true;
        try
        {
            _userPreferencesStore.ApplyPreview(BuildCurrentState());
        }
        finally
        {
            _isApplyingLivePreview = false;
        }
    }
}
