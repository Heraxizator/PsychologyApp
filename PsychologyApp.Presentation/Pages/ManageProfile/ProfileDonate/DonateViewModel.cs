using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;

public sealed class DonateViewModel : BaseViewModel
{
    public string PageTitle => AppStrings.DonateTitle;
    public string MoreInfoHeader => AppStrings.DonateMoreInfo;
    public string MoreInfoBody => AppStrings.DonateBody;
    public string DonateButtonText => AppStrings.DonateButton;

    public DonateViewModel(INavigationService navigationService)
    {
        BindNavigation(navigationService);
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
