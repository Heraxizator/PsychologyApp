using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Services.Tests;

public sealed class TestsListLoader(
    IUserProgressService userProgressService,
    ITestCatalogService testCatalogService)
{
    public async Task<IReadOnlyList<TestItem>> LoadItemsAsync(
        INavigationService navigationService,
        Func<TestItem, Task> handleSelectionAsync,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TestDefinition> definitions = await testCatalogService.GetCatalogAsync(cancellationToken);
        List<TestItem> items = [];

        foreach (TestDefinition definition in definitions)
        {
            if (string.IsNullOrWhiteSpace(definition.TestId))
            {
                continue;
            }

            TestItem item = TestItemFactory.Create(definition, navigationService);

            TestResultDTO? latest = await userProgressService.GetLatestTestResultAsync(item.TestId, cancellationToken);
            if (latest is not null)
            {
                item.LastResultSummary = AppStrings.TestLastResult(latest.Summary);
            }

            IReadOnlyList<TestResultDTO> history =
                await userProgressService.GetTestResultHistoryAsync(item.TestId, 2, cancellationToken);
            item.HasMultipleResults = history.Count > 1;

            TestItem selected = item;
            item.TapCommand = new AsyncCommand(() => handleSelectionAsync(selected));
            item.OpenHistoryCommand = new AsyncCommand(() =>
                navigationService.GoToTestHistoryAsync(selected.TestId, selected.Title));

            items.Add(item);
        }

        return items;
    }
}
