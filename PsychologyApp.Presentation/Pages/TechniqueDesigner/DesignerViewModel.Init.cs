using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.TechniqueDesigner;

public partial class DesignerViewModel
{
    private async Task FillAsync(CancellationToken cancellationToken)
    {
        try
        {
            DesignerTechniqueForm? form = await _techniqueOperations.LoadFormAsync(
                _techniqueId,
                _techniqueService,
                cancellationToken);

            if (form is null)
            {
                return;
            }

            await UiThread.RunAsync(() => ApplyForm(form));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load technique for designer.");
        }
    }

    private async Task InitAsync()
    {
        using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
        await FillAsync(timeoutSource.Token);
    }
}
