using MobileHelperMaui.Views.AboutPages;
using MobileHelperMaui.Views.SettingsPages;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.Modules.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        this.OpenAboutPageCommand = new Command(() => Navigation.PushAsync(new InfoPage(), false));

        this.OpenDonatePageCommand = new Command(() => Navigation.PushAsync(new DonatePage(), false));

        //this.OpenSettingsPageCommand = new Command(() => this.Navigation.PushAsync(new SettingsPage(), false));
    }
}
