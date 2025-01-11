using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
{
    private readonly SqliteConnection _connection;

    protected BaseRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<long> AddAsync(TEntity entity)
    {
        return await _connection.InsertAsync(entity);
    }

    public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        return await _connection.InsertAsync(entities) > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        return await _connection.DeleteAsync(entity);
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        return await _connection.DeleteAsync(entities);
    }

    public async Task<bool> EditAsync(TEntity entity)
    {
        return await _connection.UpdateAsync(entity);
    }

    public async Task<bool> EditRangeAsync(IEnumerable<TEntity> entities)
    {
        return await _connection.UpdateAsync(entities);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _connection.GetAllAsync<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(long id)
    {
        return await _connection.GetAsync<TEntity>(id);
    }
}
