using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Domain.Common;

public interface IGenericRepository<TEntity> where TEntity : class
{
    void Insert(TEntity item);
    TEntity? FindById(long id);
    IEnumerable<TEntity> Get();
    IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);
    void Remove(TEntity item);
    void Update(TEntity item);
    void InsertOrUpdate(TEntity item);
    void InsertRange(IEnumerable<TEntity> entities);
    void RemoveRange(IEnumerable<TEntity> entities);
    void UpdateRange(IEnumerable<TEntity> entities);
    void InsertOrUpdateRange(IEnumerable<TEntity> entities);
}
