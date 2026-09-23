using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Application.Chat;

/// <summary>The language the chat speaks. Provided by the host so Application does not depend on UI settings.</summary>
public interface IChatLanguageProvider
{
    bool IsEnglish { get; }
}

/// <param name="Session">The chat after the turn (updated title, summary, activity time).</param>
/// <param name="NewMessages">Everything appended by this turn, user message first.</param>
/// <param name="Action">Something the UI must do after showing the messages.</param>
public sealed record ChatTurnResult(ChatSessionDTO Session, IReadOnlyList<ChatMessageDTO> NewMessages, DialogueAction? Action);

public interface IChatService
{
    /// <summary>Opens a fresh chat, or the newest one if nothing has been said in it yet (no pile of empty chats).</summary>
    Task<ChatTurnResult> StartNewChatAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatSessionDTO>> GetChatsAsync(CancellationToken cancellationToken = default);

    /// <summary>The most recent chat where something was said, for the home screen.</summary>
    Task<ChatSessionDTO?> GetLastChatAsync(CancellationToken cancellationToken = default);

    Task<ChatSessionDTO?> GetChatAsync(long sessionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(long sessionId, CancellationToken cancellationToken = default);

    Task<ChatTurnResult> SendTextAsync(long sessionId, string text, CancellationToken cancellationToken = default);

    Task<ChatTurnResult> SendQuickReplyAsync(long sessionId, ChatQuickReply reply, CancellationToken cancellationToken = default);

    /// <summary>If a practice started from this chat has been completed since, asks how the person feels now. Null otherwise.</summary>
    Task<ChatTurnResult?> CheckPracticeFollowUpAsync(long sessionId, CancellationToken cancellationToken = default);

    Task RenameAsync(long sessionId, string title, CancellationToken cancellationToken = default);

    Task DeleteAsync(long sessionId, CancellationToken cancellationToken = default);

    /// <summary>Statistics and memory shown on the companion's profile screen.</summary>
    Task<CompanionProfile> GetProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>Sets how the companion addresses the person. A blank name makes it forget the name.</summary>
    Task SetUserNameAsync(string? name, CancellationToken cancellationToken = default);

    Task DeleteAllChatsAsync(CancellationToken cancellationToken = default);

    /// <summary>Forgets the name and which practices helped. Chats are kept.</summary>
    Task ForgetMemoryAsync(CancellationToken cancellationToken = default);
}

public sealed class ChatService(
    IChatRepository repository,
    ISituationAnalyzer analyzer,
    ICrisisDetector crisisDetector,
    IUserProgressService progress,
    IChatLanguageProvider language,
    TimeProvider time,
    IQuotContentProvider quotes) : IChatService
{
    private const int MaxTitleLength = 60;
    private const int MaxNameLength = 30;
    private const int LowMoodLevel = 2;
    private const int StressTestRelevantDays = 30;

    public async Task<ChatTurnResult> StartNewChatAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ChatSessionDTO> sessions = await repository.GetSessionsAsync(cancellationToken);
        DateTime now = Now();

        ChatSessionDTO? untouched = sessions.FirstOrDefault(s => !s.HasConversation());
        if (untouched is not null)
        {
            return new ChatTurnResult(untouched, [], null);
        }

        ChatSessionDTO? previous = sessions.FirstOrDefault(s =>
            !string.IsNullOrWhiteSpace(s.Emotion) && s.Emotion != nameof(CompanionEmotion.Unknown) && s.MessageCount >= 2);

        long id = await repository.CreateSessionAsync(DefaultTitle(), now, cancellationToken);
        ChatSessionDTO session = (await repository.GetSessionAsync(id, cancellationToken))!;

        CompanionDialogue dialogue = await DialogueAsync(cancellationToken, includeTodayMood: true);
        CompanionReply reply = dialogue.Open(await WithMemoryAsync(new CompanionState(), cancellationToken), previous);
        session.StateJson = reply.State.Serialize();
        IReadOnlyList<ChatMessageDTO> added = await StoreAsync(CompanionMessages(session, reply, now), cancellationToken);
        await repository.UpdateSessionAsync(session, cancellationToken);

        return new ChatTurnResult(session, added, null);
    }

