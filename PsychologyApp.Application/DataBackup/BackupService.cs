using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Models;
using System.Text.Json;

namespace PsychologyApp.Application.DataBackup;

public sealed class BackupService(
    IUserProgressRepository progress,
    IChatRepository chatRepository,
    IClinicalCareRepository clinicalCareRepository,
    IFavoriteQuoteTextStore favoriteQuoteTextStore,
    IBackupRepository backupRepository) : IBackupService
{
    private const int ExportLimit = 100_000;

    /// <summary>A real backup is a few MB; this only stops a wrong file (a video, a database) from being read into memory.</summary>
    internal const int MaxImportCharacters = 64 * 1024 * 1024;

    private const int MaxEscalations = 1_000;

    public async Task<string> ExportAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> moods = await progress.GetMoodsAsync(null, null, ExportLimit, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<TestResultDTO> testResults = await progress.GetAllTestResultsAsync(ExportLimit, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<CompletionDTO> completions = await progress.GetRecentTechniqueCompletionsAsync(ExportLimit, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<SessionResultDTO> sessionResults = await progress.GetRecentSessionResultsAsync(ExportLimit, cancellationToken).ConfigureAwait(false);
        SafetyPlanDTO? safetyPlan = await clinicalCareRepository.GetSafetyPlanAsync(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<ChatSessionDTO> sessions = await chatRepository.GetSessionsAsync(cancellationToken).ConfigureAwait(false);
        List<BackupChatSessionDTO> chatSessions = new(sessions.Count);
        foreach (ChatSessionDTO session in sessions)
        {
            IReadOnlyList<ChatMessageDTO> messages = await chatRepository.GetMessagesAsync(session.Id, cancellationToken).ConfigureAwait(false);
            chatSessions.Add(new BackupChatSessionDTO { Session = session, Messages = messages });
        }

        IReadOnlySet<string> favourites = await favoriteQuoteTextStore.GetTextsAsync(cancellationToken).ConfigureAwait(false);

        AppBackupDTO backup = new()
        {
            ExportedAtUtc = DateTime.UtcNow,
            MoodEntries = moods,
            TestResults = testResults,
            Completions = completions,
            SessionResults = sessionResults,
            ChatSessions = chatSessions,
            SafetyPlan = safetyPlan is null || safetyPlan.IsEmpty ? null : safetyPlan,
            ChatMemory = await chatRepository.GetMemoryAsync(cancellationToken).ConfigureAwait(false),
            FavoriteQuoteTexts = favourites.OrderBy(text => text, StringComparer.Ordinal).ToList(),
            Techniques = await backupRepository.GetTechniquesAsync(cancellationToken).ConfigureAwait(false),
            LatestRiskAssessment = await clinicalCareRepository.GetLatestRiskAssessmentAsync(cancellationToken).ConfigureAwait(false),
            TherapyProgram = await clinicalCareRepository.GetActiveProgramAsync(cancellationToken).ConfigureAwait(false),
            Escalations = await clinicalCareRepository.GetRecentEscalationsAsync(MaxEscalations, cancellationToken).ConfigureAwait(false)
        };

        return JsonSerializer.Serialize(backup, BackupJsonContext.Default.AppBackupDTO);
    }

    public async Task<BackupImportResult> ImportAsync(string json, CancellationToken cancellationToken = default)
    {
        AppBackupDTO backup = Parse(json);
        int sourceRows = backup.MoodEntries.Count + backup.TestResults.Count + backup.Completions.Count
            + backup.SessionResults.Count + backup.ChatSessions.Count + backup.Techniques.Count;

        AppBackupDTO clean = Sanitize(backup);
        BackupImportResult result = await backupRepository.ImportAsync(clean, cancellationToken).ConfigureAwait(false);

        bool safetyPlanImported = await ImportSafetyPlanAsync(clean.SafetyPlan, cancellationToken).ConfigureAwait(false);
        int added = result.MoodEntries + result.TestResults + result.Completions + result.SessionResults + result.ChatSessions + result.Techniques;
        return result with
        {
            SafetyPlanImported = safetyPlanImported,
            SkippedDuplicates = Math.Max(0, sourceRows - added)
        };
    }

    private static AppBackupDTO Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new BackupFormatException("The backup file is empty.");
        }

        if (json.Length > MaxImportCharacters)
        {
            throw new BackupFormatException("The file is too large to be a backup.");
        }

        AppBackupDTO? backup;
        try
        {
            backup = JsonSerializer.Deserialize(json, BackupJsonContext.Default.AppBackupDTO);
        }
        catch (JsonException ex)
        {
            throw new BackupFormatException("The file is not a valid backup.", ex);
        }

        if (backup is null)
        {
            throw new BackupFormatException("The backup file could not be read.");
        }

        if (backup.FormatVersion is < AppBackupDTO.OldestSupportedFormatVersion or > AppBackupDTO.CurrentFormatVersion)
        {
            throw new BackupFormatException(
                $"The backup format {backup.FormatVersion} is not supported (this version reads {AppBackupDTO.OldestSupportedFormatVersion}-{AppBackupDTO.CurrentFormatVersion}).");
        }

        return backup;
    }

    /// <summary>Drops rows a hand-edited or damaged file could not have come from the app with (no key, no date, impossible role).</summary>
    private static AppBackupDTO Sanitize(AppBackupDTO backup) => new()
    {
        FormatVersion = backup.FormatVersion,
        ExportedAtUtc = backup.ExportedAtUtc,
        MoodEntries = backup.MoodEntries.Where(m => m.RecordedAt != default).ToList(),
        TestResults = backup.TestResults.Where(t => !string.IsNullOrWhiteSpace(t.TestId) && t.CompletedAt != default).ToList(),
        Completions = backup.Completions.Where(c => !string.IsNullOrWhiteSpace(c.ItemKey) && c.CompletedAt != default).ToList(),
        SessionResults = backup.SessionResults.Where(s => !string.IsNullOrWhiteSpace(s.ItemKey) && s.CompletedAt != default).ToList(),
        ChatSessions = backup.ChatSessions
            .Select(chat => new BackupChatSessionDTO
            {
                Session = chat.Session,
                Messages = chat.Messages.Where(m => Enum.IsDefined(m.Role) && m.CreatedAt != default).ToList()
            })
            .Where(chat => chat.Session.CreatedAt != default)
            .ToList(),
        SafetyPlan = backup.SafetyPlan,
        ChatMemory = backup.ChatMemory
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Key) && pair.Value is not null)
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal),
        FavoriteQuoteTexts = backup.FavoriteQuoteTexts.Where(text => !string.IsNullOrWhiteSpace(text)).ToList(),
        Techniques = backup.Techniques
            .Where(t => !string.IsNullOrWhiteSpace(t.Header) && !string.IsNullOrWhiteSpace(t.Algorithm))
            .ToList(),
        LatestRiskAssessment = backup.LatestRiskAssessment,
        TherapyProgram = backup.TherapyProgram,
        Escalations = backup.Escalations.Where(e => e.CreatedAt != default).ToList()
    };

    /// <summary>A safety plan is one document: take the backup's only if there is none yet or it is the newer edit.</summary>
    private async Task<bool> ImportSafetyPlanAsync(SafetyPlanDTO? fromBackup, CancellationToken cancellationToken)
    {
        if (fromBackup is null || fromBackup.IsEmpty)
        {
            return false;
        }

        SafetyPlanDTO? local = await clinicalCareRepository.GetSafetyPlanAsync(cancellationToken).ConfigureAwait(false);
        bool localIsNewer = local is { IsEmpty: false }
            && (fromBackup.UpdatedAt is null || (local.UpdatedAt is { } localAt && localAt >= fromBackup.UpdatedAt));
        if (localIsNewer)
        {
            return false;
        }

        await clinicalCareRepository.SaveSafetyPlanAsync(fromBackup, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
