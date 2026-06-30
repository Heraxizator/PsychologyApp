using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class TestsListLoader(
    IUserProgressService userProgressService,
    ITestCatalogService testCatalogService,
    TestRunCoordinator testRunCoordinator)
{
    public async Task<IReadOnlyList<TestItem>> LoadItemsAsync(
        INavigationService navigationService,
        Func<TestItem, Task> handleSelectionAsync,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TestDefinition> definitions = await testCatalogService.GetCatalogAsync(cancellationToken);
        List<string> testIds = definitions
            .Select(d => d.TestId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        IReadOnlyDictionary<string, TestResultDTO> latestByTestId =
            await userProgressService.GetLatestTestResultsAsync(testIds, cancellationToken);
        IReadOnlyDictionary<string, int> countsByTestId =
            await userProgressService.GetTestResultCountsAsync(testIds, cancellationToken);

        List<TestItem> items = [];

        foreach (TestDefinition definition in definitions)
        {
            if (string.IsNullOrWhiteSpace(definition.TestId))
            {
                continue;
            }

            TestItem item = TestItemFactory.Create(definition, navigationService, testRunCoordinator);

            if (latestByTestId.TryGetValue(item.TestId, out TestResultDTO? latest))
            {
                item.LastResultSummary = AppStrings.TestLastResult(latest.Summary);
            }

            item.HasMultipleResults = countsByTestId.TryGetValue(item.TestId, out int count) && count > 1;

            TestItem selected = item;
            item.TapCommand = new AsyncCommand(() => handleSelectionAsync(selected));
            item.OpenHistoryCommand = new AsyncCommand(() =>
                navigationService.GoToTestHistoryAsync(selected.TestId, selected.Title));

            items.Add(item);
        }

        return items;
    }
}
