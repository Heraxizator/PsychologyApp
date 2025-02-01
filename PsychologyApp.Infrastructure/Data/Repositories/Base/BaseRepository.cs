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

    public async Task<long> AddAsync(TEntity entity, int cancelTimeout)
    {
        return await _connection.InsertAsync(entity, null, cancelTimeout);
    }

    public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout)
    {
        return await _connection.InsertAsync(entities, null, cancelTimeout) > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity, int cancelTimeout)
    {
        return await _connection.DeleteAsync(entity, null, cancelTimeout);
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout)
    {
        return await _connection.DeleteAsync(entities, null, cancelTimeout);
    }

    public async Task<bool> EditAsync(TEntity entity, int cancelTimeout)
    {
        return await _connection.UpdateAsync(entity, null, cancelTimeout);
    }

    public async Task<bool> EditRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout)
    {
        return await _connection.UpdateAsync(entities, null, cancelTimeout);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int cancelTimeout)
    {
        return await _connection.GetAllAsync<TEntity>(null, cancelTimeout);
    }

    public async Task<TEntity> GetByIdAsync(long id, int cancelTimeout)
    {
        return await _connection.GetAsync<TEntity>(id, null, cancelTimeout);
    }
}
