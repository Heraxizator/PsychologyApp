using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

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
