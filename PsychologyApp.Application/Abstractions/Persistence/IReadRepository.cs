namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IReadRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
