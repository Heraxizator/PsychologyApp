using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Models;
using System.Text.Json;

namespace PsychologyApp.Application.DataBackup;

public sealed class BackupService(
    IUserProgressRepository progress,
    IChatRepository chatRepository,
    IClinicalCareRepository clinicalCareRepository) : IBackupService
{
    private const int ExportLimit = 100_000;

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public async Task<string> ExportAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> moods = await progress.GetMoodsAsync(null, null, ExportLimit, cancellationToken);
        IReadOnlyList<TestResultDTO> testResults = await progress.GetAllTestResultsAsync(ExportLimit, cancellationToken);
        IReadOnlyList<CompletionDTO> completions = await progress.GetRecentTechniqueCompletionsAsync(ExportLimit, cancellationToken);
        IReadOnlyList<SessionResultDTO> sessionResults = await progress.GetRecentSessionResultsAsync(ExportLimit, cancellationToken);
        SafetyPlanDTO? safetyPlan = await clinicalCareRepository.GetSafetyPlanAsync(cancellationToken);

        IReadOnlyList<ChatSessionDTO> sessions = await chatRepository.GetSessionsAsync(cancellationToken);
        List<BackupChatSessionDTO> chatSessions = new(sessions.Count);
        foreach (ChatSessionDTO session in sessions)
        {
            IReadOnlyList<ChatMessageDTO> messages = await chatRepository.GetMessagesAsync(session.Id, cancellationToken);
            chatSessions.Add(new BackupChatSessionDTO { Session = session, Messages = messages });
        }

        AppBackupDTO backup = new()
        {
            ExportedAtUtc = DateTime.UtcNow,
            MoodEntries = moods,
            TestResults = testResults,
            Completions = completions,
            SessionResults = sessionResults,
            ChatSessions = chatSessions,
            SafetyPlan = safetyPlan is null || safetyPlan.IsEmpty ? null : safetyPlan
        };

        return JsonSerializer.Serialize(backup, SerializerOptions);
    }

    public async Task<BackupImportResult> ImportAsync(string json, CancellationToken cancellationToken = default)
    {
        AppBackupDTO backup = JsonSerializer.Deserialize<AppBackupDTO>(json)
            ?? throw new InvalidOperationException("The backup file could not be read.");

        foreach (MoodEntryDTO mood in backup.MoodEntries)
        {
            await progress.RecordMoodAsync(mood, cancellationToken);
        }

        foreach (TestResultDTO result in backup.TestResults)
        {
            await progress.SaveTestResultAsync(result, cancellationToken);
        }

        foreach (CompletionDTO completion in backup.Completions)
        {
            await progress.RecordCompletionAsync(completion, cancellationToken);
        }

        foreach (SessionResultDTO sessionResult in backup.SessionResults)
        {
            long newId = await progress.RecordSessionOutcomeAsync(
                new SessionOutcomeRequest
                {
                    ItemKey = sessionResult.ItemKey,
                    ModuleName = "restored",
                    PageName = "restored",
                    DurationSeconds = sessionResult.DurationSeconds,
                    PayloadJson = sessionResult.PayloadJson,
                    PreIntensity = sessionResult.PreIntensity,
                    ProgramType = sessionResult.ProgramType,
                    ProgramWeek = sessionResult.ProgramWeek,
                    DeleteDraft = false
                },
                cancellationToken);

            if (sessionResult.PostIntensity is int post)
            {
                await progress.UpdateSessionResultPostIntensityAsync(newId, post, cancellationToken);
            }
        }

        foreach (BackupChatSessionDTO chatSession in backup.ChatSessions)
        {
            long newSessionId = await chatRepository.CreateSessionAsync(
                chatSession.Session.Title,
                chatSession.Session.CreatedAt,
                cancellationToken);

            if (chatSession.Messages.Count > 0)
            {
                List<ChatMessageDTO> messages = chatSession.Messages
                    .Select(message => new ChatMessageDTO
                    {
                        SessionId = newSessionId,
                        Role = message.Role,
                        Text = message.Text,
                        CreatedAt = message.CreatedAt,
                        QuickReplies = message.QuickReplies
                    })
                    .ToList();
                await chatRepository.AddMessagesAsync(messages, cancellationToken);
            }

            await chatRepository.UpdateSessionAsync(
                new ChatSessionDTO
                {
                    Id = newSessionId,
                    Title = chatSession.Session.Title,
                    UpdatedAt = chatSession.Session.UpdatedAt,
                    Emotion = chatSession.Session.Emotion,
                    Theme = chatSession.Session.Theme,
                    FirstIntensity = chatSession.Session.FirstIntensity,
                    LastIntensity = chatSession.Session.LastIntensity,
                    StateJson = chatSession.Session.StateJson
                },
                cancellationToken);
        }

        bool safetyPlanImported = false;
        if (backup.SafetyPlan is { IsEmpty: false } safetyPlan)
        {
            await clinicalCareRepository.SaveSafetyPlanAsync(safetyPlan, cancellationToken);
            safetyPlanImported = true;
        }

        return new BackupImportResult(
            backup.MoodEntries.Count,
            backup.TestResults.Count,
            backup.Completions.Count,
            backup.SessionResults.Count,
            backup.ChatSessions.Count,
            safetyPlanImported);
    }
}
