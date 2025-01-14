using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Techniques;

public sealed class TechniqueRepository : BaseRepository<Technique>, ITechniqueRepository
{
    private readonly SqliteConnection _connection;

    public TechniqueRepository(SqliteConnection connection) : base(connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Technique>> GetByAuthorAsync(string author)
    {
        return await _connection.QueryAsync<Technique>($"SELECT * FROM Techniques WHERE Author = {author}");
    }

    public async Task<IEnumerable<Technique>> GetByHeaderAsync(string header)
    {
        return await _connection.QueryAsync<Technique>($"SELECT * FROM Techniques WHERE Header = {header}");
    }

    public async Task<IEnumerable<Technique>> GetBySubjectAsync(string subject)
    {
        return await _connection.QueryAsync<Technique>($"SELECT * FROM Techniques WHERE Subject = {subject}");
    }
}
