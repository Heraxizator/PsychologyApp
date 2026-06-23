using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesStore _userPreferencesStore;
    private readonly Func<TechniqueId?, Task> _onCompleted;

    public ICommand NextCommand { get; }
    public ICommand SkipCommand { get; }
    public ICommand SelectAnxietyCommand { get; }
    public ICommand SelectBodyCommand { get; }
    public ICommand SelectMoodCommand { get; }
    public ICommand SelectExploreCommand { get; }
    public ICommand StartPracticeCommand { get; }

    public OnboardingViewModel(
        INavigationService navigationService,
        IUserPreferencesStore userPreferencesStore,
        Func<TechniqueId?, Task> onCompleted)
    {
        BindPreferences(userPreferencesStore);
        _onCompleted = onCompleted;
        BindNavigation(navigationService);
        _navigationService = navigationService;
        _userPreferencesStore = userPreferencesStore;

        NextCommand = new Command(() => Step++);
        SkipCommand = new AsyncCommand(CompleteWithoutPracticeAsync);
        SelectAnxietyCommand = new Command(() => SelectedConcern = OnboardingConcernKeys.Anxiety);
        SelectBodyCommand = new Command(() => SelectedConcern = OnboardingConcernKeys.Body);
        SelectMoodCommand = new Command(() => SelectedConcern = OnboardingConcernKeys.Mood);
        SelectExploreCommand = new Command(() => SelectedConcern = OnboardingConcernKeys.Explore);
        StartPracticeCommand = new AsyncCommand(StartPracticeAsync);
    }
}
