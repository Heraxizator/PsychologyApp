using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record TestHistoryLoadResult(
    IReadOnlyList<TestHistoryEntryItem> Entries,
    IReadOnlyList<TestScoreChartPoint> ChartPoints,
    int ChartDomainMin,
    int ChartDomainMax,
    string ChartSubtitle,
    string Title);

public sealed class TestHistoryLoader(
    TestTrendResolver trendResolver,
    QuestionnaireDetailReader detailReader,
    LuscherDetailReader luscherDetailReader)
{
    public async Task<TestHistoryLoadResult> LoadEntriesAsync(
        string testId,
        string fallbackTitle,
        IUserProgressService userProgressService,
        ITestCatalogService testCatalogService,
        CancellationToken cancellationToken = default)
    {
        TestDefinition? definition = await testCatalogService.GetByIdAsync(testId, cancellationToken);
        string title = definition?.Title ?? fallbackTitle;
        ScoreDirection direction = definition?.ScoreDirection ?? ScoreDirection.LowerIsBetter;
        TestKind kind = definition?.Kind ?? TestKind.Questionnaire;

        IReadOnlyList<TestResultDTO> history =
            await userProgressService.GetTestResultHistoryAsync(testId, 50, cancellationToken);

        List<TestHistoryEntryItem> entries = [];
        for (int i = 0; i < history.Count; i++)
        {
            TestResultDTO item = history[i];
            TestResultDTO? older = i + 1 < history.Count ? history[i + 1] : null;
            TestTrendKind trend = older is null
                ? TestTrendKind.None
                : trendResolver.Compare(item.Score, older.Score, direction);

            QuestionnaireResultDetail? detail = kind == TestKind.Questionnaire
                ? detailReader.TryParse(item.DetailJson)
                : null;
            LuscherStandardResultDetail? standardDetail = kind == TestKind.LuscherStandard
                ? luscherDetailReader.TryParseStandard(item.DetailJson)
                : null;
            LuscherBriefResultDetail? briefDetail = kind == TestKind.LuscherBrief
                ? luscherDetailReader.TryParseBrief(item.DetailJson)
                : null;

            entries.Add(new TestHistoryEntryItem
            {
                DateText = item.CompletedAt.ToLocalTime().ToString("g"),
                SummaryText = item.Summary,
                ScoreText = item.Score is int score ? AppStrings.TestHistoryScore(score) : string.Empty,
                TrendText = TestTrendComparer.ToLabel(trend),
                TrendKind = trend,
                Detail = detail,
                LuscherStandardDetail = standardDetail,
                LuscherBriefDetail = briefDetail,
                LuscherFirstPassTitle = AppStrings.TestsLuscherHistoryFirstPass,
                LuscherFirstPassText = FormatColorPass(standardDetail?.FirstPassColors),
                LuscherSecondPassTitle = AppStrings.TestsLuscherHistorySecondPass,
                LuscherSecondPassText = FormatColorPass(standardDetail?.Colors),
                LuscherBkText = standardDetail is null ? string.Empty : AppStrings.TestsLuscherHistoryBk(standardDetail.Bk),
                LuscherBriefFirstTitle = AppStrings.TestsFirstColor,
                LuscherBriefFirstText = FormatBriefColor(briefDetail?.First),
                LuscherBriefSecondTitle = AppStrings.TestsSecondColor,
                LuscherBriefSecondText = FormatBriefColor(briefDetail?.Second),
                DurationText = detail is null ? string.Empty : AppStrings.TestResultDuration(detail.DurationSeconds)
            });
        }

        IReadOnlyList<TestScoreChartPoint> chartPoints = trendResolver.BuildChartPoints(history);
        IReadOnlyList<int> chartValues = chartPoints.Select(point => point.Score).ToList();
        (int chartDomainMin, int chartDomainMax) =
            TestScoreChartRangeResolver.ResolveDomain(definition?.AnalyzerId ?? testId, chartValues);
        string chartSubtitle = AppStrings.ResolveChartSubtitle(chartPoints.Count);
        return new TestHistoryLoadResult(entries, chartPoints, chartDomainMin, chartDomainMax, chartSubtitle, title);
    }

    private static string FormatColorPass(IReadOnlyList<LuscherStandardColorDetail>? colors) =>
        colors is null || colors.Count == 0
            ? string.Empty
            : string.Join(" → ", colors.Select(color => color.Name ?? color.Code));

    private static string FormatBriefColor(LuscherBriefColorDetail? color)
    {
        if (color is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(color.Text))
        {
            return color.Name ?? string.Empty;
        }

        return $"{color.Name}: {color.Text}";
    }
}
