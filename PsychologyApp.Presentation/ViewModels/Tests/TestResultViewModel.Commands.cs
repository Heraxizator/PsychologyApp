using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestResultViewModel
{
    private Task FinishAsync() => _navigationService.GoToRootAsync();

    private async Task TryTechniqueAsync()
    {
        if (_recommendedTechnique is not TechniqueId techniqueId)
        {
            return;
        }

        await _navigationService.GoToTechniqueAsync(techniqueId);
    }

    private Task RetakeAsync()
    {
        if (string.IsNullOrWhiteSpace(_testId))
        {
            return Task.CompletedTask;
        }

        return _retakeOperations.RetakeAsync(_testId, _testCatalogService, _navigationService);
    }
}
