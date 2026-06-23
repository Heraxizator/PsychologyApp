using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface ITechniqueRepository : IReadRepository<global::PsychologyApp.Domain.Entities.Technique>, IWriteRepository<global::PsychologyApp.Domain.Entities.Technique>
{
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Technique>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
}
