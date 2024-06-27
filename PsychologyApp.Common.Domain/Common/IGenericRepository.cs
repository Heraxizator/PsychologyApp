using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Common;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task InsertAsync(TEntity item);
    Task<TEntity?> FindByIdAsync(long id);
    Task<IEnumerable<TEntity>> GetAsync();
    Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate);
    Task RemoveAsync(TEntity item);
    Task UpdateAsync(TEntity item);
    Task InsertOrUpdateAsync(TEntity item);
    Task InsertRangeAsync(IEnumerable<TEntity> entities);
    Task RemoveRangeAsync(IEnumerable<TEntity> entities);
    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task InsertOrUpdateRangeAsync(IEnumerable<TEntity> entities);
}
