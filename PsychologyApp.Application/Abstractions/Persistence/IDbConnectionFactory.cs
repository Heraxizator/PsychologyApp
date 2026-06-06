using System.Data.Common;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
    string DatabasePath { get; }
}
