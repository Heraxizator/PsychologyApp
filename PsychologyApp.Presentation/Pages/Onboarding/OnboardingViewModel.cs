using PsychologyApp.Presentation.Features.Onboarding;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesStore _userPreferencesStore;
    private readonly OnboardingRecommendationResolver _onboardingRecommendationResolver;
    private readonly Func<TechniqueId?, Task> _onCompleted;

    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand SkipCommand { get; }
    public ICommand SelectAnxietyCommand { get; }
    public ICommand SelectBodyCommand { get; }
    public ICommand SelectMoodCommand { get; }
    public ICommand SelectExploreCommand { get; }
    public ICommand StartPracticeCommand { get; }

    public OnboardingViewModel(
        INavigationService navigationService,
        IUserPreferencesStore userPreferencesStore,
        OnboardingRecommendationResolver onboardingRecommendationResolver,
        Func<TechniqueId?, Task> onCompleted)
    {
        BindPreferences(userPreferencesStore);
        _onCompleted = onCompleted;
        BindNavigation(navigationService);
        _navigationService = navigationService;
        _userPreferencesStore = userPreferencesStore;
        _onboardingRecommendationResolver = onboardingRecommendationResolver;
        RefreshRecommendation();

        NextCommand = new Command(GoNext);
        BackCommand = new Command(GoBack);
        SkipCommand = new AsyncCommand(CompleteWithoutPracticeAsync);
        SelectAnxietyCommand = new Command(() => SelectConcernAndAdvance(OnboardingConcernKeys.Anxiety));
        SelectBodyCommand = new Command(() => SelectConcernAndAdvance(OnboardingConcernKeys.Body));
        SelectMoodCommand = new Command(() => SelectConcernAndAdvance(OnboardingConcernKeys.Mood));
        SelectExploreCommand = new Command(() => SelectConcernAndAdvance(OnboardingConcernKeys.Explore));
        StartPracticeCommand = new AsyncCommand(StartPracticeAsync);
    }

    private void GoNext() => Step = OnboardingStepNavigator.GoNext(Step);

    private void GoBack() => Step = OnboardingStepNavigator.GoBack(Step);

    private void SelectConcernAndAdvance(string concern)
    {
        SelectedConcern = concern;
        Step = OnboardingStepNavigator.FinishStep;
    }
}
