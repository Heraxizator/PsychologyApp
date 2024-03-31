using Microsoft.EntityFrameworkCore;
using PsychologyApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        this._context = context;
        this._dbSet = context.Set<TEntity>();
    }

    public IEnumerable<TEntity> Get()
    {
        return this._dbSet.AsNoTracking().ToList();
    }

    public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
    {
        return this._dbSet.AsNoTracking().Where(predicate).ToList();
    }
    public TEntity? FindById(long id)
    {
        return this._dbSet.Find(id);
    }

    public void Update(TEntity item)
    {
        this._context.Update(item);
    }

    public void Remove(TEntity item)
    {
        this._dbSet.Remove(item);
    }

    public void Insert(TEntity item)
    {
        this._dbSet.Add(item);
    }

    public void InsertOrUpdate(TEntity item)
    {
        this._dbSet.Update(item);
    }

    public void InsertRange(IEnumerable<TEntity> entities)
    {
        this._dbSet.AddRange(entities);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        this._dbSet.RemoveRange(entities);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        this._context.UpdateRange(entities);
    }

    public void InsertOrUpdateRange(IEnumerable<TEntity> entities)
    {
        this._dbSet.UpdateRange(entities);
    }

    public IEnumerable<TEntity> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return Include(includeProperties).ToList();
    }

    public IEnumerable<TEntity> GetWithInclude(Func<TEntity, bool> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = Include(includeProperties);
        return query.Where(predicate).ToList();
    }

    private IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = this._dbSet.AsNoTracking();
        return includeProperties
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }
}
