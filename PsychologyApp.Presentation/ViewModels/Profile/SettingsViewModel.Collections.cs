using PsychologyApp.Presentation.Services.Preferences;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class SettingsViewModel
{
    public IReadOnlyList<string> LanguageOptions { get; private set; } = [];
    public IReadOnlyList<string> ThemeOptions { get; private set; } = [];
    public IReadOnlyList<string> ColorOptions { get; private set; } = [];
    public IReadOnlyList<string> FormOptions { get; private set; } = [];
    public IReadOnlyList<string> SizeOptions { get; private set; } = [];

    private void LoadFromPreferences()
    {
        _presenter.ApplyDisplayLabels(
            _savedState,
            value => language = value,
            value => theme = value,
            value => color = value,
            value => form = value,
            value => size = value,
            value => isThick = value);

        OnPropertyChanged(nameof(Language));
        OnPropertyChanged(nameof(Theme));
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(Form));
        OnPropertyChanged(nameof(Size));
        OnPropertyChanged(nameof(IsThick));
    }

    private void RefreshLocalizedCollections()
    {
        _presenter.RefreshLocalizedLabels(
            Language,
            Theme,
            Color,
            Form,
            Size,
            value => language = value,
            value => theme = value,
            value => color = value,
            value => form = value,
            value => size = value,
            value => LanguageOptions = value,
            value => ThemeOptions = value,
            value => ColorOptions = value,
            value => FormOptions = value,
            value => SizeOptions = value);

        OnPropertyChanged(nameof(LanguageOptions));
        OnPropertyChanged(nameof(ThemeOptions));
        OnPropertyChanged(nameof(ColorOptions));
        OnPropertyChanged(nameof(FormOptions));
        OnPropertyChanged(nameof(SizeOptions));
        NotifyLocalizedLabelsChanged();
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
        RefreshLocalizedCollections();
    }
}
