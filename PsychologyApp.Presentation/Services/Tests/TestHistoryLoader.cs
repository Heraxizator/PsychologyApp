using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;

namespace PsychologyApp.Presentation.Services.Tests;

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
        foreach (TestResultDTO item in history)
        {
            entries.Add(new TestHistoryEntryItem
            {
                DateText = item.CompletedAt.ToLocalTime().ToString("g"),
                SummaryText = item.Summary
            });
        }

        return new TestHistoryLoadResult(entries, title);
    }
}
