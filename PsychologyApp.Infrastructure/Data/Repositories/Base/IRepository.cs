using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<IEnumerable<TEntity>> GetAllAsync(int cancelTimeout);
    Task<TEntity> GetByIdAsync(long id, int cancelTimeout);
    Task<long> AddAsync(TEntity entity, int cancelTimeout);
    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout);
    Task<bool> EditAsync(TEntity entity, int cancelTimeout);
    Task<bool> EditRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout);
    Task<bool> DeleteAsync(TEntity entity, int cancelTimeout);
    Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout);
}
