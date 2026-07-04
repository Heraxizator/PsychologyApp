using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.Index;

public sealed class TechniqueCatalogGateway(ITechniqueCatalogService catalogService)
{
    public async Task<TechniqueDefinition> GetAsync(
        TechniqueId techniqueId,
        CancellationToken cancellationToken = default) =>
        TechniqueDefinitionMapper.ToPresentation(await catalogService.GetAsync(techniqueId, cancellationToken));

    public async Task<IReadOnlyList<TechniqueListEntry>> GetBuiltInListEntriesAsync(
        CancellationToken cancellationToken = default) =>
        await catalogService.GetBuiltInListEntriesAsync(cancellationToken);
}
