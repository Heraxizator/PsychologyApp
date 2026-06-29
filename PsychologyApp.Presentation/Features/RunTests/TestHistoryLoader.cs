using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record TestHistoryLoadResult(
    IReadOnlyList<TestHistoryEntryItem> Entries,
    string Title);

public sealed class TestHistoryLoader
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

        IReadOnlyList<TestResultDTO> history =
            await userProgressService.GetTestResultHistoryAsync(testId, 50, cancellationToken);

        List<TestHistoryEntryItem> entries = [];
        for (int i = 0; i < history.Count; i++)
        {
            TestResultDTO item = history[i];
            TestResultDTO? older = i + 1 < history.Count ? history[i + 1] : null;
            TestTrendKind trend = older is null
                ? TestTrendKind.None
                : TestTrendComparer.CompareScores(item.Score, older.Score);

            entries.Add(new TestHistoryEntryItem
            {
                DateText = item.CompletedAt.ToLocalTime().ToString("g"),
                SummaryText = item.Summary,
                ScoreText = item.Score is int score ? AppStrings.TestHistoryScore(score) : string.Empty,
                TrendText = TestTrendComparer.ToLabel(trend),
                TrendKind = trend
            });
        }

        return new TestHistoryLoadResult(entries, title);
    }
}
