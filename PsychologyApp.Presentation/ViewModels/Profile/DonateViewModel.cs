using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public sealed class DonateViewModel : BaseViewModel
{
    public string PageTitle => AppStrings.DonateTitle;
    public string MoreInfoHeader => AppStrings.DonateMoreInfo;
    public string MoreInfoBody => AppStrings.DonateBody;
    public string DonateButtonText => AppStrings.DonateButton;

    public DonateViewModel(INavigation navigation, INavigationService navigationService)
    {
        BindNavigation(navigation, navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        DonateCommand = new AsyncCommand(OpenDonatePageAsync);
    }

    public ICommand BackCommand { get; }
    public ICommand DonateCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(MoreInfoHeader), nameof(MoreInfoBody), nameof(DonateButtonText));
    }

    private static Task OpenDonatePageAsync() =>
        Browser.Default.OpenAsync("https://yoomoney.ru/fundraise/17UP5E1QFCU.250123");
}
