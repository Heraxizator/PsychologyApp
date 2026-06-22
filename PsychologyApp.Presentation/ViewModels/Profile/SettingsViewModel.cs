using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Preferences;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesStore _userPreferencesStore;
    private readonly SettingsPreferencesPresenter _presenter;
    private readonly LanguageContentReloader _languageContentReloader;
    private UserPreferencesState _savedState;

    public SettingsViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IUserPreferencesStore userPreferencesStore,
        SettingsPreferencesPresenter presenter,
        LanguageContentReloader languageContentReloader)
    {
        BindPreferences(userPreferencesStore);
        _dialogService = dialogService;
        _navigationService = navigationService;
        _userPreferencesStore = userPreferencesStore;
        _presenter = presenter;
        _languageContentReloader = languageContentReloader;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.SettingsTitle;

        BindNavigation(navigationService);
        _savedState = _userPreferencesStore.Load();
        LoadFromPreferences();
        RefreshPickerDisplays();
        NotifyLocalizedLabelsChanged();

        Finish = new AsyncCommand(RevertAndGoBackAsync);
        ApplyCommand = new AsyncCommand(ToEndAsync);
        ReplayOnboardingCommand = new AsyncCommand(ReplayOnboardingAsync);
    }

    public ICommand ApplyCommand { get; }
    public ICommand ReplayOnboardingCommand { get; }
}
