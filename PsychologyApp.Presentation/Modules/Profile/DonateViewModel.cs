using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Profile;

public sealed class DonateViewModel : BaseViewModel
{
    public DonateViewModel(INavigation navigation, INavigationService navigationService)
    {
        BindNavigation(navigation);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        DonateCommand = new AsyncCommand(OpenDonatePageAsync);
    }

    public ICommand BackCommand { get; }
    public ICommand DonateCommand { get; }

    private static Task OpenDonatePageAsync() =>
        Browser.Default.OpenAsync("https://yoomoney.ru/fundraise/17UP5E1QFCU.250123");
}
