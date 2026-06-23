using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ProfileSettings;

public partial class SettingsViewModel
{
    public string PageTitle => AppStrings.SettingsTitle;
    public string DesignSectionTitle => AppStrings.SettingsDesignSection;
    public string FontSectionTitle => AppStrings.SettingsFontSection;
    public string LanguageLabel => AppStrings.SettingsLanguageLabel;
    public string ThemeLabel => AppStrings.SettingsThemeLabel;
    public string ColorLabel => AppStrings.SettingsColorLabel;
    public string FormLabel => AppStrings.SettingsFormLabel;
    public string SizeLabel => AppStrings.SettingsSizeLabel;
    public string BoldLabel => AppStrings.SettingsBoldLabel;
    public string ApplyButtonText => AppStrings.SettingsApplyButton;
    public string ReplayOnboardingText => AppStrings.SettingsReplayOnboarding;
    public string FormHelperText => AppStrings.SettingsFormHelper;
    public string ColorHelperText => AppStrings.SettingsColorHelper;
    public string LanguagePickerTitle => AppStrings.SettingsPickerLanguages;
    public string ThemePickerTitle => AppStrings.SettingsPickerOptions;
    public string ColorPickerTitle => AppStrings.SettingsPickerColors;
    public string FormPickerTitle => AppStrings.SettingsPickerShapes;
    public string SizePickerTitle => AppStrings.SettingsPickerSizes;
}
