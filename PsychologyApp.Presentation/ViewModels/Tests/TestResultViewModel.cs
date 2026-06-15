using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestResultViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly ITestCatalogService _testCatalogService;
    private readonly TestRetakeOperations _retakeOperations;
    private readonly TechniqueId? _recommendedTechnique;
    private readonly string? _analyzerId;
    private readonly string? _testId;

    public string Interpretation { get; }
    public string? InterpretationDetail { get; }
    public bool HasInterpretationDetail => !string.IsNullOrWhiteSpace(InterpretationDetail);
    public int Score { get; }
    public bool HasRecommendation => _recommendedTechnique is not null;
    public bool CanRetake => !string.IsNullOrWhiteSpace(_testId);

    public ICommand FinishCommand { get; }
    public ICommand TryTechniqueCommand { get; }
    public ICommand RetakeCommand { get; }

    public TestResultViewModel(
        INavigationService navigationService,
        ITestCatalogService testCatalogService,
        TestRetakeOperations retakeOperations,
        TestResultInfo result)
    {
        _navigationService = navigationService;
        _testCatalogService = testCatalogService;
        _retakeOperations = retakeOperations;
        _recommendedTechnique = result.RecommendedTechnique;
        _analyzerId = result.AnalyzerId;
        _testId = result.TestId;
        Score = result.Score;
        Interpretation = result.Interpretation;
        InterpretationDetail = result.InterpretationDetail;

        RefreshRecommendationCopy();
        BindNavigation(navigationService);

        FinishCommand = new AsyncCommand(FinishAsync);
        TryTechniqueCommand = new AsyncCommand(TryTechniqueAsync);
        RetakeCommand = new AsyncCommand(RetakeAsync);
    }
}
