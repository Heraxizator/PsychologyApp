using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
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
}

public sealed class ChatService(
    IChatRepository repository,
    ISituationAnalyzer analyzer,
    ICrisisDetector crisisDetector,
    IUserProgressService progress,
    IChatLanguageProvider language,
    TimeProvider time) : IChatService
{
    private const int MaxTitleLength = 60;

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

        CompanionReply reply = Dialogue().Open(new CompanionState(), previous);
        session.StateJson = reply.State.Serialize();
        List<ChatMessageDTO> added = await AppendCompanionMessagesAsync(session, reply, now, cancellationToken);
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

    public Task<ChatTurnResult> SendTextAsync(long sessionId, string text, CancellationToken cancellationToken = default) =>
        TakeTurnAsync(sessionId, text.Trim(), new CompanionInput.FreeText(text), cancellationToken);

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
        CompanionReply reply = Dialogue().FollowUpAfterPractice(state);
        ApplyReply(session, reply, now, firstText: null);
        List<ChatMessageDTO> added = await AppendCompanionMessagesAsync(session, reply, now, cancellationToken);
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

    private async Task<ChatTurnResult> TakeTurnAsync(long sessionId, string userText, CompanionInput input, CancellationToken cancellationToken)
    {
        ChatSessionDTO session = await repository.GetSessionAsync(sessionId, cancellationToken)
            ?? throw new InvalidOperationException($"Chat {sessionId} does not exist.");
        if (userText.Length == 0)
        {
            return new ChatTurnResult(session, [], null);
        }

        CompanionState state = CompanionState.Deserialize(session.StateJson);
        DateTime now = Now();

        List<ChatMessageDTO> added = [];
        ChatMessageDTO user = new() { SessionId = sessionId, Role = ChatRole.User, Text = userText, CreatedAt = now };
        added.Add(WithId(user, await repository.AddMessageAsync(user, cancellationToken)));

        CompanionReply reply = Dialogue().Respond(state, input);
        ApplyReply(session, reply, now, firstText: input is CompanionInput.FreeText ? userText : null);
        added.AddRange(await AppendCompanionMessagesAsync(session, reply, now, cancellationToken));
        await repository.UpdateSessionAsync(session, cancellationToken);

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

    private async Task<List<ChatMessageDTO>> AppendCompanionMessagesAsync(
        ChatSessionDTO session, CompanionReply reply, DateTime now, CancellationToken cancellationToken)
    {
        List<ChatMessageDTO> added = [];
        for (int i = 0; i < reply.Messages.Count; i++)
        {
            bool last = i == reply.Messages.Count - 1;
            ChatMessageDTO message = new()
            {
                SessionId = session.Id,
                Role = ChatRole.Companion,
                Text = reply.Messages[i],
                CreatedAt = now,
                QuickReplies = last ? reply.QuickReplies : []
            };
            added.Add(WithId(message, await repository.AddMessageAsync(message, cancellationToken)));
        }

        return added;
    }

    private static ChatMessageDTO WithId(ChatMessageDTO message, long id) => new()
    {
        Id = id,
        SessionId = message.SessionId,
        Role = message.Role,
        Text = message.Text,
        CreatedAt = message.CreatedAt,
        QuickReplies = message.QuickReplies
    };

    private CompanionDialogue Dialogue() => new(analyzer, crisisDetector, language.IsEnglish, time: time);

    private DateTime Now() => time.GetUtcNow().UtcDateTime;

    private string DefaultTitle() => language.IsEnglish ? "New chat" : "Новый чат";
}
