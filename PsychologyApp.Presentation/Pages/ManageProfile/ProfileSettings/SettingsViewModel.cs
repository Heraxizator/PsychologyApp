using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

public partial class SettingsViewModel : BaseViewModel
{
    private static readonly TimeSpan AutoSaveDebounceDelay = TimeSpan.FromMilliseconds(700);
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesStore _userPreferencesStore;
    private readonly SettingsPreferencesPresenter _presenter;
    private readonly LanguageContentReloader _languageContentReloader;
    private readonly IPracticeReminderCoordinator _practiceReminderCoordinator;
    private readonly IQuoteReminderCoordinator _quoteReminderCoordinator;
    private readonly IMoodReminderCoordinator _moodReminderCoordinator;
    private readonly bool _areRemindersSupported;
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private CancellationTokenSource? _autoSaveDebounceCts;
    private UserPreferencesState _savedState;

    public bool AreRemindersSupported => _areRemindersSupported;

    /// <summary>On-device AI section; null when the host does not provide one.</summary>
    public LocalAiSettingsViewModel? LocalAi { get; }

    public SettingsViewModel(
        IDialogService dialogService,
        INavigationService navigationService,
        IUserPreferencesStore userPreferencesStore,
        SettingsPreferencesPresenter presenter,
        LanguageContentReloader languageContentReloader,
        IPracticeReminderCoordinator practiceReminderCoordinator,
        IQuoteReminderCoordinator quoteReminderCoordinator,
        IMoodReminderCoordinator moodReminderCoordinator,
        IPracticeReminderScheduler practiceReminderScheduler,
        LocalAiSettingsViewModel? localAi = null)
    {
        BindPreferences(userPreferencesStore);
        LocalAi = localAi;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _userPreferencesStore = userPreferencesStore;
        _presenter = presenter;
        _languageContentReloader = languageContentReloader;
        _practiceReminderCoordinator = practiceReminderCoordinator;
        _quoteReminderCoordinator = quoteReminderCoordinator;
        _moodReminderCoordinator = moodReminderCoordinator;
        _areRemindersSupported = practiceReminderScheduler.IsSupported;
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
