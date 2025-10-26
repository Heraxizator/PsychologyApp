using MobileHelperMaui.Views.AboutPages;
using PsychologyApp.Presentation.Modules.Profile;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.ProfileViewModels;

public class OptionsViewModel : BaseViewModel
{
    public ICommand? OpenAboutPageCommand { get; private set; } = default!;
    public ICommand? OpenDonatePageCommand { get; private set; } = default!;
    //public ICommand? OpenSettingsPageCommand { get; private set; } = default!;

    public OptionsViewModel(INavigation navigation)
    {
        Navigation = navigation;

        OpenAboutPageCommand = new Command(() => Navigation.PushAsync(new InfoPage(), false));

        OpenDonatePageCommand = new Command(() => Navigation.PushAsync(new DonatePage(), false));

        //this.OpenSettingsPageCommand = new Command(() => this.Navigation.PushAsync(new SettingsPage(), false));
    }
}
