using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class TestRetakeOperations(TestRunCoordinator testRunCoordinator)
{
    public Task RetakeAsync(
        string testId,
        ITestCatalogService testCatalogService,
        INavigationService navigationService,
        CancellationToken cancellationToken = default) =>
        testRunCoordinator.RetakeAsync(testId, testCatalogService, navigationService, cancellationToken);
}
