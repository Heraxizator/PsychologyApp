using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed record DesignerTechniqueForm(
    string? Name,
    string? Description,
    string? Theme,
    string? Author,
    string? Actions,
    string? Path);

public sealed class DesignerTechniqueOperations
{
    public async Task<DesignerTechniqueForm?> LoadFormAsync(
        long techniqueId,
        ITechniqueService techniqueService,
        CancellationToken cancellationToken)
    {
        TechniqueDTO techniqueDto = await techniqueService.GetTechniqueByIdAsync(techniqueId, cancellationToken);

        if (techniqueDto.TechniqueId <= 0)
        {
            return null;
        }

        return new DesignerTechniqueForm(
            techniqueDto.Header,
            techniqueDto.Description,
            techniqueDto.Subject,
            techniqueDto.Author,
            techniqueDto.Algorithm,
            techniqueDto.Image);
    }

    public TechniqueDTO BuildDto(long techniqueId, DesignerTechniqueForm form) =>
        new()
        {
            TechniqueId = techniqueId,
            Header = form.Name,
            Description = form.Description,
            Subject = form.Theme,
            Author = form.Author,
            Algorithm = form.Actions,
            Image = form.Path,
            Number = Guid.NewGuid().ToString(),
            Date = GetTechniqueDate()
        };

    public async Task AddAsync(
        TechniqueDTO technique,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        CancellationToken cancellationToken)
    {
        await techniqueService.AddNewTechniqueAsync(technique, cancellationToken);
        techniqueMessenger.Send(new TechniqueMessage
        {
            MessageType = TechniqueMessageType.Add,
            Technique = technique
        });
    }

    public async Task UpdateAsync(
        TechniqueDTO technique,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        CancellationToken cancellationToken)
    {
        await techniqueService.UpdateTechniqueAsync(technique, cancellationToken);
        techniqueMessenger.Send(new TechniqueMessage
        {
            MessageType = TechniqueMessageType.Change,
            Technique = technique
        });
    }

    private static string GetTechniqueDate() =>
        DateTime.Now.ToString().Split(' ').First();
}
