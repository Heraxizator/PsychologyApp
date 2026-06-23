using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.TechniqueCreated;

public partial class CreatedViewModel
{
    private Task ToEditAsync() =>
        _navigationService.GoToDesignerAsync(_techniqueId);

    private async Task ToRemoveAsync()
    {
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            bool removed = await _sessionOperations.RemoveAsync(
                _techniqueId,
                _techniqueService,
                _techniqueMessenger,
                _dialogService,
                timeoutSource.Token);

            if (removed)
            {
                await GoToRootAsync();
            }
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Failed to remove custom technique.");
        }
    }

    private async Task InitAsync()
    {
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            string[] actions = await _sessionOperations.LoadAlgorithmLinesAsync(
                _techniqueId,
                _techniqueService,
                timeoutSource.Token);

            await UiThread.RunAsync(() =>
            {
                Algorithm.Clear();
                foreach (string action in actions)
                {
                    Algorithm.Add(action);
                }

                SetDone();
            });
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Failed to load custom technique.");
        }
    }

    private async Task CompleteSessionAsync()
    {
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            await _sessionOperations.MarkCompletedAsync(
                _techniqueId,
                _techniqueService,
                timeoutSource.Token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to mark custom technique as completed.");
        }

        await _sessionCompletionService.CompleteStandardSessionAsync(
            _userProgressService,
            _navigationService,
            _dialogService,
            $"custom_{_techniqueId}",
            ModuleName,
            PageName,
            _sessionStartedAt,
            deleteDraft: false);
    }
}
