using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Reasons;

public sealed class ReasonRepository : BaseRepository<Reason>, IReasonRepository
{
    public ReasonRepository(SqliteConnection connection)
        : base(connection, EntitySqlMaps.Reason)
    {
    }

    public Task<IEnumerable<Reason>> GetByTitle(string title) =>
        Connection.QueryAsync<Reason>(
            "SELECT * FROM Reasons WHERE Title = @title;",
            new { title });
}
