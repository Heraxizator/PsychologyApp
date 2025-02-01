using MobileHelperMaui.Views.PhysicsPages;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.PhysicsViewModels;

public class StartPhysicsViewModel : BaseViewModel
{
    public ICommand Continue { get; private set; } = default!;

    public StartPhysicsViewModel(INavigation navigation)
    {
        this.ModuleName = "Психосоматик";
        this.PageName = "О психосоматике";

        Navigation = navigation;

        this.Continue = new Command(async () =>
        {
            SetInit();

            PhysicsSearchViewModel physicsSearchViewModel = new(Navigation);

            PhysicsSerchPage physicsSerchPage = new(physicsSearchViewModel);

            await Navigation.PushAsync(physicsSerchPage, false);
        });

        SetDone();
    }

    public StartPhysicsViewModel() { }
}
