using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;

namespace PsychologyApp.Infrastructure.Data.Repositories.Base;

/// <summary>
/// One small document read and written whole (the fear ladder, the reading state, the body log): JSON in AppMetadata, inside the
/// encrypted database, with no table of its own to migrate.
/// </summary>
public abstract class AppMetadataDocumentRepository<T>(
    IDbConnectionFactory connectionFactory,
    IOptions<AppSettings> settings,
    string key,
    JsonTypeInfo<T> typeInfo) : SqliteRepositoryBase(connectionFactory, settings)
    where T : class
{
    /// <summary>Where an unreadable document is set aside, so the next save does not destroy it.</summary>
    public string UnreadableKey => key + ".unreadable";

    /// <summary>
    /// The document, or null when there is none. A document that cannot be read (damaged, or written by a newer version) also reads as
    /// null, so the screens keep working; its text is first copied to <see cref="UnreadableKey"/>, because the next save replaces it.
    /// </summary>
    protected async Task<T?> ReadAsync(CancellationToken cancellationToken)
    {
        string? json = await SelectAsync(key, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize(json, typeInfo);
        }
        catch (JsonException)
        {
            await UpsertAsync(UnreadableKey, json, cancellationToken).ConfigureAwait(false);
            return null;
        }
    }

    protected Task WriteAsync(T document, CancellationToken cancellationToken) =>
        UpsertAsync(key, JsonSerializer.Serialize(document, typeInfo), cancellationToken);

    protected async Task RemoveAsync(CancellationToken cancellationToken)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            "DELETE FROM AppMetadata WHERE Key = @key;",
            new { key },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    private async Task<string?> SelectAsync(string metadataKey, CancellationToken cancellationToken)
    {
        await using SqliteConnection connection = await OpenReadConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<string?>(DapperCommandFactory.Create(
            "SELECT Value FROM AppMetadata WHERE Key = @key;",
            new { key = metadataKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    private async Task UpsertAsync(string metadataKey, string value, CancellationToken cancellationToken)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            """
            INSERT INTO AppMetadata (Key, Value)
            VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
            """,
            new { key = metadataKey, value },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
    }
}
