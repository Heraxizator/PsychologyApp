using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.Techniques;

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
    private readonly ILogger<TechniquesViewModel> _logger;

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
        IOptions<AppSettings> settings,
        ILogger<TechniquesViewModel> logger)
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
        _logger = logger;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeTechniquesList;

        WireCommands();
        SubscribeToTechniqueChanges();
    }
}
