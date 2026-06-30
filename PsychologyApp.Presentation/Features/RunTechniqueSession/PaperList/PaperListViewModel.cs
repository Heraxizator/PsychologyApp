using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Application.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.PaperList;

public partial class PaperListViewModel : BaseViewModel
{
    private readonly bool _clearTextAfterAdd;
    private readonly TechniqueId _techniqueId;
    private readonly IUserProgressService _userProgressService;
    private readonly ListTechniqueSessionHelper _sessionHelper;
    private readonly PaperListDraftCoordinator _draftCoordinator;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ObservableCollection<Paper> PapersObservableCollection { get; private set; } = [];
    public Command AddCommand { get; private set; } = default!;
    public Command<Paper> DeleteCommand { get; private set; } = default!;
    public Command BackCommand { get; private set; } = default!;
    public Command CompleteCommand { get; private set; } = default!;

    public PaperListViewModel(
        INavigationService navigationService,
        TechniqueId techniqueId,
        bool clearTextAfterAdd,
        IUserProgressService userProgressService,
        ListTechniqueSessionHelper sessionHelper,
        PaperListDraftCoordinator draftCoordinator,
        ITechniqueCatalogService techniqueCatalogService)
    {
        TechniqueCatalogService = techniqueCatalogService;
        _techniqueId = techniqueId;
        _clearTextAfterAdd = clearTextAfterAdd;
        _userProgressService = userProgressService;
        _sessionHelper = sessionHelper;
        _draftCoordinator = draftCoordinator;
        _draftCoordinator.Attach(_techniqueId.ToString(), _userProgressService);
        BindNavigation(navigationService);
        ApplyTechnique(techniqueId);
        AddCommand = new Command(ToAdd);
        DeleteCommand = new Command<Paper>(DeleteItem);
        BackCommand = new Command(async () => await GoBackAsync());
        CompleteCommand = new Command(async () => await CompleteSessionAsync());
        Finish = CompleteCommand;
        LoadDraftAsync().FireAndForget();
    }
}
