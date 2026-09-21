using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class ChatServiceTests
{
    private sealed class InMemoryChatRepository : IChatRepository
    {
        private readonly Dictionary<long, ChatSessionDTO> _sessions = [];
        private readonly List<ChatMessageDTO> _messages = [];
        private long _sessionId;
        private long _messageId;

        public Task<long> CreateSessionAsync(string title, DateTime nowUtc, CancellationToken cancellationToken = default)
        {
            long id = ++_sessionId;
            _sessions[id] = new ChatSessionDTO { Id = id, Title = title, CreatedAt = nowUtc, UpdatedAt = nowUtc };
            return Task.FromResult(id);
        }

        public Task<IReadOnlyList<ChatSessionDTO>> GetSessionsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ChatSessionDTO>>(_sessions.Values.OrderByDescending(s => s.UpdatedAt).ThenByDescending(s => s.Id).Select(Snapshot).ToList());

        public Task<ChatSessionDTO?> GetSessionAsync(long sessionId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_sessions.TryGetValue(sessionId, out ChatSessionDTO? s) ? Snapshot(s) : null);

        public Task UpdateSessionAsync(ChatSessionDTO session, CancellationToken cancellationToken = default)
        {
            _sessions[session.Id] = Snapshot(session);
            return Task.CompletedTask;
        }

        public Task DeleteSessionAsync(long sessionId, CancellationToken cancellationToken = default)
        {
            _sessions.Remove(sessionId);
            _messages.RemoveAll(m => m.SessionId == sessionId);
            return Task.CompletedTask;
        }

        public Task<long> AddMessageAsync(ChatMessageDTO message, CancellationToken cancellationToken = default)
        {
            long id = ++_messageId;
            _messages.Add(new ChatMessageDTO { Id = id, SessionId = message.SessionId, Role = message.Role, Text = message.Text, CreatedAt = message.CreatedAt, QuickReplies = message.QuickReplies });
            return Task.FromResult(id);
        }

        public async Task<IReadOnlyList<long>> AddMessagesAsync(IReadOnlyList<ChatMessageDTO> messages, CancellationToken cancellationToken = default)
        {
            List<long> ids = [];
            foreach (ChatMessageDTO message in messages)
            {
                ids.Add(await AddMessageAsync(message, cancellationToken));
            }

            return ids;
        }

        public Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(long sessionId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ChatMessageDTO>>(_messages.Where(m => m.SessionId == sessionId).ToList());

        private ChatSessionDTO Snapshot(ChatSessionDTO s) => new()
        {
            Id = s.Id, Title = s.Title, CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt, Emotion = s.Emotion, Theme = s.Theme,
            FirstIntensity = s.FirstIntensity, LastIntensity = s.LastIntensity, StateJson = s.StateJson,
            Preview = _messages.LastOrDefault(m => m.SessionId == s.Id)?.Text,
            MessageCount = _messages.Count(m => m.SessionId == s.Id)
        };
    }

    private sealed class Clock(DateTime start) : TimeProvider
    {
        public DateTime Utc { get; set; } = start;

        public override DateTimeOffset GetUtcNow() => new(Utc, TimeSpan.Zero);
    }

    private sealed class Language(bool english = false) : IChatLanguageProvider
    {
        public bool IsEnglish { get; } = english;
    }

    private readonly InMemoryChatRepository _repository = new();
    private readonly Clock _clock = new(new DateTime(2026, 9, 21, 12, 0, 0, DateTimeKind.Utc));
    private readonly Mock<IUserProgressService> _progress = new();

    private ChatService CreateService(bool english = false)
    {
        _progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        return new ChatService(_repository, new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), _progress.Object, new Language(english), _clock);
    }

    [Fact]
    public async Task New_chat_is_created_with_a_persisted_greeting()
    {
        ChatService service = CreateService();

        ChatTurnResult result = await service.StartNewChatAsync();

        Assert.Equal("Новый чат", result.Session.Title);
        Assert.Equal(2, result.NewMessages.Count);
        IReadOnlyList<ChatMessageDTO> stored = await service.GetMessagesAsync(result.Session.Id);
        Assert.Equal(2, stored.Count);
        Assert.All(stored, m => Assert.Equal(ChatRole.Companion, m.Role));
    }

    [Fact]
    public async Task Starting_a_chat_twice_reuses_the_one_where_nothing_was_said()
    {
        ChatService service = CreateService();

        long first = (await service.StartNewChatAsync()).Session.Id;
        ChatTurnResult second = await service.StartNewChatAsync();

        Assert.Equal(first, second.Session.Id);
        Assert.Empty(second.NewMessages);
        Assert.Single(await service.GetChatsAsync());
    }

    [Fact]
    public async Task A_message_is_stored_with_the_reply_and_names_and_summarises_the_chat()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        _clock.Utc = _clock.Utc.AddMinutes(1);

        ChatTurnResult result = await service.SendTextAsync(id, "Завтра важная презентация, я очень тревожусь на работе");

        Assert.Equal(ChatRole.User, result.NewMessages[0].Role);
        Assert.True(result.NewMessages.Count >= 3);
        Assert.Equal("Тревога · работа", result.Session.Title);
        Assert.Equal("Anxiety", result.Session.Emotion);
        Assert.Equal("Work", result.Session.Theme);
        Assert.Equal(_clock.Utc, result.Session.UpdatedAt);
        ChatSessionDTO stored = (await service.GetChatAsync(id))!;
        Assert.Equal(1, CompanionState.Deserialize(stored.StateJson).Turns);
    }

    [Fact]
    public async Task The_dialogue_continues_from_the_stored_state_after_a_restart()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Меня бесит начальник, так злюсь");

        ChatService restarted = CreateService();
        ChatTurnResult second = await restarted.SendTextAsync(id, "Он опять раскритиковал мой отчёт при всех");

        Assert.Contains(second.NewMessages, m => m.QuickReplies.Count == 11);
        Assert.Equal(2, CompanionState.Deserialize(second.Session.StateJson).Turns);
    }

    [Fact]
    public async Task Quick_replies_are_stored_only_on_the_last_message_of_a_turn()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;

        ChatTurnResult result = await service.SendTextAsync(id, "Паническая атака, не могу дышать, сердце колотится");

        List<ChatMessageDTO> companion = result.NewMessages.Where(m => m.Role == ChatRole.Companion).ToList();
        Assert.NotEmpty(companion[^1].QuickReplies);
        Assert.All(companion.Take(companion.Count - 1), m => Assert.Empty(m.QuickReplies));
    }

    [Fact]
    public async Task Choosing_a_practice_returns_the_action_and_stores_the_label_as_the_users_message()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Паническая атака, не могу дышать, сердце колотится");

        ChatTurnResult result = await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Practice, "Начнём: Заземление 5-4-3-2-1", "Grounding"));

        Assert.Equal(new DialogueAction(DialogueActionKind.StartTechnique, TechniqueId.Grounding), result.Action);
        Assert.Equal("Начнём: Заземление 5-4-3-2-1", result.NewMessages[0].Text);
    }

    [Fact]
    public async Task Follow_up_appears_only_after_the_practice_has_been_completed()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Паническая атака, не могу дышать, сердце колотится");
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Practice, "Начнём", "Grounding"));

        Assert.Null(await service.CheckPracticeFollowUpAsync(id));

        _progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new CompletionDTO { ItemKey = "Grounding", CompletedAt = _clock.Utc.AddMinutes(4) }]);
        ChatTurnResult? followUp = await service.CheckPracticeFollowUpAsync(id);

        Assert.NotNull(followUp);
        Assert.Contains("С возвращением", followUp!.NewMessages[0].Text);
        Assert.Equal(11, followUp.NewMessages[^1].QuickReplies.Count);
        Assert.Null(await service.CheckPracticeFollowUpAsync(id));
    }

    [Fact]
    public async Task A_completion_of_another_technique_or_before_the_start_does_not_count()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Паническая атака, не могу дышать, сердце колотится");
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Practice, "Начнём", "Grounding"));
        _progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new CompletionDTO { ItemKey = "Observer", CompletedAt = _clock.Utc.AddMinutes(4) },
                new CompletionDTO { ItemKey = "Grounding", CompletedAt = _clock.Utc.AddMinutes(-30) }
            ]);

        Assert.Null(await service.CheckPracticeFollowUpAsync(id));
    }

    [Fact]
    public async Task A_second_chat_remembers_the_first_one()
    {
        ChatService service = CreateService();
        long first = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(first, "Завтра важная презентация, я очень тревожусь на работе");
        _clock.Utc = _clock.Utc.AddDays(2);

        ChatTurnResult second = await service.StartNewChatAsync();

        Assert.NotEqual(first, second.Session.Id);
        Assert.Contains("«Тревога · работа»", second.NewMessages[0].Text);
        Assert.Equal(4, second.NewMessages[0].QuickReplies.Count);
    }

    [Fact]
    public async Task Crisis_text_is_stored_and_returns_the_hub_action()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;

        ChatTurnResult result = await service.SendTextAsync(id, "Я не хочу больше жить");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, result.Action!.Kind);
        Assert.Equal(2, result.NewMessages.Count);
    }

    [Fact]
    public async Task Rename_trims_and_limits_the_title_and_ignores_blank_names()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;

        await service.RenameAsync(id, "  Разговор про работу  ");
        Assert.Equal("Разговор про работу", (await service.GetChatAsync(id))!.Title);

        await service.RenameAsync(id, "   ");
        Assert.Equal("Разговор про работу", (await service.GetChatAsync(id))!.Title);

        await service.RenameAsync(id, new string('я', 200));
        Assert.Equal(60, (await service.GetChatAsync(id))!.Title.Length);
    }

    [Fact]
    public async Task Delete_removes_the_chat_and_last_chat_ignores_untouched_ones()
    {
        ChatService service = CreateService();
        long first = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(first, "Мне грустно и пусто");
        Assert.Equal(first, (await service.GetLastChatAsync())!.Id);

        await service.DeleteAsync(first);

        Assert.Null(await service.GetChatAsync(first));
        Assert.Null(await service.GetLastChatAsync());
        long untouched = (await service.StartNewChatAsync()).Session.Id;
        Assert.Null(await service.GetLastChatAsync());
        Assert.NotEqual(first, untouched);
    }

    [Fact]
    public async Task Sending_to_a_missing_chat_fails_clearly_and_blank_text_is_ignored()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SendTextAsync(9999, "привет всем"));
        ChatTurnResult blank = await service.SendTextAsync(id, "   ");

        Assert.Empty(blank.NewMessages);
    }


    [Fact]
    public async Task A_manual_title_survives_later_taps_on_suggestion_chips()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Паническая атака, не могу дышать, сердце колотится");
        await service.RenameAsync(id, "Мой разговор");

        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.More, "Хочу ещё рассказать"));

        Assert.Equal("Мой разговор", (await service.GetChatAsync(id))!.Title);
    }

    [Fact]
    public async Task A_chat_that_starts_with_hello_is_named_after_the_first_real_message()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;

        ChatTurnResult hello = await service.SendTextAsync(id, "Привет");
        Assert.Equal("Новый чат", hello.Session.Title);
        Assert.True((await service.GetChatAsync(id))!.HasConversation());

        ChatTurnResult real = await service.SendTextAsync(id, "Мне очень тревожно из-за работы");
        Assert.Equal("Тревога · работа", real.Session.Title);
    }

    [Fact]
    public async Task English_chats_use_english_titles_and_wording()
    {
        ChatService service = CreateService(english: true);

        ChatTurnResult started = await service.StartNewChatAsync();
        ChatTurnResult sent = await service.SendTextAsync(started.Session.Id, "I feel so anxious about my job. I can't sleep");

        Assert.Equal("New chat", started.Session.Title);
        Assert.Equal("Anxiety · work", sent.Session.Title);
    }
}
