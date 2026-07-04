using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

public interface ITechniqueCatalogService
{
    Task<IReadOnlyList<BuiltInTechniqueDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BuiltInTechniqueDefinition> GetAsync(TechniqueId techniqueId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TechniqueListEntry>> GetBuiltInListEntriesAsync(CancellationToken cancellationToken = default);

    void Invalidate();
}
