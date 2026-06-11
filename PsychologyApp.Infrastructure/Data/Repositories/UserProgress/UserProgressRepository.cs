using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.Data;
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
        await connection.ExecuteAsync(DapperCommandFactory.Create(
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
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<TestResultDTO>(DapperCommandFactory.Create(
            """
            SELECT TestResultId, TestId, Score, Summary, DetailJson, CompletedAt
            FROM TestResults
            WHERE TestId = @testId
            ORDER BY TestResultId DESC
            LIMIT 1;
            """,
            new { testId },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<TestResultDTO> rows = await connection.QueryAsync<TestResultDTO>(DapperCommandFactory.Create(
            """
            SELECT TestResultId, TestId, Score, Summary, DetailJson, CompletedAt
            FROM TestResults
            WHERE TestId = @testId
            ORDER BY TestResultId DESC
            LIMIT @limit;
            """,
            new { testId, limit },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.QuerySingleOrDefaultAsync<string>(DapperCommandFactory.Create(
            """
            SELECT CompletedAt
            FROM Completions
            WHERE CompletionKind = 'technique'
            ORDER BY CompletionId DESC
            LIMIT 1;
            """,
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return value is null
            ? null
            : DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    public async Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            "SELECT COUNT(*) FROM TestResults;",
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task RecordCompletionAsync(CompletionDTO completion, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
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
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            "SELECT COUNT(*) FROM Completions WHERE CompletionKind = 'technique';",
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<DateOnly>> GetCompletionDatesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<string> rows = await connection.QueryAsync<string>(DapperCommandFactory.Create(
            """
            SELECT DISTINCT substr(CompletedAt, 1, 10) AS Day
            FROM Completions
            ORDER BY Day DESC;
            """,
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows
            .Select(day => DateOnly.Parse(day, System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
    }

    public async Task<DateTime?> GetLastCompletionForItemAsync(string itemKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.QuerySingleOrDefaultAsync<string>(DapperCommandFactory.Create(
            """
            SELECT CompletedAt
            FROM Completions
            WHERE ItemKey = @itemKey
            ORDER BY CompletionId DESC
            LIMIT 1;
            """,
            new { itemKey },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return value is null
            ? null
            : DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    public async Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
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
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<string>(DapperCommandFactory.Create(
            "SELECT PayloadJson FROM SessionDrafts WHERE TechniqueKey = @techniqueKey;",
            new { techniqueKey },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            "DELETE FROM SessionDrafts WHERE TechniqueKey = @techniqueKey;",
            new { techniqueKey },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task RecordMoodAsync(MoodEntryDTO entry, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(DapperCommandFactory.Create(
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
                transaction,
                _commandTimeoutSeconds,
                cancellationToken));

            await connection.ExecuteAsync(DapperCommandFactory.Create(
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
                transaction,
                _commandTimeoutSeconds,
                cancellationToken));

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<MoodEntryDTO> rows = await connection.QueryAsync<MoodEntryDTO>(DapperCommandFactory.Create(
            """
            SELECT MoodEntryId, MoodLevel, Note, RecordedAt
            FROM MoodEntries
            ORDER BY MoodEntryId DESC
            LIMIT @limit;
            """,
            new { limit },
            commandTimeout: _commandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = (SqliteConnection)await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
        return connection;
    }
}
