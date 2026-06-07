using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.ViewModels.Settings;

public class SettingsViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    public SettingsViewModel(INavigation navigation, IDialogService dialogService, INavigationService navigationService)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.SettingsTitle;

        BindNavigation(navigation);
        LoadFromPreferences();
        RefreshLocalizedCollections();

        Finish = new AsyncCommand(ToEndAsync);
    }

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
    public string LanguagePickerTitle => AppStrings.SettingsPickerLanguages;
    public string ThemePickerTitle => AppStrings.SettingsPickerOptions;
    public string ColorPickerTitle => AppStrings.SettingsPickerColors;
    public string FormPickerTitle => AppStrings.SettingsPickerShapes;
    public string SizePickerTitle => AppStrings.SettingsPickerSizes;

    public IReadOnlyList<string> LanguageOptions { get; private set; } = [];
    public IReadOnlyList<string> ThemeOptions { get; private set; } = [];
    public IReadOnlyList<string> ColorOptions { get; private set; } = [];
    public IReadOnlyList<string> FormOptions { get; private set; } = [];
    public IReadOnlyList<string> SizeOptions { get; private set; } = [];

    private void LoadFromPreferences()
    {
        UserPreferencesState state = UserPreferences.Load();
        Language = UserPreferences.GetLanguageLabel(state.Language);
        Theme = UserPreferences.GetThemeLabel(state.Theme, state.Language);
        Color = UserPreferences.GetColorLabel(state.Color, state.Language);
        Form = UserPreferences.GetFormLabel(state.Form, state.Language);
        Size = UserPreferences.GetSizeLabel(state.Size, state.Language);
        IsThick = state.IsBold;
    }

    private void RefreshLocalizedCollections()
    {
        string language = UserPreferences.ParseLanguageKey(Language);
        LanguageOptions = UserPreferences.GetLanguageOptions(language);
        ThemeOptions = UserPreferences.GetThemeOptions(language);
        ColorOptions = UserPreferences.GetColorOptions(language);
        FormOptions = UserPreferences.GetFormOptions(language);
        SizeOptions = UserPreferences.GetSizeOptions(language);

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

    private async Task ToEndAsync()
    {
        string language = UserPreferences.ParseLanguageKey(Language);
        UserPreferencesState current = UserPreferences.Load();

        UserPreferences.Save(new UserPreferencesState
        {
            Language = language,
            Theme = UserPreferences.ParseThemeKey(Theme),
            Color = UserPreferences.ParseColorKey(Color),
            Form = UserPreferences.ParseFormKey(Form),
            Size = UserPreferences.ParseSizeKey(Size),
            IsBold = IsThick,
            HasCompletedOnboarding = current.HasCompletedOnboarding,
            OnboardingConcern = current.OnboardingConcern
        });

        UserPreferences.ApplyAll();
        await _dialogService.ShowAsync(AppStrings.SettingsAppliedTitle, AppStrings.SettingsAppliedMessage);
        await _navigationService.GoBackAsync();
    }

    public string language { get; private set; } = UserPreferences.GetLanguageLabel(UserPreferences.DefaultLanguage);
    public string Language
    {
        get => language;
        set
        {
            if (language != value)
            {
                string themeKey = UserPreferences.ParseThemeKey(Theme);
                string colorKey = UserPreferences.ParseColorKey(Color);
                string formKey = UserPreferences.ParseFormKey(Form);
                string sizeKey = UserPreferences.ParseSizeKey(Size);

                language = value;
                string newLanguage = UserPreferences.ParseLanguageKey(value);
                Theme = UserPreferences.GetThemeLabel(themeKey, newLanguage);
                Color = UserPreferences.GetColorLabel(colorKey, newLanguage);
                Form = UserPreferences.GetFormLabel(formKey, newLanguage);
                Size = UserPreferences.GetSizeLabel(sizeKey, newLanguage);
                RefreshLocalizedCollections();
                OnPropertyChanged(nameof(Language));
            }
        }
    }

    public string theme { get; private set; } = UserPreferences.GetThemeLabel(UserPreferences.DefaultTheme);
    public string Theme
    {
        get => theme;
        set
        {
            if (theme != value)
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
    }

    public string color { get; private set; } = UserPreferences.GetColorLabel(UserPreferences.DefaultColor);
    public string Color
    {
        get => color;
        set
        {
            if (color != value)
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    public string form { get; private set; } = UserPreferences.GetFormLabel(UserPreferences.DefaultForm);
    public string Form
    {
        get => form;
        set
        {
            if (form != value)
            {
                form = value;
                OnPropertyChanged(nameof(Form));
            }
        }
    }

    public string size { get; private set; } = UserPreferences.GetSizeLabel(UserPreferences.DefaultSize);
    public string Size
    {
        get => size;
        set
        {
            if (size != value)
            {
                size = value;
                OnPropertyChanged(nameof(Size));
            }
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
            }
        }
    }
}