    public Task<IReadOnlyList<ChatSessionDTO>> GetChatsAsync(CancellationToken cancellationToken = default) =>
        repository.GetSessionsAsync(cancellationToken);

    public async Task<ChatSessionDTO?> GetLastChatAsync(CancellationToken cancellationToken = default) =>
        (await repository.GetSessionsAsync(cancellationToken))
            .FirstOrDefault(s => s.HasConversation());

    public Task<ChatSessionDTO?> GetChatAsync(long sessionId, CancellationToken cancellationToken = default) =>
        repository.GetSessionAsync(sessionId, cancellationToken);

    public Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(long sessionId, CancellationToken cancellationToken = default) =>
        repository.GetMessagesAsync(sessionId, cancellationToken);

    public Task<ChatTurnResult> SendTextAsync(long sessionId, string text, CancellationToken cancellationToken = default)
    {
        string capitalized = ChatText.Capitalize(text);
        return TakeTurnAsync(sessionId, capitalized, new CompanionInput.FreeText(capitalized), cancellationToken);
    }

    public Task<ChatTurnResult> SendQuickReplyAsync(long sessionId, ChatQuickReply reply, CancellationToken cancellationToken = default) =>
        TakeTurnAsync(sessionId, reply.Label, new CompanionInput.QuickReply(reply), cancellationToken);

