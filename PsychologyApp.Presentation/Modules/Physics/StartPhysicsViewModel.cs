using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public class StartPhysicsViewModel : BaseViewModel
{
    public IReadOnlyList<string> AlgorithmSteps =>
    [
        AppStrings.PhysicsAlgorithmStep1,
        AppStrings.PhysicsAlgorithmStep2
    ];

    public string PageTitle => AppStrings.PhysicsTitle;
    public string ExplanationHeader => AppStrings.PhysicsExplanationHeader;
    public string ExplanationBody => AppStrings.PhysicsExplanationBody;
    public string DescriptionHeader => AppStrings.PhysicsDescriptionHeader;
    public string DescriptionBody => AppStrings.PhysicsDescriptionBody;
    public string AlgorithmHeader => AppStrings.TechniqueAlgorithm;
    public string StartButtonText => AppStrings.TestsStartButton;
    public string LoadingText => AppStrings.PhysicsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    public ICommand StartCommand { get; private set; } = default!;

    public StartPhysicsViewModel(INavigation navigation, INavigationService navigationService)
    {
        ModuleName = AppStrings.PhysicsTitle;
        PageName = AppStrings.PhysicsIntroPage;

            BindNavigation(navigation);
            StartCommand = new AsyncCommand(() => navigationService.GoToPhysicsSearchAsync());
            UserPreferences.Changed += OnPreferencesChanged;
            SetDone();
        }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(ExplanationHeader));
        OnPropertyChanged(nameof(ExplanationBody));
        OnPropertyChanged(nameof(DescriptionHeader));
        OnPropertyChanged(nameof(DescriptionBody));
        OnPropertyChanged(nameof(AlgorithmHeader));
        OnPropertyChanged(nameof(StartButtonText));
        OnPropertyChanged(nameof(LoadingText));
        OnPropertyChanged(nameof(FailedText));
        OnPropertyChanged(nameof(RetryText));
        OnPropertyChanged(nameof(AlgorithmSteps));
    }

    public StartPhysicsViewModel() { }
}
