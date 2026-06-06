using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public class StartPhysicsViewModel : BaseViewModel
{
    public IReadOnlyList<string> AlgorithmSteps { get; } =
    [
        "1. Назвать болезнь или часть тела, которая болит",
        "2. Узнать несколько возможных причин."
    ];

    public ICommand StartCommand { get; private set; } = default!;

    public StartPhysicsViewModel(INavigation navigation, INavigationService navigationService)
    {
        ModuleName = "Психосоматик";
        PageName = "С введением";

        BindNavigation(navigation);
        StartCommand = new AsyncCommand(() => navigationService.GoToPhysicsSearchAsync());
    }

    public StartPhysicsViewModel() { }
}
