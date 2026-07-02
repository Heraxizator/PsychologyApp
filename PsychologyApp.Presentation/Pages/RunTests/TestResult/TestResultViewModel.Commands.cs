using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultViewModel
{
    private Task TryTechniqueAsync()
    {
        if (_result.RecommendedTechnique is not TechniqueId techniqueId)
        {
            return Task.CompletedTask;
        }

        return NavigationService!.GoToTechniqueAsync(techniqueId);
    }

    private Task RetakeAsync() =>
        string.IsNullOrWhiteSpace(_result.TestId)
            ? Task.CompletedTask
            : _retakeOperations.RetakeAsync(_result.TestId, _testCatalogService, NavigationService!);
}
