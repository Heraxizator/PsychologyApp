using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;

public sealed class DonateViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;

    public string PageTitle => AppStrings.OptionsDonateTitle;
    public string MoreInfoHeader => AppStrings.DonateMoreInfo;
    public string MoreInfoBody => AppStrings.DonateBody;
    public string DonateButtonText => AppStrings.DonateButton;

    public DonateViewModel(IDialogService dialogService, INavigationService navigationService)
    {
        _dialogService = dialogService;
        BindNavigation(navigationService);
        ModuleName = AppStrings.OptionsTitle;
        PageName = AppStrings.OptionsDonateTitle;
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        DonateCommand = new AsyncCommand(OpenDonatePageAsync);
    }

    public ICommand BackCommand { get; }
    public ICommand DonateCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(MoreInfoHeader), nameof(MoreInfoBody), nameof(DonateButtonText));
    }

    private async Task OpenDonatePageAsync()
    {
        try
        {
            await Browser.Default.OpenAsync(Constants.DonateUrl);
        }
        catch (Exception)
        {
            await _dialogService.ShowAsync(null, AppStrings.DonateOpenFailed);
        }
    }
}
