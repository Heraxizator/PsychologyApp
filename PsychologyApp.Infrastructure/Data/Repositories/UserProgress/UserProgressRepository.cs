using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

public sealed class UserProgressRepository : IUserProgressRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly int _commandTimeoutSeconds;

    public UserProgressRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    {
        _connectionFactory = connectionFactory;
        _commandTimeoutSeconds = settings.Value.DbCommandTimeoutSeconds > 0
            ? settings.Value.DbCommandTimeoutSeconds
            : 30;
    }

    public async Task SaveTestResultAsync(TestResultDTO result, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(
            """
            INSERT INTO TestResults (TestId, Score, Summary, DetailJson, CompletedAt)
            VALUES (@TestId, @Score, @Summary, @DetailJson, @CompletedAt);
            """,
            new
            {
                result.TestId,
                result.Score,
                result.Summary,
                result.DetailJson,
                CompletedAt = result.CompletedAt.ToString("O")
            },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<TestResultDTO>(
            """
            SELECT TestResultId, TestId, Score, Summary, DetailJson, CompletedAt
            FROM TestResults
            WHERE TestId = @testId
            ORDER BY TestResultId DESC
            LIMIT 1;
            """,
            new { testId },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM TestResults;",
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task RecordCompletionAsync(CompletionDTO completion, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(
            """
            INSERT INTO Completions (CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds)
            VALUES (@CompletionKind, @ItemKey, @ModuleName, @PageName, @CompletedAt, @DurationSeconds);
            """,
            new
            {
                completion.CompletionKind,
                completion.ItemKey,
                completion.ModuleName,
                completion.PageName,
                CompletedAt = completion.CompletedAt.ToString("O"),
                completion.DurationSeconds
            },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM Completions WHERE CompletionKind = 'technique';",
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task<IReadOnlyList<DateOnly>> GetCompletionDatesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<string> rows = await connection.QueryAsync<string>(
            """
            SELECT DISTINCT substr(CompletedAt, 1, 10) AS Day
            FROM Completions
            ORDER BY Day DESC;
            """,
            commandTimeout: _commandTimeoutSeconds);

        return rows
            .Select(day => DateOnly.Parse(day, System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
    }

    public async Task<DateTime?> GetLastCompletionForItemAsync(string itemKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.QuerySingleOrDefaultAsync<string>(
            """
            SELECT CompletedAt
            FROM Completions
            WHERE ItemKey = @itemKey
            ORDER BY CompletionId DESC
            LIMIT 1;
            """,
            new { itemKey },
            commandTimeout: _commandTimeoutSeconds);

        return value is null ? null : DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    public async Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(
            """
            INSERT INTO SessionDrafts (TechniqueKey, PayloadJson, UpdatedAt)
            VALUES (@techniqueKey, @payloadJson, @updatedAt)
            ON CONFLICT(TechniqueKey) DO UPDATE SET
                PayloadJson = excluded.PayloadJson,
                UpdatedAt = excluded.UpdatedAt;
            """,
            new
            {
                techniqueKey,
                payloadJson,
                updatedAt = DateTime.UtcNow.ToString("O")
            },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<string>(
            "SELECT PayloadJson FROM SessionDrafts WHERE TechniqueKey = @techniqueKey;",
            new { techniqueKey },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(
            "DELETE FROM SessionDrafts WHERE TechniqueKey = @techniqueKey;",
            new { techniqueKey },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task RecordMoodAsync(MoodEntryDTO entry, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(
            """
            INSERT INTO MoodEntries (MoodLevel, Note, RecordedAt)
            VALUES (@MoodLevel, @Note, @RecordedAt);
            """,
            new
            {
                entry.MoodLevel,
                entry.Note,
                RecordedAt = entry.RecordedAt.ToString("O")
            },
            commandTimeout: _commandTimeoutSeconds);

        await connection.ExecuteAsync(
            """
            INSERT INTO Completions (CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds)
            VALUES ('mood', 'mood', @moduleName, @pageName, @completedAt, 0);
            """,
            new
            {
                moduleName = "Practice",
                pageName = "Mood",
                completedAt = entry.RecordedAt.ToString("O")
            },
            commandTimeout: _commandTimeoutSeconds);
    }

    public async Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<MoodEntryDTO> rows = await connection.QueryAsync<MoodEntryDTO>(
            """
            SELECT MoodEntryId, MoodLevel, Note, RecordedAt
            FROM MoodEntries
            ORDER BY MoodEntryId DESC
            LIMIT @limit;
            """,
            new { limit },
            commandTimeout: _commandTimeoutSeconds);

        return rows.ToList();
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }
}
