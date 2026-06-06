using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Techniques;

public sealed class TechniqueRepository : BaseRepository<Technique>, ITechniqueRepository
{
    private readonly int _commandTimeoutSeconds;

    public TechniqueRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, EntitySqlMaps.Technique, settings)
    {
        _commandTimeoutSeconds = settings.Value.DbCommandTimeoutSeconds > 0
            ? settings.Value.DbCommandTimeoutSeconds
            : 30;
    }

    public async Task<IEnumerable<Technique>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Technique>(
            "SELECT * FROM Techniques ORDER BY TechniqueId DESC LIMIT @count;",
            new { count },
            commandTimeout: _commandTimeoutSeconds);
    }
}
