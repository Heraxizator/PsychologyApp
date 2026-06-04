using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Techniques;

public sealed class TechniqueRepository : BaseRepository<Technique>, ITechniqueRepository
{
    public TechniqueRepository(SqliteConnection connection)
        : base(connection, EntitySqlMaps.Technique)
    {
    }

    public Task<IEnumerable<Technique>> GetByAuthorAsync(string author) =>
        Connection.QueryAsync<Technique>(
            "SELECT * FROM Techniques WHERE Author = @author;",
            new { author });

    public Task<IEnumerable<Technique>> GetByHeaderAsync(string header) =>
        Connection.QueryAsync<Technique>(
            "SELECT * FROM Techniques WHERE Header = @header;",
            new { header });

    public Task<IEnumerable<Technique>> GetBySubjectAsync(string subject) =>
        Connection.QueryAsync<Technique>(
            "SELECT * FROM Techniques WHERE Subject = @subject;",
            new { subject });
}
