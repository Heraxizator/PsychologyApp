using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileAlice;

public sealed class AliceViewModel : BaseViewModel
{
    private bool _isLoading = true;

    private readonly IDialogService _dialogService;

    public AliceViewModel(IDialogService dialogService, INavigationService navigationService)
    {
        _dialogService = dialogService;
        BindNavigation(navigationService);
        ModuleName = AppStrings.OptionsTitle;
        PageName = AppStrings.OptionsAliceTitle;
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        OpenInBrowserCommand = new AsyncCommand(OpenInBrowserAsync);
    }

    public string PageTitle => AppStrings.OptionsAliceTitle;
    public string DisclaimerBody => AppStrings.AliceDisclaimerBody;
    public string OpenInBrowserText => AppStrings.AliceOpenInBrowser;
    public string LoadingText => AppStrings.AliceLoadingText;
    public string AliceUrl => Constants.AliceUrl;

    public bool IsWebViewLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading == value)
            {
                return;
            }

            _isLoading = value;
            OnPropertyChanged(nameof(IsWebViewLoading));
        }
    }

    public ICommand BackCommand { get; }
    public ICommand OpenInBrowserCommand { get; }

    public void SetLoading(bool isLoading) => IsWebViewLoading = isLoading;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(DisclaimerBody),
            nameof(OpenInBrowserText),
            nameof(LoadingText),
            nameof(AliceUrl));
    }

    private async Task OpenInBrowserAsync()
    {
        try
        {
            await Browser.Default.OpenAsync(Constants.AliceUrl);
        }
        catch (Exception)
        {
            await _dialogService.ShowAsync(null, AppStrings.AliceOpenFailed);
        }
    }
}
