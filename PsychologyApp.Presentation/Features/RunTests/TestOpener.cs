using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

/// <summary>
/// Opens one questionnaire by its id, the same intro screen the Tests tab opens, so an article, a course or the companion can point at
/// the right test instead of the whole list.
/// </summary>
public sealed class TestOpener(ITestCatalogService testCatalogService, TestRunCoordinator testRunCoordinator)
{
    public async Task OpenAsync(string testId, INavigationService navigationService)
    {
        TestDefinition? definition = await testCatalogService.GetByIdAsync(testId);
        if (definition is null)
        {
            await navigationService.GoToTestsTabAsync();
            return;
        }

        TestItem item = TestItemFactory.Create(definition, navigationService, testRunCoordinator);
        await navigationService.GoToFindProblemAsync(item.Description, item.Algorithm, item.Comment, item.StartAsync, item.TestId);
    }
}
