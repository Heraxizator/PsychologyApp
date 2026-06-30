using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

public interface ITechniqueCatalogService
{
    IReadOnlyList<BuiltInTechniqueDefinition> GetAll();

    BuiltInTechniqueDefinition Get(TechniqueId techniqueId);

    IReadOnlyList<TechniqueListEntry> GetBuiltInListEntries();

    void Invalidate();
}
