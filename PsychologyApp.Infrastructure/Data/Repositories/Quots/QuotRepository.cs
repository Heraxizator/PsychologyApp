using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Quots;

public sealed class QuotRepository : BaseRepository<Quot>, IQuotRepository
{
    public QuotRepository(SqliteConnection connection)
        : base(connection, EntitySqlMaps.Quot)
    {
    }

    public Task<IEnumerable<Quot>> GetByThemeAsync(string theme) =>
        Connection.QueryAsync<Quot>(
            "SELECT * FROM Quots WHERE Theme = @theme;",
            new { theme });

    public Task<IEnumerable<Quot>> GetByTitleAsync(string title) =>
        Connection.QueryAsync<Quot>(
            "SELECT * FROM Quots WHERE Title = @title;",
            new { title });
}
