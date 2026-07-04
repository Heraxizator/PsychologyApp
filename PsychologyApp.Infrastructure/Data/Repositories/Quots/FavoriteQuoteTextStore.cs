using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Quots;

public sealed class FavoriteQuoteTextStore(
    IDbConnectionFactory connectionFactory,
    IOptions<AppSettings> settings) : IFavoriteQuoteTextStore
{
    private const string TextsKey = "FavoriteQuoteTexts";
    private const string LegacyIndicesKey = "FavoriteQuoteIndices";
    private int CommandTimeoutSeconds => settings.Value.DbCommandTimeoutSeconds;

    public async Task<IReadOnlySet<string>> GetTextsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.ExecuteScalarAsync<string?>(DapperCommandFactory.Create(
            "SELECT Value FROM AppMetadata WHERE Key = @key;",
            new { key = TextsKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return ParseTexts(value);
    }

    public async Task AddTextAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        HashSet<string> texts = new(await GetTextsAsync(cancellationToken), StringComparer.Ordinal);
        if (!texts.Add(text))
        {
            return;
        }

        await SaveTextsInternalAsync(texts, cancellationToken);
    }

    public async Task RemoveTextAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        HashSet<string> texts = new(await GetTextsAsync(cancellationToken), StringComparer.Ordinal);
        if (!texts.Remove(text))
        {
            return;
        }

        await SaveTextsInternalAsync(texts, cancellationToken);
    }

    public async Task SaveTextsAsync(IReadOnlySet<string> texts, CancellationToken cancellationToken = default)
    {
        HashSet<string> normalized = new(StringComparer.Ordinal);
        foreach (string text in texts)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                normalized.Add(text);
            }
        }

        await SaveTextsInternalAsync(normalized, cancellationToken);
        await ClearLegacyIndicesAsync(cancellationToken);
    }

    public async Task<IReadOnlySet<int>> GetLegacyIndicesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.ExecuteScalarAsync<string?>(DapperCommandFactory.Create(
            "SELECT Value FROM AppMetadata WHERE Key = @key;",
            new { key = LegacyIndicesKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return ParseIndices(value);
    }

    public async Task ClearLegacyIndicesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            "DELETE FROM AppMetadata WHERE Key = @key;",
            new { key = LegacyIndicesKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    private async Task SaveTextsInternalAsync(HashSet<string> texts, CancellationToken cancellationToken)
    {
        string serialized = string.Join('\n', texts.OrderBy(text => text, StringComparer.Ordinal));
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            """
            INSERT INTO AppMetadata (Key, Value)
            VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
            """,
            new { key = TextsKey, value = serialized },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    private static HashSet<string> ParseTexts(string? value)
    {
        HashSet<string> texts = new(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(value))
        {
            return texts;
        }

        foreach (string part in value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            texts.Add(part);
        }

        return texts;
    }

    private static HashSet<int> ParseIndices(string? value)
    {
        HashSet<int> indices = [];
        if (string.IsNullOrWhiteSpace(value))
        {
            return indices;
        }

        foreach (string part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(part, out int index))
            {
                indices.Add(index);
            }
        }

        return indices;
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        SqliteConnection connection = (SqliteConnection)await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }
}
