using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ProfileSettings;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesStore _userPreferencesStore;
    private readonly SettingsPreferencesPresenter _presenter;
    private readonly LanguageContentReloader _languageContentReloader;
    private readonly IPracticeReminderCoordinator _practiceReminderCoordinator;
    private UserPreferencesState _savedState;

    public SettingsViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IUserPreferencesStore userPreferencesStore,
        SettingsPreferencesPresenter presenter,
        LanguageContentReloader languageContentReloader,
        IPracticeReminderCoordinator practiceReminderCoordinator)
    {
        BindPreferences(userPreferencesStore);
        _dialogService = dialogService;
        _navigationService = navigationService;
        _userPreferencesStore = userPreferencesStore;
        _presenter = presenter;
        _languageContentReloader = languageContentReloader;
        _practiceReminderCoordinator = practiceReminderCoordinator;
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
