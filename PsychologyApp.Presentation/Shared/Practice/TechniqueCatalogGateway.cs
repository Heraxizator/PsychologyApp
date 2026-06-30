using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Shared.Practice;

public sealed class TechniqueCatalogGateway(ITechniqueCatalogService catalogService)
{
    public TechniqueDefinition Get(TechniqueId techniqueId) =>
        TechniqueDefinitionMapper.ToPresentation(catalogService.Get(techniqueId));

    public IReadOnlyList<TechniqueListEntry> GetBuiltInListEntries() =>
        catalogService.GetBuiltInListEntries();
}
