using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;

public partial class DesignerViewModel : BaseViewModel
{
    private readonly long _techniqueId;
    private readonly ITechniqueService _techniqueService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly DesignerTechniqueOperations _techniqueOperations;
    private readonly ILogger<DesignerViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;

    public ICommand ExecuteTechnique { get; private set; } = default!;

    public DesignerViewModel(
        long techniqueId,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        DesignerTechniqueOperations techniqueOperations,
        ILogger<DesignerViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _techniqueService = techniqueService;
            _techniqueMessenger = techniqueMessenger;
            _techniqueOperations = techniqueOperations;
            _logger = logger;
            _settings = settings;
            _navigationService = navigationService;
            _techniqueId = techniqueId;

            Path = "method.png";

            ModuleName = AppStrings.ShellTabPractice;
            PageName = AppStrings.PracticeConstructor;

            BindNavigation(_navigationService);

            ExecuteTechnique = new AsyncCommand(ExecuteOperationAsync);
            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "DesignerViewModel initialization failed.");
        }
    }
}
