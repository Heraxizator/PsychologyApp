using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Onboarding;

public class OnboardingViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly Func<TechniqueId?, Task> _onCompleted;

    public string WelcomeTitle => AppStrings.OnboardingWelcomeTitle;
    public string WelcomeBody => AppStrings.OnboardingWelcomeBody;
    public string ConcernTitle => AppStrings.OnboardingConcernTitle;
    public string DisclaimerTitle => AppStrings.OnboardingDisclaimerTitle;
    public string DisclaimerBody => AppStrings.OnboardingDisclaimerBody;
    public string StartLabel => AppStrings.OnboardingStart;
    public string SkipLabel => AppStrings.OnboardingSkip;
    public string NextLabel => AppStrings.OnboardingNext;
    public string ConcernAnxiety => AppStrings.OnboardingConcernAnxiety;
    public string ConcernBody => AppStrings.OnboardingConcernBody;
    public string ConcernMood => AppStrings.OnboardingConcernMood;
    public string ConcernExplore => AppStrings.OnboardingConcernExplore;

    public ICommand NextCommand { get; }
    public ICommand SkipCommand { get; }
    public ICommand SelectAnxietyCommand { get; }
    public ICommand SelectBodyCommand { get; }
    public ICommand SelectMoodCommand { get; }
    public ICommand SelectExploreCommand { get; }
    public ICommand StartPracticeCommand { get; }

    private int _step;
    public int Step
    {
        get => _step;
        set
        {
            if (SetProperty(ref _step, value))
            {
                OnPropertyChanged(nameof(IsWelcomeStep));
                OnPropertyChanged(nameof(IsConcernStep));
                OnPropertyChanged(nameof(IsDisclaimerStep));
            }
        }
    }

    public bool IsWelcomeStep => Step == 0;
    public bool IsConcernStep => Step == 1;
    public bool IsDisclaimerStep => Step == 2;

    private string _selectedConcern = "explore";
    public string SelectedConcern
    {
        get => _selectedConcern;
        set => SetProperty(ref _selectedConcern, value);
    }

    public OnboardingViewModel(INavigation navigation, INavigationService navigationService, Func<TechniqueId?, Task> onCompleted)
    {
        _onCompleted = onCompleted;
        BindNavigation(navigation, navigationService);
        _navigationService = navigationService;

        NextCommand = new Command(() => Step++);
        SkipCommand = new AsyncCommand(CompleteWithoutPracticeAsync);
        SelectAnxietyCommand = new Command(() => SelectedConcern = "anxiety");
        SelectBodyCommand = new Command(() => SelectedConcern = "body");
        SelectMoodCommand = new Command(() => SelectedConcern = "mood");
        SelectExploreCommand = new Command(() => SelectedConcern = "explore");
        StartPracticeCommand = new AsyncCommand(StartPracticeAsync);
        UserPreferences.Changed += OnPreferencesChanged;
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(WelcomeTitle));
        OnPropertyChanged(nameof(WelcomeBody));
        OnPropertyChanged(nameof(ConcernTitle));
        OnPropertyChanged(nameof(DisclaimerTitle));
        OnPropertyChanged(nameof(DisclaimerBody));
        OnPropertyChanged(nameof(StartLabel));
        OnPropertyChanged(nameof(SkipLabel));
        OnPropertyChanged(nameof(NextLabel));
        OnPropertyChanged(nameof(ConcernAnxiety));
        OnPropertyChanged(nameof(ConcernBody));
        OnPropertyChanged(nameof(ConcernMood));
        OnPropertyChanged(nameof(ConcernExplore));
    }

    private async Task StartPracticeAsync()
    {
        UserPreferences.CompleteOnboarding(SelectedConcern);
        TechniqueId techniqueId = OnboardingRecommendation.ResolveTechnique(SelectedConcern);
        await _onCompleted(techniqueId);
    }

    private async Task CompleteWithoutPracticeAsync()
    {
        UserPreferences.CompleteOnboarding(SelectedConcern);
        await _onCompleted(null);
    }
}
