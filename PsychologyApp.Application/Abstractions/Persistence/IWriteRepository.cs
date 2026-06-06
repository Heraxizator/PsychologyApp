namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IWriteRepository<TEntity> where TEntity : class
{
    Task<long> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> EditAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
