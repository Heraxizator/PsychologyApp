using System.Data.Common;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
    string DatabasePath { get; }

    /// <summary>True when several connections may read at once (file database in WAL mode); false for the shared-cache in-memory test database, which locks whole tables.</summary>
    bool AllowsParallelReads => false;
}
