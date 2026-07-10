using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand ReloadCommand { get; private set; } = default!;
    public ICommand ReloadQuotesCommand { get; private set; } = default!;
    public ICommand CancelQuotesCommand { get; private set; } = default!;
    public ICommand OpenTestsListCommand { get; private set; } = default!;
    public ICommand OpenQuotesTabCommand { get; private set; } = default!;
    public ICommand OpenQuotesFavoritesCommand { get; private set; } = default!;

    private void WireCommands(INavigationService navigationService)
    {
        OpenOptionsCommand = new AsyncCommand(() => navigationService.GoToOptionsAsync());
        ReloadCommand = new AsyncCommand(() => RefreshAsync(forceQuotesReload: true));
        ReloadQuotesCommand = new AsyncCommand(() => ReloadQuotesAsync());
        CancelQuotesCommand = new Command(CancelQuotesLoading);
        OpenTestsListCommand = new AsyncCommand(() => _navigationService.GoToTestsTabAsync());
        OpenQuotesTabCommand = new AsyncCommand(() => _navigationService.GoToQuotesTabAsync());
        OpenQuotesFavoritesCommand = new AsyncCommand(() => _navigationService.GoToQuotesFavoritesAsync());
    }
}
