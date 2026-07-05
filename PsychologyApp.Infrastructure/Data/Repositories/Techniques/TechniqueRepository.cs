using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Techniques;

public sealed class TechniqueRepository : BaseRepository<Technique>, ITechniqueRepository
{
    public TechniqueRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, EntitySqlMaps.Technique, settings)
    {
    }

    public Task<IEnumerable<Technique>> GetLatestAsync(int count, CancellationToken cancellationToken = default) =>
        GetLatestPageAsync(offset: 0, limit: count, cancellationToken);

    public async Task<IEnumerable<Technique>> GetLatestPageAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QueryAsync<Technique>(DapperCommandFactory.Create(
            "SELECT * FROM Techniques ORDER BY TechniqueId DESC LIMIT @limit OFFSET @offset;",
            new { limit, offset },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }
}
