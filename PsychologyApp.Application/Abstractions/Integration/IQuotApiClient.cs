using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Integration;

public interface IQuotApiClient
{
    Task<Quot> FetchRandomQuotAsync(CancellationToken cancellationToken = default);
}
