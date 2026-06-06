using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public class OptionsViewModel : BaseViewModel
{
    public ICommand OpenAboutPageCommand { get; private set; } = default!;
    public ICommand OpenDonatePageCommand { get; private set; } = default!;
    public ICommand OpenFeedbackPageCommand { get; private set; } = default!;
    public ICommand OpenSettingsPageCommand { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;

    public OptionsViewModel(INavigation navigation, INavigationService navigationService)
    {
        BindNavigation(navigation);

        OpenAboutPageCommand = new AsyncCommand(() => navigationService.GoToInfoAsync());
        OpenDonatePageCommand = new AsyncCommand(() => navigationService.GoToDonateAsync());
        OpenFeedbackPageCommand = new AsyncCommand(() => navigationService.GoToFormAsync());
        OpenSettingsPageCommand = new AsyncCommand(() => navigationService.GoToSettingsAsync());
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
    }
}
