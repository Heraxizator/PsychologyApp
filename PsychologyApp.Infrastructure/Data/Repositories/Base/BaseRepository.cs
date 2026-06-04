using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly SqliteConnection _connection;
    private readonly EntitySqlMap _sql;

    protected BaseRepository(SqliteConnection connection, EntitySqlMap sql)
    {
        _connection = connection;
        _sql = sql;
    }

    protected SqliteConnection Connection => _connection;

    public async Task<long> AddAsync(TEntity entity, int cancelTimeout)
    {
        await _connection.ExecuteAsync(_sql.InsertSql, entity, commandTimeout: cancelTimeout);
        return await _connection.ExecuteScalarAsync<long>("SELECT last_insert_rowid();", commandTimeout: cancelTimeout);
    }

    public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout)
    {
        using SqliteTransaction transaction = _connection.BeginTransaction();
        int affected = 0;

        foreach (TEntity entity in entities)
        {
            affected += await _connection.ExecuteAsync(
                _sql.InsertSql,
                entity,
                transaction,
                commandTimeout: cancelTimeout);
        }

        transaction.Commit();
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity, int cancelTimeout)
    {
        int affected = await _connection.ExecuteAsync(_sql.DeleteSql, entity, commandTimeout: cancelTimeout);
        return affected > 0;
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout)
    {
        using SqliteTransaction transaction = _connection.BeginTransaction();
        int affected = 0;

        foreach (TEntity entity in entities)
        {
            affected += await _connection.ExecuteAsync(
                _sql.DeleteSql,
                entity,
                transaction,
                commandTimeout: cancelTimeout);
        }

        transaction.Commit();
        return affected > 0;
    }

    public async Task<bool> EditAsync(TEntity entity, int cancelTimeout)
    {
        int affected = await _connection.ExecuteAsync(_sql.UpdateSql, entity, commandTimeout: cancelTimeout);
        return affected > 0;
    }

    public async Task<bool> EditRangeAsync(IEnumerable<TEntity> entities, int cancelTimeout)
    {
        using SqliteTransaction transaction = _connection.BeginTransaction();
        int affected = 0;

        foreach (TEntity entity in entities)
        {
            affected += await _connection.ExecuteAsync(
                _sql.UpdateSql,
                entity,
                transaction,
                commandTimeout: cancelTimeout);
        }

        transaction.Commit();
        return affected > 0;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int cancelTimeout)
    {
        return await _connection.QueryAsync<TEntity>(_sql.SelectAllSql, commandTimeout: cancelTimeout);
    }

    public async Task<TEntity?> GetByIdAsync(long id, int cancelTimeout)
    {
        return await _connection.QuerySingleOrDefaultAsync<TEntity>(
            _sql.SelectByKeySql,
            new { id },
            commandTimeout: cancelTimeout);
    }
}
