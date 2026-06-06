using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface ITechniqueRepository : IReadRepository<Technique>, IWriteRepository<Technique>
{
    Task<IEnumerable<Technique>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
}
