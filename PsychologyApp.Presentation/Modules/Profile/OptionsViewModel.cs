using MobileHelperMaui.Views.AboutPages;
using MobileHelperMaui.Views.SettingsPages;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
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
    public ICommand? OpenSettingsPageCommand { get; private set; } = default!;

    public OptionsViewModel(INavigation navigation)
    {
        this.Navigation = navigation;

        if (this.Navigation is null)
        {
            Base.ServiceLocator.ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "this.Navigation имеет значение null");
            return;
        }

        this.OpenAboutPageCommand = new Command(() => this.Navigation.PushAsync(new InfoPage(), false));

        this.OpenSettingsPageCommand = new Command(() => this.Navigation.PushAsync(new SettingsPage(), false));
    }
}
