using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class UserViewModel
{
    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand OpenSettingsCommand { get; private set; } = default!;
    public ICommand OpenDonateCommand { get; private set; } = default!;
    public ICommand OpenFeedbackCommand { get; private set; } = default!;
    public ICommand OpenInfoCommand { get; private set; } = default!;
    public ICommand ReloadQuotesCommand { get; private set; } = default!;
    public ICommand CancelQuotesCommand { get; private set; } = default!;
    public ICommand OpenTestsListCommand { get; private set; } = default!;
    public ICommand OpenQuotesTabCommand { get; private set; } = default!;

    private void WireCommands(INavigationService navigationService)
    {
        OpenOptionsCommand = new AsyncCommand(() => navigationService.GoToOptionsAsync());
        OpenSettingsCommand = new AsyncCommand(() => navigationService.GoToSettingsAsync());
        OpenDonateCommand = new AsyncCommand(() => navigationService.GoToDonateAsync());
        OpenFeedbackCommand = new AsyncCommand(() => navigationService.GoToFormAsync());
        OpenInfoCommand = new AsyncCommand(() => navigationService.GoToInfoAsync());
        ReloadQuotesCommand = new AsyncCommand(() => ReloadQuotesAsync());
        CancelQuotesCommand = new Command(CancelQuotesLoading);
        OpenTestsListCommand = new AsyncCommand(() => _navigationService.GoToTestsTabAsync());
        OpenQuotesTabCommand = new AsyncCommand(() => _navigationService.GoToQuotesTabAsync());
    }
}
