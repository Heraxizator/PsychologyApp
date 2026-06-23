using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class CustomTechniqueSessionOperations
{
    public async Task<string[]> LoadAlgorithmLinesAsync(
        long techniqueId,
        ITechniqueService techniqueService,
        CancellationToken cancellationToken)
    {
        TechniqueDTO techniqueDto = await techniqueService.GetTechniqueByIdAsync(techniqueId, cancellationToken);
        return techniqueDto.Algorithm?.Split('\n') ?? [];
    }

    public async Task<bool> RemoveAsync(
        long techniqueId,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        IDialogService dialogService,
        CancellationToken cancellationToken)
    {
        bool isConfirmed = await dialogService.AskAsync(
            null,
            AppStrings.PracticeDeleteConfirm,
            AppStrings.Yes,
            AppStrings.No);

        if (!isConfirmed)
        {
            return false;
        }

        TechniqueDTO techniqueDto = await techniqueService.GetTechniqueByIdAsync(techniqueId, cancellationToken);
        await techniqueService.DeleteTechniqueAsync(techniqueDto, cancellationToken);
        techniqueMessenger.Send(new TechniqueMessage
        {
            MessageType = TechniqueMessageType.Remove,
            Technique = techniqueDto
        });

        return true;
    }

    public Task MarkCompletedAsync(
        long techniqueId,
        ITechniqueService techniqueService,
        CancellationToken cancellationToken) =>
        techniqueService.MarkTechniqueAsCompletedAsync(techniqueId, cancellationToken);
}
