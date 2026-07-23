using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

public sealed class UserProgressRepository : SqliteRepositoryBase, IUserProgressRepository
{
    public UserProgressRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, settings)
    {
    }

    public async Task SaveTestResultAsync(TestResultDTO result, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.InsertTestResult,
            new
            {
                result.TestId,
                result.Score,
                result.Summary,
                result.DetailJson,
                CompletedAt = result.CompletedAt.ToString("O")
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<TestResultDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectLatestTestResult,
            new { testId },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<TestResultDTO?> GetMostRecentTestResultAsync(TimeSpan within, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<TestResultDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectMostRecentTestResultSince,
            new { sinceUtc = DateTime.UtcNow.Subtract(within).ToString("O") },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<TestResultDTO> rows = await connection.QueryAsync<TestResultDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectTestResultHistory,
            new { testId, limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<TestResultDTO>> GetLatestTestResultsAsync(
        IReadOnlyList<string> testIds,
        CancellationToken cancellationToken = default)
    {
        if (testIds.Count == 0)
        {
            return [];
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<TestResultDTO> rows = await connection.QueryAsync<TestResultDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectLatestTestResults,
            new { testIds },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<(string TestId, int Count)>> GetTestResultCountsAsync(
        IReadOnlyList<string> testIds,
        CancellationToken cancellationToken = default)
    {
        if (testIds.Count == 0)
        {
            return [];
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<(string TestId, int Count)> rows =
            await connection.QueryAsync<(string TestId, int Count)>(DapperCommandFactory.Create(
                UserProgressSql.SelectTestResultCounts,
                new { testIds },
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.QuerySingleOrDefaultAsync<string>(DapperCommandFactory.Create(
            UserProgressSql.SelectLastTechniqueCompletionDate,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return ParseOptionalUtcDateTime(value);
    }

    public async Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            UserProgressSql.CountTestResults,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task RecordCompletionAsync(CompletionDTO completion, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.InsertCompletion,
            new
            {
                completion.CompletionKind,
                completion.ItemKey,
                completion.ModuleName,
                completion.PageName,
                CompletedAt = completion.CompletedAt.ToString("O"),
                completion.DurationSeconds
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<long> RecordSessionOutcomeAsync(SessionOutcomeRequest request, CancellationToken cancellationToken = default)
    {
        DateTime completedAt = DateTime.UtcNow;
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(DapperCommandFactory.Create(
                UserProgressSql.InsertCompletion,
                new
                {
                    CompletionKind = "technique",
                    request.ItemKey,
                    request.ModuleName,
                    request.PageName,
                    CompletedAt = completedAt.ToString("O"),
                    request.DurationSeconds
                },
                transaction,
                CommandTimeoutSeconds,
                cancellationToken));

            await connection.ExecuteAsync(DapperCommandFactory.Create(
                UserProgressSql.InsertSessionResult,
                new
                {
                    request.ItemKey,
                    CompletedAt = completedAt.ToString("O"),
                    request.DurationSeconds,
                    request.PayloadJson,
                    request.PreIntensity,
                    PostIntensity = (int?)null,
                    request.ProgramType,
                    request.ProgramWeek
                },
                transaction,
                CommandTimeoutSeconds,
                cancellationToken));

            long sessionResultId = await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
                UserProgressSql.SelectLastInsertRowId,
                transaction: transaction,
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken));

            if (request.DeleteDraft)
            {
                await connection.ExecuteAsync(DapperCommandFactory.Create(
                    UserProgressSql.DeleteSessionDraft,
                    new { techniqueKey = request.ItemKey },
                    transaction,
                    CommandTimeoutSeconds,
                    cancellationToken));
            }

            await transaction.CommitAsync(cancellationToken);
            return sessionResultId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            UserProgressSql.CountTechniqueCompletions,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<CompletionDTO>> GetRecentTechniqueCompletionsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<CompletionDTO> rows = await connection.QueryAsync<CompletionDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectRecentTechniqueCompletions,
            new { limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<DateOnly>> GetCompletionDatesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<string> rows = await connection.QueryAsync<string>(DapperCommandFactory.Create(
            UserProgressSql.SelectCompletionDates,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows
            .Select(day => DateOnly.Parse(day, System.Globalization.CultureInfo.InvariantCulture))
            .ToList();
    }

    public async Task<DateTime?> GetLastCompletionForItemAsync(string itemKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        string? value = await connection.QuerySingleOrDefaultAsync<string>(DapperCommandFactory.Create(
            UserProgressSql.SelectLastCompletionForItem,
            new { itemKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return ParseOptionalUtcDateTime(value);
    }

    public async Task<IReadOnlyDictionary<string, DateTime>> GetLastPracticeDatesAsync(
        IReadOnlyList<string> itemKeys,
        CancellationToken cancellationToken = default)
    {
        if (itemKeys.Count == 0)
        {
            return new Dictionary<string, DateTime>(StringComparer.Ordinal);
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<(string ItemKey, string CompletedAt)> rows = await connection.QueryAsync<(string ItemKey, string CompletedAt)>(
            DapperCommandFactory.Create(
                UserProgressSql.SelectLastPracticeDates,
                new { itemKeys },
                commandTimeout: CommandTimeoutSeconds,
                cancellationToken: cancellationToken));

        Dictionary<string, DateTime> result = new(StringComparer.Ordinal);
        foreach ((string itemKey, string completedAt) in rows)
        {
            result[itemKey] = ParseUtcDateTime(completedAt);
        }

        return result;
    }

    public async Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.UpsertSessionDraft,
            new
            {
                techniqueKey,
                payloadJson,
                updatedAt = DateTime.UtcNow.ToString("O")
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<string>(DapperCommandFactory.Create(
            UserProgressSql.SelectSessionDraft,
            new { techniqueKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlySet<string>> GetSessionDraftKeysAsync(
        IReadOnlyList<string> techniqueKeys,
        CancellationToken cancellationToken = default)
    {
        if (techniqueKeys.Count == 0)
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<string> rows = await connection.QueryAsync<string>(DapperCommandFactory.Create(
            UserProgressSql.SelectSessionDraftKeys,
            new { techniqueKeys },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToHashSet(StringComparer.Ordinal);
    }

    public async Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.DeleteSessionDraft,
            new { techniqueKey },
            commandTimeout: CommandTimeoutSeconds,
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
                UserProgressSql.InsertMoodEntry,
                new
                {
                    entry.MoodLevel,
                    entry.Note,
                    RecordedAt = entry.RecordedAt.ToString("O")
                },
                transaction,
                CommandTimeoutSeconds,
                cancellationToken));

            await connection.ExecuteAsync(DapperCommandFactory.Create(
                UserProgressSql.InsertMoodCompletion,
                new
                {
                    moduleName = "Practice",
                    pageName = "Mood",
                    completedAt = entry.RecordedAt.ToString("O")
                },
                transaction,
                CommandTimeoutSeconds,
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
            UserProgressSql.SelectRecentMoods,
            new { limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<MoodEntryDTO>> GetMoodsAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<MoodEntryDTO> rows = await connection.QueryAsync<MoodEntryDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectMoodsInRange,
            new
            {
                fromUtc = fromUtc?.ToString("O"),
                toUtc = toUtc?.ToString("O"),
                limit
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task UpdateMoodEntryAsync(
        long moodEntryId,
        int moodLevel,
        string? note,
        CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.UpdateMoodEntry,
            new
            {
                MoodEntryId = moodEntryId,
                MoodLevel = moodLevel,
                Note = note
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task DeleteMoodEntryAsync(long moodEntryId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.DeleteMoodEntry,
            new { MoodEntryId = moodEntryId },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task UpdateSessionResultPostIntensityAsync(
        long sessionResultId,
        int postIntensity,
        CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            UserProgressSql.UpdateSessionResultPostIntensity,
            new { sessionResultId, postIntensity },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<SessionResultDTO?> GetSessionResultAsync(long sessionResultId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<SessionResultDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectSessionResultById,
            new { sessionResultId },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<SessionResultDTO>> GetRecentSessionResultsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<SessionResultDTO> rows = await connection.QueryAsync<SessionResultDTO>(DapperCommandFactory.Create(
            UserProgressSql.SelectRecentSessionResults,
            new { limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        return rows.ToList();
    }

    public async Task<int> CountDistinctTechniqueCompletionsForItemsBetweenAsync(
        IReadOnlyList<string> itemKeys,
        DateTime sinceUtc,
        DateTime beforeUtc,
        CancellationToken cancellationToken = default)
    {
        if (itemKeys.Count == 0)
        {
            return 0;
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(DapperCommandFactory.Create(
            UserProgressSql.CountDistinctTechniqueCompletionsForItemsBetween,
            new
            {
                itemKeys,
                sinceUtc = sinceUtc.ToString("O"),
                beforeUtc = beforeUtc.ToString("O")
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    private static DateTime? ParseOptionalUtcDateTime(string? value) =>
        value is null
            ? null
            : ParseUtcDateTime(value);

    private static DateTime ParseUtcDateTime(string value) =>
        DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
}
