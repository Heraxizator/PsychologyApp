using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public ICommand ConstructorTapped { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;
    public ICommand StartTodayPracticeCommand { get; private set; } = default!;
    public ICommand OpenCrisisHubCommand { get; private set; } = default!;
    public ICommand LoadMoreCustomTechniquesCommand { get; private set; } = default!;

    private void WireCommands()
    {
        ConstructorTapped = new AsyncCommand(() => _navigationService.GoToDesignerAsync(-1));
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        StartTodayPracticeCommand = new AsyncCommand(StartTodayPracticeAsync);
        OpenCrisisHubCommand = new AsyncCommand(() => _navigationService.GoToCrisisHubAsync());

        Cancel = new Command(CancelProgress);
        Reload = new AsyncCommand(() => InitializeAsync(showLoadingOverlay: true));
        LoadMoreCustomTechniquesCommand = new AsyncCommand(LoadMoreCustomTechniquesAsync);
    }

    private async Task StartTodayPracticeAsync()
    {
        if (await _clinicalCareService.ShouldRouteToCrisisHubAsync())
        {
            await _navigationService.GoToCrisisHubAsync();
            return;
        }

        await _navigationService.GoToTechniqueAsync(_todayTechniqueId);
    }
}
