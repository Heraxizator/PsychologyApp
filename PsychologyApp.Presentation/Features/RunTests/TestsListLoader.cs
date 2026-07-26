using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class TestsListLoader(
    IUserProgressService userProgressService,
    ITestCatalogService testCatalogService,
    TestRunCoordinator testRunCoordinator)
{
    public async Task<TestsListLoadResult> LoadItemsAsync(
        INavigationService navigationService,
        Func<TestItem, Task> handleSelectionAsync,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TestItem> items = await LoadCatalogItemsAsync(
            navigationService,
            handleSelectionAsync,
            cancellationToken);

        if (items.Count <= CatalogListPolicy.EagerProgressItemThreshold)
        {
            await EnrichProgressAsync(items, cancellationToken);
            return new TestsListLoadResult(items, ProgressDeferred: false);
        }

        return new TestsListLoadResult(items, ProgressDeferred: true);
    }

    public async Task EnrichProgressAsync(
        IReadOnlyList<TestItem> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
        {
            return;
        }

        List<string> testIds = items
            .Select(item => item.TestId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        IReadOnlyDictionary<string, TestResultDTO> latestByTestId =
            await userProgressService.GetLatestTestResultsAsync(testIds, cancellationToken);
        IReadOnlyDictionary<string, int> countsByTestId =
            await userProgressService.GetTestResultCountsAsync(testIds, cancellationToken);

        foreach (TestItem item in items)
        {
            if (latestByTestId.TryGetValue(item.TestId, out TestResultDTO? latest))
            {
                string summary = ResolveSummary(item.AnalyzerId, latest);
                string date = latest.CompletedAt.ToLocalTime().ToString("d");
                item.LastResultSummary = AppStrings.TestLastResultDated(summary, date);
            }
            else
            {
                item.LastResultSummary = null;
            }

            item.HasMultipleResults = countsByTestId.TryGetValue(item.TestId, out int count) && count > 1;
        }
    }

    private static string ResolveSummary(string? analyzerId, TestResultDTO latest)
    {
        if (latest.Score is int score)
        {
            string? mapped = TestScoreLabelMapper.GetSummary(analyzerId, score);
            if (!string.IsNullOrWhiteSpace(mapped))
            {
                return mapped;
            }
        }

        return string.IsNullOrWhiteSpace(latest.Summary) ? "—" : latest.Summary;
    }

    private async Task<IReadOnlyList<TestItem>> LoadCatalogItemsAsync(
        INavigationService navigationService,
        Func<TestItem, Task> handleSelectionAsync,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TestDefinition> definitions = await testCatalogService.GetCatalogAsync(cancellationToken);
        List<TestItem> items = [];

        foreach (TestDefinition definition in definitions)
        {
            if (string.IsNullOrWhiteSpace(definition.TestId))
            {
                continue;
            }

            TestItem item = TestItemFactory.Create(definition, navigationService, testRunCoordinator);
            TestItem selected = item;
            item.TapCommand = new AsyncCommand(() => handleSelectionAsync(selected));
            item.OpenHistoryCommand = new AsyncCommand(() =>
                navigationService.GoToTestHistoryAsync(selected.TestId, selected.Title));
            item.RetakeCommand = new AsyncCommand(() =>
                testRunCoordinator.RetakeAsync(selected.TestId, testCatalogService, navigationService));

            items.Add(item);
        }

        return items;
    }
}
