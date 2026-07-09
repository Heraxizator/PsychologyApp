using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;

public partial class DesignerViewModel
{
    private async Task InitAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_techniqueId <= 0)
            {
                await UiThread.RunAsync(() =>
                {
                    SetDone();
                    RaiseSaveCanExecuteChanged();
                });
                return;
            }

            await UiThread.RunAsync(SetInit);

            DesignerTechniqueForm? form = await _techniqueOperations.LoadFormAsync(
                _techniqueId,
                _techniqueService,
                cancellationToken);

            if (form is null)
            {
                await UiThread.RunAsync(() =>
                {
                    SetFail();
                    RaiseSaveCanExecuteChanged();
                });
                _toastService.ShortToast(AppStrings.DesignerLoadError);
                return;
            }

            await UiThread.RunAsync(() =>
            {
                ApplyForm(form);
                SetDone();
                RaiseSaveCanExecuteChanged();
            });
        }
        catch (OperationCanceledException)
        {
            await UiThread.RunAsync(CancelProgress);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load technique for designer.");
            await UiThread.RunAsync(() =>
            {
                SetFail();
                RaiseSaveCanExecuteChanged();
            });
            _toastService.ShortToast(AppStrings.DesignerLoadError);
        }
    }

    private void RaiseSaveCanExecuteChanged() =>
        (ExecuteTechnique as AsyncCommand)?.RaiseCanExecuteChanged();
}
