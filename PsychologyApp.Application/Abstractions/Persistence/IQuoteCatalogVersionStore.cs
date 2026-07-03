namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IQuoteCatalogVersionStore
{
    Task<int> GetAsync(CancellationToken cancellationToken = default);

    Task SetAsync(int version, CancellationToken cancellationToken = default);
}
