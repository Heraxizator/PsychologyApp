using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ProfileSettings;

public partial class SettingsViewModel
{
    private bool _isApplyingLivePreview;
    private bool _isSyncingPickers;

    public IReadOnlyList<string> LanguageOptions { get; private set; } = [];
    public IReadOnlyList<string> ThemeOptions { get; private set; } = [];
    public IReadOnlyList<string> ColorOptions { get; private set; } = [];
    public IReadOnlyList<string> FormOptions { get; private set; } = [];
    public IReadOnlyList<string> SizeOptions { get; private set; } = [];
    public IReadOnlyList<string> PracticeReminderHourOptions { get; private set; } = [];

    private void LoadFromPreferences()
    {
        _isSyncingPickers = true;
        try
        {
            _presenter.ApplyKeysFromState(
                _savedState,
                value => language = value,
                value => theme = value,
                value => color = value,
                value => form = value,
                value => size = value,
                value => isThick = value,
                value => questionnaireAutoAdvance = value,
                value => practiceRemindersEnabled = value,
                value => practiceReminderHour = value);

            NotifyPickerValuesChanged();
            OnPropertyChanged(nameof(IsThick));
            OnPropertyChanged(nameof(QuestionnaireAutoAdvance));
            OnPropertyChanged(nameof(PracticeRemindersEnabled));
        }
        finally
        {
            _isSyncingPickers = false;
        }
    }

    private void RefreshPickerDisplays()
    {
        LanguageOptions = UserPreferences.LanguageKeys.ToArray();
        ThemeOptions = UserPreferences.ThemeKeys.ToArray();
        ColorOptions = UserPreferences.ColorKeys.ToArray();
        FormOptions = UserPreferences.FormKeys.ToArray();
        SizeOptions = UserPreferences.SizeKeys.ToArray();
        PracticeReminderHourOptions = UserPreferences.GetPracticeReminderHourOptions().ToArray();

        OnPropertyChanged(nameof(LanguageOptions));
        OnPropertyChanged(nameof(ThemeOptions));
        OnPropertyChanged(nameof(ColorOptions));
        OnPropertyChanged(nameof(FormOptions));
        OnPropertyChanged(nameof(SizeOptions));
        OnPropertyChanged(nameof(PracticeReminderHourOptions));
    }

    private void NotifyPickerValuesChanged()
    {
        OnPropertyChanged(nameof(Language));
        OnPropertyChanged(nameof(Theme));
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(Form));
        OnPropertyChanged(nameof(Size));
        OnPropertyChanged(nameof(PracticeReminderHour));
    }

    private void NotifyLocalizedLabelsChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(DesignSectionTitle));
        OnPropertyChanged(nameof(FontSectionTitle));
        OnPropertyChanged(nameof(LanguageLabel));
        OnPropertyChanged(nameof(ThemeLabel));
        OnPropertyChanged(nameof(ColorLabel));
        OnPropertyChanged(nameof(FormLabel));
        OnPropertyChanged(nameof(SizeLabel));
        OnPropertyChanged(nameof(BoldLabel));
        OnPropertyChanged(nameof(TestsSectionTitle));
        OnPropertyChanged(nameof(QuestionnaireAutoAdvanceLabel));
        OnPropertyChanged(nameof(PracticeSectionTitle));
        OnPropertyChanged(nameof(PracticeRemindersLabel));
        OnPropertyChanged(nameof(PracticeReminderHourLabel));
        OnPropertyChanged(nameof(PracticeReminderHourPickerTitle));
        OnPropertyChanged(nameof(ApplyButtonText));
        OnPropertyChanged(nameof(ReplayOnboardingText));
        OnPropertyChanged(nameof(FormHelperText));
        OnPropertyChanged(nameof(ColorHelperText));
        OnPropertyChanged(nameof(LanguagePickerTitle));
        OnPropertyChanged(nameof(ThemePickerTitle));
        OnPropertyChanged(nameof(ColorPickerTitle));
        OnPropertyChanged(nameof(FormPickerTitle));
        OnPropertyChanged(nameof(SizePickerTitle));
    }

    protected override void RefreshLocalizedProperties()
    {
        if (_isApplyingLivePreview)
        {
            RefreshPickerDisplays();
            NotifyLocalizedLabelsChanged();
            return;
        }

        LoadFromPreferences();
        RefreshPickerDisplays();
        NotifyLocalizedLabelsChanged();
    }
}
