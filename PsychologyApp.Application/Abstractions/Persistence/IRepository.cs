namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity> where TEntity : class
{
}
