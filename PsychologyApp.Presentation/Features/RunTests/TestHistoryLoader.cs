using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record TestHistoryLoadResult(
    IReadOnlyList<TestHistoryEntryItem> Entries,
    IReadOnlyList<TestScoreChartPoint> ChartPoints,
    string Title);

public sealed class TestHistoryLoader(
    TestTrendResolver trendResolver,
    QuestionnaireDetailReader detailReader)
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

            QuestionnaireResultDetail? detail = detailReader.TryParse(item.DetailJson);

            entries.Add(new TestHistoryEntryItem
            {
                DateText = item.CompletedAt.ToLocalTime().ToString("g"),
                SummaryText = item.Summary,
                ScoreText = item.Score is int score ? AppStrings.TestHistoryScore(score) : string.Empty,
                TrendText = TestTrendComparer.ToLabel(trend),
                TrendKind = trend,
                Detail = detail,
                DurationText = detail is null ? string.Empty : AppStrings.TestResultDuration(detail.DurationSeconds)
            });
        }

        IReadOnlyList<TestScoreChartPoint> chartPoints = trendResolver.BuildChartPoints(history);
        return new TestHistoryLoadResult(entries, chartPoints, title);
    }
}
