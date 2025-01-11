using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(long id);
    Task<long> AddAsync(TEntity entity);
    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<bool> EditAsync(TEntity entity);
    Task<bool> EditRangeAsync(IEnumerable<TEntity> entities);
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities);
}
