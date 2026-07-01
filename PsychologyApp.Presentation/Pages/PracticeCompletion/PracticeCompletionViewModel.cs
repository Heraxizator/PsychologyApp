using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.PracticeCompletion;

public sealed class PracticeCompletionViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    public PracticeCompletionViewModel(INavigationService navigationService, int streakDays)
    {
        _navigationService = navigationService;
        StreakDays = streakDays;
        MorePracticeCommand = new AsyncCommand(MorePracticeAsync);
        GoHomeCommand = new AsyncCommand(() => _navigationService.GoToRootAsync());
    }

    public int StreakDays { get; }

    public string TitleText => AppStrings.PracticeCompletedTitle;
    public string BodyText => AppStrings.PracticeCompletedBody(StreakDays);
    public string StreakValueText => StreakDays > 0 ? AppStrings.ProfileStreakCount(StreakDays) : string.Empty;
    public string StreakLabelText => AppStrings.ProfileStreakDays;
    public bool HasStreak => StreakDays > 0;
    public string MorePracticeText => AppStrings.PracticeMoreButton;
    public string GoHomeText => AppStrings.PracticeGoHomeButton;

    public ICommand MorePracticeCommand { get; }
    public ICommand GoHomeCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(TitleText),
            nameof(BodyText),
            nameof(StreakValueText),
            nameof(StreakLabelText),
            nameof(HasStreak),
            nameof(MorePracticeText),
            nameof(GoHomeText));
    }

    private async Task MorePracticeAsync()
    {
        await _navigationService.GoBackAsync();
        await _navigationService.GoBackAsync();
    }
}
