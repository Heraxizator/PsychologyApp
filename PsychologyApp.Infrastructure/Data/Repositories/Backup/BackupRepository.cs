using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Backup;

public sealed class BackupRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    : SqliteRepositoryBase(connectionFactory, settings), IBackupRepository
{
    private const string FavouriteTextsKey = "FavoriteQuoteTexts";

    public async Task<IReadOnlyList<BackupTechniqueDTO>> GetTechniquesAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenReadConnectionAsync(cancellationToken);
        IEnumerable<BackupTechniqueDTO> rows = await connection.QueryAsync<BackupTechniqueDTO>(DapperCommandFactory.Create(
            BackupSql.SelectTechniques,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
        return rows.ToList();
    }

    public async Task<BackupImportResult> ImportAsync(AppBackupDTO backup, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        // Any failure leaves the transaction uncommitted, so disposing it rolls everything back: no half-restored database.
        Func<string, object, Task<int>> run = (sql, parameters) => connection.ExecuteAsync(DapperCommandFactory.Create(
            sql, parameters, transaction, CommandTimeoutSeconds, cancellationToken));

        int moods = 0;
        foreach (MoodEntryDTO mood in backup.MoodEntries)
        {
            string recordedAt = SqliteTime.ToIso(mood.RecordedAt);
            moods += await run(BackupSql.InsertMood, new { mood.MoodLevel, mood.Note, RecordedAt = recordedAt }).ConfigureAwait(false);
            // The journal streak counts mood check-ins as completions; RecordMoodAsync writes both, so a restore must too.
            await run(BackupSql.InsertMoodCompletion, new { RecordedAt = recordedAt }).ConfigureAwait(false);
        }

        int tests = 0;
        foreach (TestResultDTO result in backup.TestResults)
        {
            tests += await run(BackupSql.InsertTestResult, new
            {
                result.TestId,
                result.Score,
                result.Summary,
                result.DetailJson,
                CompletedAt = SqliteTime.ToIso(result.CompletedAt)
            }).ConfigureAwait(false);
        }

        int completions = 0;
        foreach (CompletionDTO completion in backup.Completions)
        {
            completions += await run(BackupSql.InsertCompletion, new
            {
                completion.CompletionKind,
                completion.ItemKey,
                completion.ModuleName,
                completion.PageName,
                CompletedAt = SqliteTime.ToIso(completion.CompletedAt),
                completion.DurationSeconds
            }).ConfigureAwait(false);
        }

        int sessionResults = 0;
        foreach (SessionResultDTO result in backup.SessionResults)
        {
            sessionResults += await run(BackupSql.InsertSessionResult, new
            {
                result.ItemKey,
                CompletedAt = SqliteTime.ToIso(result.CompletedAt),
                result.DurationSeconds,
                result.PayloadJson,
                result.PreIntensity,
                result.PostIntensity,
                result.ProgramType,
                result.ProgramWeek,
                result.Note
            }).ConfigureAwait(false);
        }

        int chats = 0;
        foreach (BackupChatSessionDTO chat in backup.ChatSessions)
        {
            int added = await run(BackupSql.InsertChatSession, new
            {
                chat.Session.Title,
                CreatedAt = SqliteTime.ToIso(chat.Session.CreatedAt),
                UpdatedAt = SqliteTime.ToIso(chat.Session.UpdatedAt == default ? chat.Session.CreatedAt : chat.Session.UpdatedAt),
                chat.Session.Emotion,
                chat.Session.Theme,
                chat.Session.FirstIntensity,
                chat.Session.LastIntensity,
                chat.Session.StateJson
            }).ConfigureAwait(false);
            if (added == 0)
            {
                continue;
            }

            chats++;
            long sessionId = await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
                "SELECT last_insert_rowid();", transaction: transaction, commandTimeout: CommandTimeoutSeconds, cancellationToken: cancellationToken)).ConfigureAwait(false);
            foreach (ChatMessageDTO message in chat.Messages.OrderBy(m => m.Id))
            {
                await run(BackupSql.InsertChatMessage, new
                {
                    SessionId = sessionId,
                    Role = (int)message.Role,
                    message.Text,
                    CreatedAt = SqliteTime.ToIso(message.CreatedAt),
                    QuickRepliesJson = ChatQuickReplyJson.Serialize(message.QuickReplies)
                }).ConfigureAwait(false);
            }
        }

