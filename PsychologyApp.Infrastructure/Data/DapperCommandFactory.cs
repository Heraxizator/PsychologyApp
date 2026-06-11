using System.Data;
using Dapper;

namespace PsychologyApp.Infrastructure.Data;

internal static class DapperCommandFactory
{
    public static CommandDefinition Create(
        string sql,
        object? parameters = null,
        IDbTransaction? transaction = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default) =>
        new(sql, parameters, transaction, commandTimeout: commandTimeout, cancellationToken: cancellationToken);
}
