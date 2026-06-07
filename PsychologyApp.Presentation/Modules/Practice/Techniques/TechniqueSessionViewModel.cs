using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Practice.Techniques;

public class TechniqueSessionViewModel : BaseViewModel
{
    private readonly TechniqueId _techniqueId;
    private readonly IUserProgressService _userProgressService;
    private readonly ITechniqueService? _techniqueService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;

    public ICommand BackCommand { get; }
    public ICommand CompleteCommand { get; }

    public TechniqueSessionViewModel(
        INavigation navigation,
        TechniqueId techniqueId,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        ITechniqueService? techniqueService = null)
    {
        _techniqueId = techniqueId;
        _userProgressService = userProgressService;
        _techniqueService = techniqueService;

        ApplyTechnique(techniqueId);
        BindNavigation(navigation, navigationService);

        BackCommand = new AsyncCommand(GoBackAsync);
        CompleteCommand = new AsyncCommand(CompleteSessionAsync);
        Finish = CompleteCommand;
    }

    private async Task CompleteSessionAsync()
    {
        int durationSeconds = Math.Max(0, (int)(DateTime.UtcNow - _sessionStartedAt).TotalSeconds);
        string itemKey = _techniqueId.ToString();

        await _userProgressService.RecordTechniqueCompletionAsync(
            itemKey,
            ModuleName,
            PageName,
            durationSeconds);
        await _userProgressService.DeleteSessionDraftAsync(itemKey);

        await GoBackAsync();
    }
}
