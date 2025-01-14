using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Reasons;

public sealed class ReasonRepository : BaseRepository<Reason>, IReasonRepository
{
    private readonly SqliteConnection _connection;
    public ReasonRepository(SqliteConnection connection) : base(connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Reason>> GetByTitle(string title)
    {
        return await _connection.QueryAsync<Reason>($"SELECT * FROM Reasons WHERE Title = {title}");
    }
}
