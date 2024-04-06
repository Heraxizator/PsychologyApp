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

    public async Task<IEnumerable<TEntity>> GetAsync()
    {
        return await this._dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate)
    {
        return (await this._dbSet.AsNoTracking().ToListAsync()).Where(predicate).ToList();
    }
    public async Task<TEntity?> FindByIdAsync(long id)
    {
        return await this._dbSet.FindAsync(id);
    }

    public async Task UpdateAsync(TEntity item)
    {
        this._context.Update(item);
    }

    public async Task RemoveAsync(TEntity item)
    {
        this._dbSet.Remove(item);
    }

    public async Task InsertAsync(TEntity item)
    {
        await this._dbSet.AddAsync(item);
    }

    public async Task InsertOrUpdateAsync(TEntity item)
    {
        this._dbSet.Update(item);
    }

    public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
    {
        await this._dbSet.AddRangeAsync(entities);
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
    {
        this._dbSet.RemoveRange(entities);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        this._context.UpdateRange(entities);
    }

    public async Task InsertOrUpdateRangeAsync(IEnumerable<TEntity> entities)
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
