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
    private readonly string? _testId;

    public string PageTitle => AppStrings.TestsResultPageTitle;
    public string ScoreTitle => AppStrings.TestsResultTitle(Score);
    public string Interpretation { get; }
    public int Score { get; }
    public bool HasRecommendation => _recommendedTechnique is not null;
    public bool CanRetake => !string.IsNullOrWhiteSpace(_testId);
    public string RecommendationHint => AppStrings.TestsResultRecommendationHint;
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
        _testId = result.TestId;
        Score = result.Score;
        Interpretation = result.Interpretation;

        BindNavigation(navigation, navigationService);

        FinishCommand = new AsyncCommand(FinishAsync);
        TryTechniqueCommand = new AsyncCommand(TryTechniqueAsync);
        RetakeCommand = new AsyncCommand(RetakeAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ScoreTitle),
            nameof(RecommendationHint),
            nameof(FinishButtonText),
            nameof(TryTechniqueButtonText),
            nameof(RetakeButtonText));
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
        item.Action.Invoke();
    }
}
