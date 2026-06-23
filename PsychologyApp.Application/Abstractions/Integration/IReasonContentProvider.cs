using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Integration;

public interface IReasonContentProvider
{
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<global::PsychologyApp.Domain.Entities.Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
