using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Integration;

public interface IReasonContentProvider
{
    Task<IEnumerable<Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
