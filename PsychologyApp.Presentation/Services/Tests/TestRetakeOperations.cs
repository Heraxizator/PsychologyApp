using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Services.Tests;

public sealed class TestRetakeOperations
{
    public async Task RetakeAsync(
        string testId,
        ITestCatalogService testCatalogService,
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        TestDefinition? definition = await testCatalogService.GetByIdAsync(testId, cancellationToken);
        if (definition is null)
        {
            return;
        }

        TestItem item = TestItemFactory.Create(definition, navigationService);
        await navigationService.GoToRootAsync();
        await item.StartAsync();
    }
}
