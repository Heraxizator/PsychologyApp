using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.DataBackup;

public sealed class AppBackupDTO
{
    public const int CurrentFormatVersion = 1;

    public int FormatVersion { get; init; } = CurrentFormatVersion;
    public DateTime ExportedAtUtc { get; init; }
    public IReadOnlyList<MoodEntryDTO> MoodEntries { get; init; } = [];
    public IReadOnlyList<TestResultDTO> TestResults { get; init; } = [];
    public IReadOnlyList<CompletionDTO> Completions { get; init; } = [];
    public IReadOnlyList<SessionResultDTO> SessionResults { get; init; } = [];
    public IReadOnlyList<BackupChatSessionDTO> ChatSessions { get; init; } = [];
    public SafetyPlanDTO? SafetyPlan { get; init; }
}

public sealed class BackupChatSessionDTO
{
    public ChatSessionDTO Session { get; init; } = new();
    public IReadOnlyList<ChatMessageDTO> Messages { get; init; } = [];
}

public sealed record BackupImportResult(
    int MoodEntries,
    int TestResults,
    int Completions,
    int SessionResults,
    int ChatSessions,
    bool SafetyPlanImported);
