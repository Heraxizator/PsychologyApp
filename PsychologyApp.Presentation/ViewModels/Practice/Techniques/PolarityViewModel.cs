using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class PolarityViewModel : BaseViewModel
{
    private readonly IUserProgressService _userProgressService;
    private readonly ListTechniqueSessionHelper _sessionHelper;
    private readonly PolarityListDraftCoordinator _draftCoordinator;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ICommand Add { get; private set; } = default!;
    public Command<Polarity> Delete { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public ICommand CompleteCommand { get; private set; } = default!;
    public ObservableCollection<Polarity> polarities { get; private set; } = [];

    public PolarityViewModel(
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ListTechniqueSessionHelper sessionHelper,
        PolarityListDraftCoordinator draftCoordinator)
    {
        _userProgressService = userProgressService;
        _sessionHelper = sessionHelper;
        _draftCoordinator = draftCoordinator;
        _draftCoordinator.Attach(TechniqueId.Polarity.ToString(), _userProgressService);
        ApplyTechnique(TechniqueId.Polarity);
        IsFull = false;
        BindNavigation(navigationService);
        Add = new Command(ToAdd);
        Delete = new Command<Polarity>(DeleteItem);
        BackCommand = new AsyncCommand(GoBackAsync);
        CompleteCommand = new AsyncCommand(CompleteSessionAsync);
        Finish = CompleteCommand;
        LoadDraftAsync().FireAndForget();
    }
}