    public async Task<ChatTurnResult?> CheckPracticeFollowUpAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        ChatSessionDTO? session = await repository.GetSessionAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return null;
        }

        CompanionState state = CompanionState.Deserialize(session.StateJson);
        if (state.PendingPractice is not { } technique || state.PendingPracticeStartedUtc is not { } startedAt)
        {
            return null;
        }

        IReadOnlyList<Models.CompletionDTO> completions = await progress.GetRecentTechniqueCompletionsAsync(20, cancellationToken);
        bool completed = completions.Any(c => c.ItemKey == technique && c.CompletedAt.ToUniversalTime() >= startedAt);
        if (!completed)
        {
            return null;
        }

        DateTime now = Now();
        CompanionDialogue dialogue = await DialogueAsync(cancellationToken);
        CompanionReply reply = dialogue.FollowUpAfterPractice(state);
        ApplyReply(session, reply, now, firstText: null);
        IReadOnlyList<ChatMessageDTO> added = await StoreAsync(CompanionMessages(session, reply, now), cancellationToken);
        await repository.UpdateSessionAsync(session, cancellationToken);
        return new ChatTurnResult(session, added, null);
    }

    public async Task RenameAsync(long sessionId, string title, CancellationToken cancellationToken = default)
    {
        ChatSessionDTO? session = await repository.GetSessionAsync(sessionId, cancellationToken);
        string trimmed = title.Trim();
        if (session is null || trimmed.Length == 0)
        {
            return;
        }

        session.Title = trimmed.Length > MaxTitleLength ? trimmed[..MaxTitleLength] : trimmed;
        await repository.UpdateSessionAsync(session, cancellationToken);
    }

    public Task DeleteAsync(long sessionId, CancellationToken cancellationToken = default) =>
        repository.DeleteSessionAsync(sessionId, cancellationToken);

    public async Task<CompanionProfile> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        // Independent reads: run them together instead of waiting for one, then the other.
        Task<IReadOnlyList<ChatSessionDTO>> sessionsTask = repository.GetSessionsAsync(cancellationToken);
        Task<IReadOnlyDictionary<string, string>> memoryTask = repository.GetMemoryAsync(cancellationToken);
        await Task.WhenAll(sessionsTask, memoryTask);
        return ChatStatistics.Compute(sessionsTask.Result, memoryTask.Result, Now(), time.LocalTimeZone);
    }

    public async Task SetUserNameAsync(string? name, CancellationToken cancellationToken = default)
    {
        string trimmed = (name ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            await repository.DeleteMemoryAsync(ChatMemoryKeys.Name, cancellationToken);
            return;
        }

        await repository.SetMemoryAsync(ChatMemoryKeys.Name, trimmed.Length > MaxNameLength ? trimmed[..MaxNameLength] : trimmed, cancellationToken);
    }

    public Task DeleteAllChatsAsync(CancellationToken cancellationToken = default) =>
        repository.DeleteAllSessionsAsync(cancellationToken);

    public Task ForgetMemoryAsync(CancellationToken cancellationToken = default) =>
        repository.ClearMemoryAsync(cancellationToken);

    /// <summary>What the companion knows from earlier chats: the name and the practice that helped most. Memory wins over the chat's own copy, so a renamed person is renamed everywhere.</summary>
    private async Task<CompanionState> WithMemoryAsync(CompanionState state, CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<string, string> memory = await repository.GetMemoryAsync(cancellationToken);
        return state with
        {
            UserName = memory.TryGetValue(ChatMemoryKeys.Name, out string? name) && !string.IsNullOrWhiteSpace(name) ? name : state.UserName,
            PreferredPractice = ChatMemoryKeys.MostHelped(memory)?.ToString() ?? state.PreferredPractice
        };
    }

    /// <summary>Carries what a turn revealed into the memory that outlives the chat: a new name, a started practice, a practice that helped.</summary>
    private async Task RememberAsync(CompanionState before, CompanionReply reply, CancellationToken cancellationToken)
    {
        if (reply.State.UserName is { } name && name != before.UserName)
        {
            await repository.SetMemoryAsync(ChatMemoryKeys.Name, name, cancellationToken);
        }

        if (reply.Action is { Kind: DialogueActionKind.StartTechnique, TechniqueId: { } started })
        {
            await repository.IncrementMemoryAsync(ChatMemoryKeys.Tried(started), cancellationToken);
        }

        for (int i = before.HelpedPractices.Count; i < reply.State.HelpedPractices.Count; i++)
        {
            if (Enum.TryParse(reply.State.HelpedPractices[i], out TechniqueId helped))
            {
                await repository.IncrementMemoryAsync(ChatMemoryKeys.Helped(helped), cancellationToken);
            }
        }
    }

    private async Task<ChatTurnResult> TakeTurnAsync(long sessionId, string userText, CompanionInput input, CancellationToken cancellationToken)
    {
        ChatSessionDTO session = await repository.GetSessionAsync(sessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Chat {sessionId} does not exist.");
        if (userText.Length == 0)
        {
            return new ChatTurnResult(session, [], null);
        }

        CompanionState state = await WithMemoryAsync(CompanionState.Deserialize(session.StateJson), cancellationToken);
        DateTime now = Now();

        CompanionDialogue dialogue = await DialogueAsync(cancellationToken);
        CompanionReply reply = dialogue.Respond(state, input);
        await RememberAsync(state, reply, cancellationToken);
        ApplyReply(session, reply, now, firstText: input is CompanionInput.FreeText ? userText : null);

        // The user's message and the whole reply are stored in one transaction: a turn is saved completely or not at all.
        List<ChatMessageDTO> pending =
        [
            new() { SessionId = sessionId, Role = ChatRole.User, Text = userText, CreatedAt = now },
            .. CompanionMessages(session, reply, now)
        ];
        IReadOnlyList<ChatMessageDTO> added = await StoreAsync(pending, cancellationToken);
        await repository.UpdateSessionAsync(session, cancellationToken);

        // Logging to the journal is a side effect the companion carries out itself; the UI never sees it as a navigation instruction.
        if (reply.Action is { Kind: DialogueActionKind.LogMood, MoodLevel: { } moodLevel })
        {
            await progress.RecordMoodAsync(moodLevel, reply.Action.Note, now, cancellationToken);
            return new ChatTurnResult(session, added, null);
        }

        return new ChatTurnResult(session, added, reply.Action);
    }

    /// <summary>Copies what the turn learned onto the session: state, recognised feeling, tension, title, activity time.</summary>
    private void ApplyReply(ChatSessionDTO session, CompanionReply reply, DateTime now, string? firstText)
    {
        bool emotionWasUnknown = string.IsNullOrWhiteSpace(session.Emotion) || session.Emotion == nameof(CompanionEmotion.Unknown);

        session.StateJson = reply.State.Serialize();
        session.UpdatedAt = now;
        session.FirstIntensity = reply.State.FirstIntensity;
        session.LastIntensity = reply.State.LastIntensity;
        if (reply.Emotion != CompanionEmotion.Unknown)
        {
            session.Emotion = reply.Emotion.ToString();
        }

        if (reply.Theme is { } theme)
        {
            session.Theme = theme.ToString();
        }

        bool becameKnown = emotionWasUnknown && reply.Emotion != CompanionEmotion.Unknown;
        bool firstFreeText = firstText is not null && reply.State.RecentTexts.Count == 1;
        if (firstFreeText || becameKnown)
        {
            string source = firstText ?? reply.State.RecentTexts.FirstOrDefault() ?? session.Title;
            session.Title = CompanionDialogueContent.Title(reply.Emotion, reply.Theme, source, language.IsEnglish);
        }
    }

    /// <summary>The companion's messages for a reply. Quick replies belong to the last one.</summary>
    private static IEnumerable<ChatMessageDTO> CompanionMessages(ChatSessionDTO session, CompanionReply reply, DateTime now) =>
        reply.Messages.Select((text, i) => new ChatMessageDTO
        {
            SessionId = session.Id,
            Role = ChatRole.Companion,
            Text = text,
            CreatedAt = now,
            QuickReplies = i == reply.Messages.Count - 1 ? reply.QuickReplies : []
        });

    /// <summary>Saves the messages in one transaction and returns them with their ids.</summary>
    private async Task<IReadOnlyList<ChatMessageDTO>> StoreAsync(IEnumerable<ChatMessageDTO> messages, CancellationToken cancellationToken)
    {
        List<ChatMessageDTO> pending = messages.ToList();
        if (pending.Count == 0)
        {
            return pending;
        }

        IReadOnlyList<long> ids = await repository.AddMessagesAsync(pending, cancellationToken);
        return pending.Select((m, i) => new ChatMessageDTO
        {
            Id = ids[i],
            SessionId = m.SessionId,
            Role = m.Role,
            Text = m.Text,
            CreatedAt = m.CreatedAt,
            QuickReplies = m.QuickReplies
        }).ToList();
    }

    /// <summary>
    /// Builds a dialogue engine with everything it might reference ready in advance: the quote catalog (cached after the
    /// first load), a recent stress self-assessment if there is one, and — only when opening a chat, since nowhere else
    /// needs it — today's journal entry if it was a low one.
    /// </summary>
    private async Task<CompanionDialogue> DialogueAsync(CancellationToken cancellationToken, bool includeTodayMood = false)
    {
        Task<IReadOnlyList<QuotSeed>> quotesTask = quotes.LoadAllAsync(cancellationToken);
        Task<TestResultDTO?> stressTestTask = progress.GetLatestTestResultAsync(CompanionResourceContent.StressTestId, cancellationToken);
        Task<IReadOnlyList<MoodEntryDTO>> moodsTask = includeTodayMood
            ? progress.GetRecentMoodsAsync(3, cancellationToken)
            : Task.FromResult<IReadOnlyList<MoodEntryDTO>>([]);
        await Task.WhenAll(quotesTask, stressTestTask, moodsTask);

        DateTime now = Now();
        TestResultDTO? stressTest = stressTestTask.Result is { } result && (now - result.CompletedAt).TotalDays <= StressTestRelevantDays
            ? result
            : null;
        MoodEntryDTO? todayLowMood = moodsTask.Result.FirstOrDefault(m => m.MoodLevel <= LowMoodLevel && IsLocalToday(m.RecordedAt, now));

        return new(
            analyzer,
            crisisDetector,
            language.IsEnglish,
            time: time,
            quotes: quotesTask.Result,
            todayLowMood: todayLowMood,
            recentStressTest: stressTest);
    }

    private bool IsLocalToday(DateTime recordedAtUtc, DateTime nowUtc)
    {
        TimeZoneInfo zone = time.LocalTimeZone;
        DateOnly recordedDay = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(recordedAtUtc, DateTimeKind.Utc), zone));
        DateOnly today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc), zone));
        return recordedDay == today;
    }

    private DateTime Now() => time.GetUtcNow().UtcDateTime;

    private string DefaultTitle() => language.IsEnglish ? "New chat" : "Новый чат";
}
