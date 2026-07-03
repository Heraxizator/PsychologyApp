using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Application.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession.SubViewModels.Polarity;

public partial class PolarityViewModel : BaseViewModel
{
    private readonly IUserProgressService _userProgressService;
    private readonly ListTechniqueSessionHelper _sessionHelper;
    private readonly PolarityListDraftCoordinator _draftCoordinator;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ICommand Add { get; private set; } = default!;
    public Command<Models.Practice.Techniques.Polarity> Delete { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public ICommand CompleteCommand { get; private set; } = default!;
    public ObservableCollection<Models.Practice.Techniques.Polarity> polarities { get; private set; } = [];

    public PolarityViewModel(
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ListTechniqueSessionHelper sessionHelper,
        PolarityListDraftCoordinator draftCoordinator,
        ITechniqueCatalogService techniqueCatalogService)
    {
        TechniqueCatalogService = techniqueCatalogService;
        _userProgressService = userProgressService;
        _sessionHelper = sessionHelper;
        _draftCoordinator = draftCoordinator;
        _draftCoordinator.Attach(TechniqueId.Polarity.ToString(), _userProgressService);
        ApplyTechnique(TechniqueId.Polarity);
        IsFull = false;
        BindNavigation(navigationService);
        Add = new Command(ToAdd);
        Delete = new Command<Models.Practice.Techniques.Polarity>(DeleteItem);
        BackCommand = new AsyncCommand(GoBackAsync);
        CompleteCommand = new AsyncCommand(CompleteSessionAsync);
        Finish = CompleteCommand;
        LoadDraftAsync().FireAndForget();
    }
}
