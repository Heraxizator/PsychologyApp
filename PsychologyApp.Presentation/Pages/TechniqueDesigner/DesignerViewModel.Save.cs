using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.TechniqueDesigner;

public partial class DesignerViewModel
{
    private async Task ExecuteOperationAsync()
    {
        if (_techniqueId <= 0)
        {
            await ToAddTechniqueAsync();
        }
        else
        {
            await ToChangeTechniqueAsync();
        }
    }

    private async Task ToChangeTechniqueAsync()
    {
        try
        {
            TechniqueDTO item = _techniqueOperations.BuildDto(_techniqueId, CaptureForm());

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            await _techniqueOperations.UpdateAsync(
                item,
                _techniqueService,
                _techniqueMessenger,
                timeoutSource.Token);

            await GoToRootAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update technique.");
        }
    }

    private async Task ToAddTechniqueAsync()
    {
        try
        {
            TechniqueDTO technique = _techniqueOperations.BuildDto(-1, CaptureForm());

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            await _techniqueOperations.AddAsync(
                technique,
                _techniqueService,
                _techniqueMessenger,
                timeoutSource.Token);

            await GoBackAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add technique.");
        }
    }
}
