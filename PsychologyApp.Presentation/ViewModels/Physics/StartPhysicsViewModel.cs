using PsychologyApp.Presentation.Common;
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

    public StartPhysicsViewModel(INavigationService navigationService)
    {
        ModuleName = AppStrings.PhysicsTitle;
        PageName = AppStrings.PhysicsIntroPage;

        BindNavigation(navigationService);
        StartCommand = new AsyncCommand(() => navigationService.GoToPhysicsSearchAsync());
        SetDone();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ExplanationHeader),
            nameof(ExplanationBody),
            nameof(DescriptionHeader),
            nameof(DescriptionBody),
            nameof(AlgorithmHeader),
            nameof(StartButtonText),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText),
            nameof(AlgorithmSteps));
    }

    public StartPhysicsViewModel() { }
}
