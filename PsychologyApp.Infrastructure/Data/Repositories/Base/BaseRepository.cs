using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

public abstract class BaseRepository<TEntity> : SqliteRepositoryBase, IRepository<TEntity> where TEntity : class
{
    private readonly EntitySqlMap _sql;

    protected BaseRepository(
        IDbConnectionFactory connectionFactory,
        EntitySqlMap sql,
        IOptions<AppSettings> settings)
        : base(connectionFactory, settings)
    {
        _sql = sql;
    }

    public async Task<long> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await using SqliteTransaction transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(DapperCommandFactory.Create(
                _sql.InsertSql,
                entity,
                transaction,
                CommandTimeoutSeconds,
                cancellationToken));
            long id = await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
                "SELECT last_insert_rowid();",
                transaction: transaction,
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
            return id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        int affected = await connection.ExecuteAsync(DapperCommandFactory.Create(
            _sql.DeleteSql,
            entity,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        if (affected == 0)
        {
            throw new PersistenceException($"Entity of type {typeof(TEntity).Name} was not deleted because no rows were affected.");
        }

        return true;
    }

    public async Task<bool> EditAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        int affected = await connection.ExecuteAsync(DapperCommandFactory.Create(
            _sql.UpdateSql,
            entity,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        if (affected == 0)
        {
            throw new PersistenceException($"Entity of type {typeof(TEntity).Name} was not updated because no rows were affected.");
        }

        return true;
    }

    public async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<TEntity>(DapperCommandFactory.Create(
            _sql.SelectByKeySql,
            new { id },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }
}
