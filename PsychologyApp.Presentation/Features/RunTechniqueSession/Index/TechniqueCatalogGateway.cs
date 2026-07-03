using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.Index;

public sealed class TechniqueCatalogGateway(ITechniqueCatalogService catalogService)
{
    public TechniqueDefinition Get(TechniqueId techniqueId) =>
        TechniqueDefinitionMapper.ToPresentation(catalogService.Get(techniqueId));

    public IReadOnlyList<TechniqueListEntry> GetBuiltInListEntries() =>
        catalogService.GetBuiltInListEntries();
}