        // What the companion already learned on this device wins over the backup: only missing keys are filled.
        foreach ((string key, string value) in backup.ChatMemory)
        {
            await run(BackupSql.InsertChatMemoryIfMissing, new { Key = key, Value = value }).ConfigureAwait(false);
        }

        await MergeFavouriteTextsAsync(connection, transaction, backup.FavoriteQuoteTexts, cancellationToken).ConfigureAwait(false);

        int techniques = 0;
        foreach (BackupTechniqueDTO technique in backup.Techniques)
        {
            techniques += await run(BackupSql.InsertTechnique, new
            {
                technique.Number,
                technique.Date,
                technique.Header,
                technique.Description,
                technique.Subject,
                technique.Author,
                technique.Algorithm,
                technique.Image,
                technique.IsCompleted
            }).ConfigureAwait(false);
        }

        // "Latest" is decided by row id, so an older assessment must never be inserted after a newer local one.
        if (backup.LatestRiskAssessment is { } risk)
        {
            await run(BackupSql.InsertRiskAssessmentIfNewest, new
            {
                AssessedAt = SqliteTime.ToIso(risk.AssessedAt),
                risk.Source,
                risk.Notes,
                risk.HasSelfHarmThoughts,
                risk.HasSevereDisorientation,
                risk.HasSubstanceRisk,
                risk.HasSevereInsomnia,
                RiskLevel = risk.RiskLevel.ToString().ToLowerInvariant()
            }).ConfigureAwait(false);
        }

        if (backup.TherapyProgram is { } program)
        {
            await run(BackupSql.InsertProgramIfNoneActive, new
            {
                ProgramKey = program.ProgramType.ToString(),
                StartedAt = SqliteTime.ToIso(program.StartedAt),
                program.CurrentWeek,
                IsActive = program.IsActive ? 1 : 0
            }).ConfigureAwait(false);
        }

        foreach (EscalationEventDTO escalation in backup.Escalations)
        {
            await run(BackupSql.InsertEscalation, new
            {
                CreatedAt = SqliteTime.ToIso(escalation.CreatedAt),
                RiskLevel = escalation.RiskLevel.ToString().ToLowerInvariant(),
                escalation.TriggerSource,
                escalation.Action,
                escalation.Notes
            }).ConfigureAwait(false);
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return new BackupImportResult(moods, tests, completions, sessionResults, chats, SafetyPlanImported: false, techniques);
    }

    private async Task MergeFavouriteTextsAsync(
        SqliteConnection connection,
        SqliteTransaction transaction,
        IReadOnlyList<string> imported,
        CancellationToken cancellationToken)
    {
        if (imported.Count == 0)
        {
            return;
        }

        string? existing = await connection.ExecuteScalarAsync<string?>(DapperCommandFactory.Create(
            "SELECT Value FROM AppMetadata WHERE Key = @key;",
            new { key = FavouriteTextsKey },
            transaction,
            CommandTimeoutSeconds,
            cancellationToken)).ConfigureAwait(false);

        // Same storage format as FavoriteQuoteTextStore: texts joined by newlines, ordinal order.
        HashSet<string> texts = new(StringComparer.Ordinal);
        foreach (string part in (existing ?? string.Empty).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            texts.Add(part);
        }

        int before = texts.Count;
        foreach (string text in imported)
        {
            texts.Add(text.Trim());
        }

        if (texts.Count == before)
        {
            return;
        }

        await connection.ExecuteAsync(DapperCommandFactory.Create(
            """
            INSERT INTO AppMetadata (Key, Value) VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
            """,
            new { key = FavouriteTextsKey, value = string.Join('\n', texts.OrderBy(t => t, StringComparer.Ordinal)) },
            transaction,
            CommandTimeoutSeconds,
            cancellationToken)).ConfigureAwait(false);
    }
}
