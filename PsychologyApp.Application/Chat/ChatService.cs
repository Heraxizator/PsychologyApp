using System.Collections.Concurrent;
using System.Globalization;
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

    /// <summary>Opens a chat on a topic (for example trauma, from its articles): the companion starts with that topic's choices.</summary>
    Task<ChatTurnResult> StartTopicChatAsync(string topic, CancellationToken cancellationToken = default);

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

    /// <summary>Deletes every chat and what the memory took from them; the name and the practice counters stay.</summary>
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
    IQuotContentProvider quotes,
    IEmotionGuesser? emotionGuesser = null) : IChatService
{
    private const int MaxTitleLength = 60;
    private const int MaxNameLength = 30;
    private const int LowMoodLevel = 2;
    private const int StressTestRelevantDays = 30;

    // A turn is read-modify-write on the chat's state: two at once (a double send, a quick reply racing a follow-up) would each start from
    // the same state and the later save would silently erase the earlier turn. Turns of one chat therefore run one at a time.
    private readonly ConcurrentDictionary<long, SemaphoreSlim> _turnGates = new();
    private readonly SemaphoreSlim _newChatGate = new(1, 1);
    // The session snapshot and cross-chat memory must be read and written with deletes/forgetting as one ordered operation.
    private readonly SemaphoreSlim _memoryGate = new(1, 1);

    public Task<ChatTurnResult> StartNewChatAsync(CancellationToken cancellationToken = default) => StartChatAsync(topic: null, cancellationToken);

    public Task<ChatTurnResult> StartTopicChatAsync(string topic, CancellationToken cancellationToken = default) => StartChatAsync(topic, cancellationToken);

    private async Task<ChatTurnResult> StartChatAsync(string? topic, CancellationToken cancellationToken)
    {
        // "Is there an untouched chat?" then "create one" must not interleave, or two taps make two empty chats.
        await _newChatGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await StartNewChatCoreAsync(topic, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _memoryGate.Release();
            }
        }
        finally
        {
            _newChatGate.Release();
        }
    }

    private async Task<ChatTurnResult> StartNewChatCoreAsync(string? topic, CancellationToken cancellationToken)
    {
        IReadOnlyList<ChatSessionDTO> sessions = await repository.GetSessionsAsync(cancellationToken).ConfigureAwait(false);
        DateTime now = Now();

        ChatSessionDTO? untouched = sessions.FirstOrDefault(s => !s.HasConversation());
        if (untouched is not null && topic is null)
        {
            return new ChatTurnResult(untouched, [], null);
        }

        // An untouched chat holds only the companion's greeting: a chat on a topic replaces it, so the two openings do not mix.
        if (untouched is not null)
        {
            await repository.DeleteSessionAsync(untouched.Id, cancellationToken).ConfigureAwait(false);
        }

        ChatSessionDTO? previous = sessions.FirstOrDefault(s =>
            !string.IsNullOrWhiteSpace(s.Emotion) && s.Emotion != nameof(CompanionEmotion.Unknown) && s.MessageCount >= 2);

        long id = await repository.CreateSessionAsync(DefaultTitle(), now, cancellationToken).ConfigureAwait(false);
        try
        {
            ChatSessionDTO session = (await repository.GetSessionAsync(id, cancellationToken).ConfigureAwait(false))!;
            CompanionDialogue dialogue = await DialogueAsync(cancellationToken, includeTodayMood: true).ConfigureAwait(false);
            IReadOnlyDictionary<string, string> memory = await repository.GetMemoryAsync(cancellationToken).ConfigureAwait(false);
            CompanionState opening = WithMemory(new CompanionState(), memory);
            // A chat on a topic starts with that topic: the step decided last time waits for an ordinary chat.
            CompanionReply reply = topic is not null
                ? dialogue.OpenTopic(opening, topic)
                : dialogue.Open(opening with { OpenPlan = PlanToAskAbout(memory, now) }, previous);
            session.StateJson = reply.State.Serialize();
            IReadOnlyList<ChatMessageDTO> added = await StoreAsync(session, CompanionMessages(session, reply, now), [], cancellationToken).ConfigureAwait(false);

            return new ChatTurnResult(session, added, null);
        }
        catch
        {
            // Session creation precedes greeting generation. Do not leave a blank row that the next open would mistake for a valid chat.
            await repository.DeleteSessionAsync(id, CancellationToken.None).ConfigureAwait(false);
            throw;
        }
    }

    public Task<IReadOnlyList<ChatSessionDTO>> GetChatsAsync(CancellationToken cancellationToken = default) =>
        repository.GetSessionsAsync(cancellationToken);

    public async Task<ChatSessionDTO?> GetLastChatAsync(CancellationToken cancellationToken = default) =>
        (await repository.GetSessionsAsync(cancellationToken).ConfigureAwait(false))
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
        SemaphoreSlim gate = _turnGates.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await CheckPracticeFollowUpCoreAsync(sessionId, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _memoryGate.Release();
            }
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<ChatTurnResult?> CheckPracticeFollowUpCoreAsync(long sessionId, CancellationToken cancellationToken)
    {
        ChatSessionDTO? session = await repository.GetSessionAsync(sessionId, cancellationToken).ConfigureAwait(false);
        if (session is null)
        {
            return null;
        }

        CompanionState state = CompanionState.Deserialize(session.StateJson);
        if (state.PendingPractice is not { } technique || state.PendingPracticeStartedUtc is not { } startedAt)
        {
            return null;
        }

        IReadOnlyList<Models.CompletionDTO> completions = await progress.GetRecentTechniqueCompletionsAsync(20, cancellationToken).ConfigureAwait(false);
        bool completed = completions.Any(c => c.ItemKey == technique && c.CompletedAt.ToUniversalTime() >= startedAt);
        if (!completed)
        {
            return null;
        }

        DateTime now = Now();
        CompanionDialogue dialogue = await DialogueAsync(cancellationToken).ConfigureAwait(false);
        CompanionReply reply = dialogue.FollowUpAfterPractice(state);
        ApplyReply(session, reply, now, firstText: null);
        IReadOnlyList<ChatMessageDTO> added = await StoreAsync(session, CompanionMessages(session, reply, now), [], cancellationToken).ConfigureAwait(false);
        return new ChatTurnResult(session, added, null);
    }

    public async Task RenameAsync(long sessionId, string title, CancellationToken cancellationToken = default)
    {
        SemaphoreSlim gate = _turnGates.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                ChatSessionDTO? session = await repository.GetSessionAsync(sessionId, cancellationToken).ConfigureAwait(false);
                string trimmed = title.Trim();
                if (session is null || trimmed.Length == 0)
                {
                    return;
                }

                session.Title = trimmed.Length > MaxTitleLength ? trimmed[..MaxTitleLength] : trimmed;
                await repository.UpdateSessionAsync(session, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _memoryGate.Release();
            }
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task DeleteAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        SemaphoreSlim gate = _turnGates.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await repository.DeleteSessionAsync(sessionId, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _memoryGate.Release();
            }
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<CompanionProfile> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        // Independent reads. (Database access is queued one operation at a time, so this is for clarity, not speed.)
        Task<IReadOnlyList<ChatSessionDTO>> sessionsTask = repository.GetSessionsAsync(cancellationToken);
        Task<IReadOnlyDictionary<string, string>> memoryTask = repository.GetMemoryAsync(cancellationToken);
        await Task.WhenAll(sessionsTask, memoryTask).ConfigureAwait(false);
        return ChatStatistics.Compute(sessionsTask.Result, memoryTask.Result, Now(), time.LocalTimeZone);
    }

    public async Task SetUserNameAsync(string? name, CancellationToken cancellationToken = default)
    {
        string trimmed = (name ?? string.Empty).Trim();
        await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (trimmed.Length == 0)
            {
                await repository.DeleteMemoryAsync(ChatMemoryKeys.Name, cancellationToken).ConfigureAwait(false);
                return;
            }

            await repository.SetMemoryAsync(ChatMemoryKeys.Name, trimmed.Length > MaxNameLength ? trimmed[..MaxNameLength] : trimmed, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _memoryGate.Release();
        }
    }

    public async Task DeleteAllChatsAsync(CancellationToken cancellationToken = default)
    {
        await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await repository.DeleteAllSessionsAsync(cancellationToken).ConfigureAwait(false);
            IReadOnlyDictionary<string, string> memory = await repository.GetMemoryAsync(cancellationToken).ConfigureAwait(false);
            foreach (string key in memory.Keys.Where(key => !ChatMemoryKeys.OutlivesChats(key)).ToList())
            {
                await repository.DeleteMemoryAsync(key, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _memoryGate.Release();
        }
    }

    public async Task ForgetMemoryAsync(CancellationToken cancellationToken = default)
    {
        await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await repository.ClearMemoryAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _memoryGate.Release();
        }
    }

    /// <summary>What the companion knows from earlier chats: the name and the practice that helped most. Memory wins over the chat's own copy, so a renamed person is renamed everywhere.</summary>
    private static CompanionState WithMemory(CompanionState state, IReadOnlyDictionary<string, string> memory) =>
        state with
        {
            // The memory store is the authority for these cross-chat facts. Falling back to the session copy
            // resurrects a name or preference after the person explicitly forgets it.
            UserName = memory.TryGetValue(ChatMemoryKeys.Name, out string? name) && !string.IsNullOrWhiteSpace(name) ? name : null,
            PreferredPractice = ChatMemoryKeys.MostHelped(memory)?.ToString(),
            TraumaNotes = memory.TryGetValue(ChatMemoryKeys.TraumaNotes, out string? notes) ? notes : null
        };

    /// <summary>
    /// The step decided on in an earlier chat, once it has had time to be tried (from six hours on) and while it is still fresh (two weeks).
    /// </summary>
    private static string? PlanToAskAbout(IReadOnlyDictionary<string, string> memory, DateTime now)
    {
        if (!memory.TryGetValue(ChatMemoryKeys.Plan, out string? plan) || string.IsNullOrWhiteSpace(plan)
            || !memory.TryGetValue(ChatMemoryKeys.PlanAt, out string? at)
            || !DateTime.TryParse(at, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime decided))
        {
            return null;
        }

        TimeSpan age = now - decided.ToUniversalTime();
        return age >= PlanAskAfter && age <= PlanForgetAfter ? plan : null;
    }

    private static readonly TimeSpan HeavyWindow = TimeSpan.FromDays(14);
    private const int HeavyTension = 7;

    private static readonly TimeSpan PlanAskAfter = TimeSpan.FromHours(6);
    private static readonly TimeSpan PlanForgetAfter = TimeSpan.FromDays(14);

    /// <summary>Carries what a turn revealed into the memory that outlives the chat: a new name, a started practice, a practice that helped,
    /// a step decided on (and that it was asked about), the topic of the chat.</summary>
    private List<ChatMemoryChange> MemoryChanges(CompanionState before, CompanionReply reply, IReadOnlyDictionary<string, string> memory, DateTime now)
    {
        List<ChatMemoryChange> changes = [];
        if (reply.State.TopicCounted is { } topic && topic != before.TopicCounted)
        {
            string key = RecurringTopics.MemoryKey(topic);
            changes.Add(new ChatMemoryChange(key, RecurringTopics.Record(memory.GetValueOrDefault(key), now)));
        }

        if (reply.State.NewPlan is { } plan && plan != before.NewPlan)
        {
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.Plan, plan));
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.PlanAt, Now().ToString("O", CultureInfo.InvariantCulture)));
        }
        else if (reply.State.PlanResolved && !before.PlanResolved)
        {
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.Plan));
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.PlanAt));
        }

        if (reply.State.TraumaNotes is { } traumaNotes && traumaNotes != before.TraumaNotes)
        {
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.TraumaNotes, traumaNotes));
        }

        if (reply.State.UserName is { } name && name != before.UserName)
        {
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.Name, name));
        }

        if (reply.Action is { Kind: DialogueActionKind.StartTechnique, TechniqueId: { } started })
        {
            changes.Add(new ChatMemoryChange(ChatMemoryKeys.Tried(started), Increment: true));
        }

        for (int i = before.HelpedPractices.Count; i < reply.State.HelpedPractices.Count; i++)
        {
            if (Enum.TryParse(reply.State.HelpedPractices[i], out TechniqueId helped) && Enum.IsDefined(helped))
            {
                changes.Add(new ChatMemoryChange(ChatMemoryKeys.Helped(helped), Increment: true));
            }
        }

        return changes;
    }

    private async Task<ChatTurnResult> TakeTurnAsync(long sessionId, string userText, CompanionInput input, CancellationToken cancellationToken)
    {
        SemaphoreSlim gate = _turnGates.GetOrAdd(sessionId, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await TakeTurnCoreAsync(sessionId, userText, input, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<ChatTurnResult> TakeTurnCoreAsync(long sessionId, string userText, CompanionInput input, CancellationToken cancellationToken)
    {
        await _memoryGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // The session snapshot must be read under the same gate as delete-all and memory changes. Otherwise a
            // queued send can save its pre-delete snapshot after delete-all has already finished.
            ChatSessionDTO session = await repository.GetSessionAsync(sessionId, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Chat {sessionId} does not exist.");
            if (userText.Length == 0)
            {
                return new ChatTurnResult(session, [], null);
            }

            return await TakeTurnWithMemoryAsync(sessionId, userText, input, session, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _memoryGate.Release();
        }
    }

    private async Task<ChatTurnResult> TakeTurnWithMemoryAsync(
        long sessionId,
        string userText,
        CompanionInput input,
        ChatSessionDTO session,
        CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<string, string> memory = await repository.GetMemoryAsync(cancellationToken).ConfigureAwait(false);
        CompanionState state = WithMemory(CompanionState.Deserialize(session.StateJson), memory);
        DateTime now = Now();

        CompanionDialogue dialogue = await DialogueAsync(cancellationToken, currentSessionId: sessionId, earlierTopicChats: RecurringTopics.EarlierChats(memory, now)).ConfigureAwait(false);
        CompanionReply reply = dialogue.Respond(state, input);
        ApplyReply(session, reply, now, firstText: input is CompanionInput.FreeText ? userText : null);

        // Do not persist a reply that says the mood was logged before the journal write commits. A failed journal
        // write leaves the user's turn unsaved, so the UI can restore the chip and retry the same action.
        if (reply.Action is { Kind: DialogueActionKind.LogMood, MoodLevel: { } moodLevel })
        {
            await progress.RecordMoodAsync(moodLevel, reply.Action.Note, now, cancellationToken).ConfigureAwait(false);
        }

        // The user's message and the whole reply are stored in one transaction: a turn is saved completely or not at all.
        List<ChatMessageDTO> pending =
        [
            new() { SessionId = sessionId, Role = ChatRole.User, Text = userText, CreatedAt = now },
            .. CompanionMessages(session, reply, now)
        ];
        IReadOnlyList<ChatMessageDTO> added = await StoreAsync(session, pending, MemoryChanges(state, reply, memory, now), cancellationToken).ConfigureAwait(false);

        // Logging to the journal is a side effect the companion carries out itself; the UI never sees it as a navigation instruction.
        if (reply.Action?.Kind == DialogueActionKind.LogMood)
        {
            return new ChatTurnResult(session, added, null);
        }
        return new ChatTurnResult(session, added, reply.Action);
    }

    /// <summary>Copies what the turn learned onto the session: state, recognised feeling, tension, title, activity time.</summary>
    private void ApplyReply(ChatSessionDTO session, CompanionReply reply, DateTime now, string? firstText)
    {
        bool emotionWasUnknown = string.IsNullOrWhiteSpace(session.Emotion) || session.Emotion == nameof(CompanionEmotion.Unknown);
        bool themeWasUnknown = string.IsNullOrWhiteSpace(session.Theme);

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
        else if (themeWasUnknown && reply.Theme is { } laterTheme
            && Enum.TryParse(session.Emotion, out CompanionEmotion named) && Enum.IsDefined(named) && named != CompanionEmotion.Unknown
            && session.Title == CompanionDialogueContent.Title(named, null, session.Title, language.IsEnglish))
        {
            // The chat was named by its feeling before its theme was heard ("Тревога"): the theme joins the title, so two
            // such chats can be told apart. A title the person typed is theirs and stays.
            session.Title = CompanionDialogueContent.Title(named, laterTheme, session.Title, language.IsEnglish);
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

    /// <summary>Saves the turn (memory, messages and the session) in one transaction and returns the messages with their ids.</summary>
    private async Task<IReadOnlyList<ChatMessageDTO>> StoreAsync(
        ChatSessionDTO session,
        IEnumerable<ChatMessageDTO> messages,
        IReadOnlyList<ChatMemoryChange> memory,
        CancellationToken cancellationToken)
    {
        List<ChatMessageDTO> pending = messages.ToList();
        IReadOnlyList<long> ids = await repository.SaveTurnAsync(session, pending, memory, cancellationToken).ConfigureAwait(false);
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
    private async Task<CompanionDialogue> DialogueAsync(
        CancellationToken cancellationToken, bool includeTodayMood = false, long? currentSessionId = null, IReadOnlyDictionary<string, int>? earlierTopicChats = null)
    {
        // Heavy days that keep coming back are measured: earlier chats of two weeks that ended at 7 or more, and the questionnaires already taken.
        IReadOnlyList<ChatSessionDTO> sessions = await repository.GetSessionsAsync(cancellationToken).ConfigureAwait(false);
        DateTime since = Now() - HeavyWindow;
        int heavyChats = sessions.Count(s => s.Id != currentSessionId && s.UpdatedAt >= since && (s.LastIntensity ?? s.FirstIntensity) >= HeavyTension);

        Task<IReadOnlyDictionary<string, TestResultDTO>> takenTask = progress.GetLatestTestResultsAsync(["phq9", "gad7", "pcl5"], cancellationToken);
        Task<IReadOnlyList<QuotSeed>> quotesTask = quotes.LoadAllAsync(cancellationToken);
        Task<TestResultDTO?> stressTestTask = progress.GetLatestTestResultAsync(CompanionResourceContent.StressTestId, cancellationToken);
        Task<IReadOnlyList<MoodEntryDTO>> moodsTask = includeTodayMood
            ? progress.GetRecentMoodsAsync(3, cancellationToken)
            : Task.FromResult<IReadOnlyList<MoodEntryDTO>>([]);
        await Task.WhenAll(takenTask, quotesTask, stressTestTask, moodsTask).ConfigureAwait(false);
        HashSet<string> recentlyTaken = [.. takenTask.Result.Where(pair => pair.Value.CompletedAt >= since).Select(pair => pair.Key)];

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
            recentStressTest: stressTest,
            emotionGuesser: emotionGuesser,
            recentHeavyChats: heavyChats,
            recentlyTakenTests: recentlyTaken,
            earlierTopicChats: earlierTopicChats);
    }

    private bool IsLocalToday(DateTime recordedAtUtc, DateTime nowUtc)
    {
        TimeZoneInfo zone = time.LocalTimeZone;
        DateOnly recordedDay = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(recordedAtUtc, DateTimeKind.Utc), zone));
        DateOnly today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc), zone));
        return recordedDay == today;
    }

    private DateTime Now() => time.GetUtcNow().UtcDateTime;

    private string DefaultTitle() => language.IsEnglish ? EnglishDefaultTitle : RussianDefaultTitle;

    private const string EnglishDefaultTitle = "New chat";
    private const string RussianDefaultTitle = "Новый чат";

    /// <summary>The name a chat has until its feeling or the person names it, in either language it was created in.</summary>
    public static bool HasDefaultTitle(string title) => title is EnglishDefaultTitle or RussianDefaultTitle;
}
