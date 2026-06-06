using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.About;

public class InfoViewModel : BaseViewModel
{
    public InfoViewModel(INavigation navigation, INavigationService navigationService)
    {
        ModuleName = "Практик";
        PageName = "О приложении";
        BindNavigation(navigation);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
    }

    public ICommand BackCommand { get; }
}
