using MobileHelperMaui.Views.PhysicsPages;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.PhysicsViewModels;

public class StartPhysicsViewModel : BaseViewModel
{
    public StartPhysicsViewModel(INavigation navigation)
    {
        this.ModuleName = "Психосоматик";
        this.PageName = "О психосоматике";

        Navigation = navigation;
    }

    public StartPhysicsViewModel() { }
}
