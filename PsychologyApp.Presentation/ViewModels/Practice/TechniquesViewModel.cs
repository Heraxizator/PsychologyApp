using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Practice;

public partial class TechniquesViewModel : BaseViewModel
{
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private readonly ITechniqueService _techniqueService;
    private readonly IToastService _toastService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly INavigationService _navigationService;
    private readonly IUserProgressService _userProgressService;
    private readonly TechniqueListBuilder _techniqueListBuilder;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly PracticeDashboardLoader _dashboardLoader;
    private readonly TechniquesListInitializer _listInitializer;
    private readonly IOptions<AppSettings> _settings;

    public TechniquesViewModel(
        ITechniqueService techniqueService,
        IToastService toastService,
        ITechniqueMessenger techniqueMessenger,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        TechniqueListBuilder techniqueListBuilder,
        IDatabaseReadySignal databaseReadySignal,
        PracticeDashboardLoader dashboardLoader,
        TechniquesListInitializer listInitializer,
        IOptions<AppSettings> settings)
    {
        _techniqueService = techniqueService;
        _toastService = toastService;
        _techniqueMessenger = techniqueMessenger;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _techniqueListBuilder = techniqueListBuilder;
        _databaseReadySignal = databaseReadySignal;
        _dashboardLoader = dashboardLoader;
        _listInitializer = listInitializer;
        _settings = settings;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeTechniquesList;

        WireCommands();
        SubscribeToTechniqueChanges();
    }
}
