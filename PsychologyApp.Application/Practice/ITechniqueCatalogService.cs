using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

public interface ITechniqueCatalogService
{
    IReadOnlyList<BuiltInTechniqueDefinition> GetAll();

    BuiltInTechniqueDefinition Get(TechniqueId techniqueId);

    IReadOnlyList<TechniqueListEntry> GetBuiltInListEntries();

    Task<IReadOnlyList<BuiltInTechniqueDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BuiltInTechniqueDefinition> GetAsync(TechniqueId techniqueId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TechniqueListEntry>> GetBuiltInListEntriesAsync(CancellationToken cancellationToken = default);

    void Invalidate();
}
