using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.TechniqueSession;

public partial class TechniqueSessionViewModel : BaseViewModel
{
    private readonly TechniqueId _techniqueId;
    private readonly IUserProgressService _userProgressService;
    private readonly ListTechniqueSessionHelper _sessionHelper;
    private readonly EntryDraftCoordinator _entryDraftCoordinator;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ICommand BackCommand { get; }
    public ICommand CompleteCommand { get; }

    public TechniqueSessionViewModel(
        TechniqueId techniqueId,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ListTechniqueSessionHelper sessionHelper,
        EntryDraftCoordinator entryDraftCoordinator)
    {
        _techniqueId = techniqueId;
        _userProgressService = userProgressService;
        _sessionHelper = sessionHelper;
        _entryDraftCoordinator = entryDraftCoordinator;
        _entryDraftCoordinator.Attach(_techniqueId, Entries, _userProgressService);

        ApplyTechnique(techniqueId);
        BindNavigation(navigationService);

        BackCommand = new AsyncCommand(GoBackAsync);
        CompleteCommand = new AsyncCommand(CompleteSessionAsync);
        Finish = CompleteCommand;
    }
}
