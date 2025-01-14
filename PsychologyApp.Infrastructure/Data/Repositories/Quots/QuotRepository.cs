using Dapper;
using Microsoft.Data.Sqlite;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.Data.Repositories.Quots;

public sealed class QuotRepository : BaseRepository<Quot>, IQuotRepository
{
    private readonly SqliteConnection _connection;

    public QuotRepository(SqliteConnection connection) : base(connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Quot>> GetByThemeAsync(string theme)
    {
        return await _connection.QueryAsync<Quot>($"SELECT * FROM Quots WHERE Theme = {theme}");
    }

    public async Task<IEnumerable<Quot>> GetByTitleAsync(string title)
    {
        return await _connection.QueryAsync<Quot>($"SELECT * FROM Quots WHERE Title = {title}");
    }
}
