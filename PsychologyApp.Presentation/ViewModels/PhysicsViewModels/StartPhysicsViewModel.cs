using MobileHelperMaui.Views.PhysicsPages;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.PhysicsViewModels
{
    public class StartPhysicsViewModel : BaseViewModel
    {
        public ICommand Continue { get; set; }
        public StartPhysicsViewModel(INavigation navigation)
        {
            this.Title = "Психосоматик";

            this.Navigation = navigation;

            this.Continue = new Command(async () =>
            {
                SetInit();

                PhysicsSearchViewModel physicsSearchViewModel = new(this.Navigation);

                PhysicsSerchPage physicsSerchPage = new(physicsSearchViewModel);

                await this.Navigation.PushAsync(physicsSerchPage);
            });

            SetDone();
        }

        public StartPhysicsViewModel()
        {

        }
    }
}
