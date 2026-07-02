using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultViewModel : BaseViewModel
{
    private readonly TestRetakeOperations _retakeOperations;
    private readonly ITestCatalogService _testCatalogService;
    private readonly TechniqueCatalogGateway _techniqueCatalog;
    private readonly IUserProgressService _userProgressService;
    private readonly TestTrendResolver _trendResolver;
    private readonly TestResultInfo _result;

    public ICommand TryTechniqueCommand { get; }
    public ICommand RetakeCommand { get; }
    public ICommand BackToListCommand { get; }

    public TestResultViewModel() { }

    public TestResultViewModel(
        INavigationService navigationService,
        ITestCatalogService testCatalogService,
        TechniqueCatalogGateway techniqueCatalog,
        TestRetakeOperations retakeOperations,
        IUserProgressService userProgressService,
        TestTrendResolver trendResolver,
        TestResultInfo result)
    {
        _testCatalogService = testCatalogService;
        _techniqueCatalog = techniqueCatalog;
        _retakeOperations = retakeOperations;
        _userProgressService = userProgressService;
        _trendResolver = trendResolver;
        _result = result;

        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = AppStrings.TestsResultPageTitle;

        BindNavigation(navigationService);

        TryTechniqueCommand = new AsyncCommand(TryTechniqueAsync, () => _result.RecommendedTechnique is not null);
        RetakeCommand = new AsyncCommand(RetakeAsync, () => !string.IsNullOrWhiteSpace(_result.TestId));
        BackToListCommand = new AsyncCommand(GoToRootAsync);

        ApplyResult();
        LoadTrendAsync().FireAndForget();
    }

    public string ScoreTitle { get; private set; } = string.Empty;
    public string Interpretation { get; private set; } = string.Empty;
    public string InterpretationDetail { get; private set; } = string.Empty;
    public bool HasInterpretationDetail { get; private set; }
    public bool HasRecommendation { get; private set; }
    public string TrendText { get; private set; } = string.Empty;
    public TestTrendKind TrendKind { get; private set; } = TestTrendKind.None;
    public bool HasTrendBadge { get; private set; }
    public IReadOnlyList<TestScoreChartPoint> ChartPoints { get; private set; } = [];
    public bool HasTrendChart { get; private set; }
    public string ChartTitle => AppStrings.TestHistoryTrendTitle;
    public string TestFlowId => _result.TestId ?? string.Empty;

    private void ApplyResult()
    {
        ScoreTitle = AppStrings.TestsResultTitle(_result.Score);
        Interpretation = _result.Interpretation;
        InterpretationDetail = _result.InterpretationDetail ?? string.Empty;
        HasInterpretationDetail = !string.IsNullOrWhiteSpace(_result.InterpretationDetail);
        HasRecommendation = _result.RecommendedTechnique is not null;
        ApplyAnswerDetail(_result.Detail);
        RefreshRecommendationCopy();

        Notify(
            nameof(ScoreTitle),
            nameof(Interpretation),
            nameof(InterpretationDetail),
            nameof(HasInterpretationDetail),
            nameof(HasRecommendation));
    }

    private async Task LoadTrendAsync()
    {
        if (string.IsNullOrWhiteSpace(_result.TestId))
        {
            return;
        }

        try
        {
            IReadOnlyList<TestResultDTO> history =
                await _userProgressService.GetTestResultHistoryAsync(_result.TestId, 50);

            TestTrendSnapshot? snapshot = null;
            if (history.Count >= 2)
            {
                ScoreDirection direction = await _trendResolver.ResolveDirectionAsync(_result.TestId);
                TestTrendKind kind = _trendResolver.Compare(history[0].Score, history[1].Score, direction);
                snapshot = new TestTrendSnapshot(kind, TestTrendComparer.ToLabel(kind), kind is not TestTrendKind.None);
            }

            IReadOnlyList<TestScoreChartPoint> chartPoints = _trendResolver.BuildChartPoints(history);

            await UiThread.RunAsync(() =>
            {
                if (snapshot is not null)
                {
                    TrendKind = snapshot.Kind;
                    TrendText = snapshot.Label;
                    HasTrendBadge = snapshot.HasBadge;
                }

                ChartPoints = chartPoints;
                HasTrendChart = chartPoints.Count >= 2;
                Notify(
                    nameof(TrendKind),
                    nameof(TrendText),
                    nameof(HasTrendBadge),
                    nameof(ChartPoints),
                    nameof(HasTrendChart),
                    nameof(ChartTitle));
            });
        }
        catch
        {
            // Trend/chart are optional; result screen must stay usable if history load fails.
        }
    }
}
