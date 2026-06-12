using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class TestResultViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly ITestCatalogService _testCatalogService;
    private readonly TechniqueId? _recommendedTechnique;
    private readonly string? _analyzerId;
    private readonly string? _testId;

    public string PageTitle => AppStrings.TestsResultPageTitle;
    public string ScoreTitle => AppStrings.TestsResultTitle(Score);
    public string Interpretation { get; }
    public string? InterpretationDetail { get; }
    public bool HasInterpretationDetail => !string.IsNullOrWhiteSpace(InterpretationDetail);
    public int Score { get; }
    public bool HasRecommendation => _recommendedTechnique is not null;
    public bool CanRetake => !string.IsNullOrWhiteSpace(_testId);
    public string RecommendedTechniqueTitle { get; private set; } = string.Empty;
    public string RecommendationHint { get; private set; } = string.Empty;
    public string FinishButtonText => AppStrings.TestsFinishButton;
    public string TryTechniqueButtonText => AppStrings.TestTryTechnique;
    public string RetakeButtonText => AppStrings.TestsRestart;

    public ICommand FinishCommand { get; }
    public ICommand TryTechniqueCommand { get; }
    public ICommand RetakeCommand { get; }

    public TestResultViewModel(
        INavigation navigation,
        INavigationService navigationService,
        ITestCatalogService testCatalogService,
        TestResultInfo result)
    {
        _navigationService = navigationService;
        _testCatalogService = testCatalogService;
        _recommendedTechnique = result.RecommendedTechnique;
        _analyzerId = result.AnalyzerId;
        _testId = result.TestId;
        Score = result.Score;
        Interpretation = result.Interpretation;
        InterpretationDetail = result.InterpretationDetail;

        RefreshRecommendationCopy();

        BindNavigation(navigation, navigationService);

        FinishCommand = new AsyncCommand(FinishAsync);
        TryTechniqueCommand = new AsyncCommand(TryTechniqueAsync);
        RetakeCommand = new AsyncCommand(RetakeAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        RefreshRecommendationCopy();
        Notify(
            nameof(PageTitle),
            nameof(ScoreTitle),
            nameof(FinishButtonText),
            nameof(TryTechniqueButtonText),
            nameof(RetakeButtonText),
            nameof(RecommendedTechniqueTitle),
            nameof(RecommendationHint));
    }

    private void RefreshRecommendationCopy()
    {
        if (_recommendedTechnique is not TechniqueId techniqueId)
        {
            RecommendedTechniqueTitle = string.Empty;
            RecommendationHint = string.Empty;
            return;
        }

        string title = TechniqueCatalog.Get(techniqueId).ListTitle;
        RecommendedTechniqueTitle = AppStrings.TestRecommendationFor(title);
        string? reason = TestScoreAnalyzers.ResolveRecommendationReason(_analyzerId, Score);
        RecommendationHint = string.IsNullOrWhiteSpace(reason)
            ? AppStrings.TestsResultRecommendationHint
            : AppStrings.TestRecommendationReason(reason);
    }

    private Task FinishAsync() => _navigationService.GoToRootAsync();

    private async Task TryTechniqueAsync()
    {
        if (_recommendedTechnique is not TechniqueId techniqueId)
        {
            return;
        }

        await _navigationService.GoToTechniqueAsync(techniqueId);
    }

    private async Task RetakeAsync()
    {
        if (string.IsNullOrWhiteSpace(_testId))
        {
            return;
        }

        TestDefinition? definition = await _testCatalogService.GetByIdAsync(_testId);
        if (definition is null)
        {
            return;
        }

        TestItem item = TestItemFactory.Create(definition, _navigationService);
        await _navigationService.GoToRootAsync();
        await item.StartAsync();
    }
}
