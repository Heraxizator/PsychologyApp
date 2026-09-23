using Moq;
using PsychologyApp.Application.Abstractions.Integration;
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

        private readonly Dictionary<string, string> _memory = [];

        public Task DeleteAllSessionsAsync(CancellationToken cancellationToken = default)
        {
            _sessions.Clear();
            _messages.Clear();
            return Task.CompletedTask;
        }

        public Task<IReadOnlyDictionary<string, string>> GetMemoryAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyDictionary<string, string>>(new Dictionary<string, string>(_memory));

        public Task SetMemoryAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            _memory[key] = value;
            return Task.CompletedTask;
        }

        public Task IncrementMemoryAsync(string key, CancellationToken cancellationToken = default)
        {
            _memory[key] = (_memory.TryGetValue(key, out string? v) && int.TryParse(v, out int n) ? n + 1 : 1).ToString();
            return Task.CompletedTask;
        }

        public Task DeleteMemoryAsync(string key, CancellationToken cancellationToken = default)
        {
            _memory.Remove(key);
            return Task.CompletedTask;
        }

        public Task ClearMemoryAsync(CancellationToken cancellationToken = default)
        {
            _memory.Clear();
            return Task.CompletedTask;
        }

        private ChatSessionDTO Snapshot(ChatSessionDTO s) => new()
        {
            Id = s.Id, Title = s.Title, CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt, Emotion = s.Emotion, Theme = s.Theme,
            FirstIntensity = s.FirstIntensity, LastIntensity = s.LastIntensity, StateJson = s.StateJson,
            Preview = _messages.LastOrDefault(m => m.SessionId == s.Id)?.Text,
            MessageCount = _messages.Count(m => m.SessionId == s.Id),
            UserMessageCount = _messages.Count(m => m.SessionId == s.Id && m.Role == ChatRole.User)
        };
    }

    private sealed class Clock(DateTime start) : TimeProvider
    {
        public DateTime Utc { get; set; } = start;

        public override DateTimeOffset GetUtcNow() => new(Utc, TimeSpan.Zero);

        // Fixed, not the test machine's own zone: "today" must mean the same thing no matter where this test runs.
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private sealed class Language(bool english = false) : IChatLanguageProvider
    {
        public bool IsEnglish { get; } = english;
    }

    private sealed class StubQuotContentProvider : IQuotContentProvider
    {
        public Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<QuotSeed>>(
            [
                new QuotSeed("Author A", "Grief is love with nowhere to go.", "hope"),
                new QuotSeed("Author B", "Healing is not linear.", "healing"),
                new QuotSeed("Author C", "You are worthy of the love you keep giving others.", "self-love"),
                new QuotSeed("Author D", "General wisdom for a general day.", "general")
            ]);
    }

    private readonly InMemoryChatRepository _repository = new();
    private readonly Clock _clock = new(new DateTime(2026, 9, 21, 12, 0, 0, DateTimeKind.Utc));
    private readonly Mock<IUserProgressService> _progress = new();

    private ChatService CreateService(bool english = false)
    {
        _progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _progress.Setup(p => p.GetLatestTestResultAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestResultDTO?)null);
        return new ChatService(_repository, new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), _progress.Object, new Language(english), _clock, new StubQuotContentProvider());
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

    [Fact]
    public async Task A_name_told_in_one_chat_is_used_in_the_next()
    {
        ChatService service = CreateService();
        long first = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(first, "Меня зовут Аня");

        ChatTurnResult second = await service.StartNewChatAsync();

        Assert.NotEqual(first, second.Session.Id);
        Assert.Contains("Аня", second.NewMessages[0].Text);
        Assert.Equal("Аня", (await service.GetProfileAsync()).UserName);
    }

    [Fact]
    public async Task The_name_can_be_changed_and_forgotten_from_the_profile()
    {
        ChatService service = CreateService();
        await service.SetUserNameAsync("  Вера  ");
        Assert.Equal("Вера", (await service.GetProfileAsync()).UserName);

        long id = (await service.StartNewChatAsync()).Session.Id;
        ChatTurnResult bye = await service.SendTextAsync(id, "Пока");
        Assert.Contains("Вера", bye.NewMessages[^1].Text);

        await service.SetUserNameAsync("   ");
        Assert.Null((await service.GetProfileAsync()).UserName);
    }

    [Fact]
    public async Task A_practice_that_lowered_the_tension_is_remembered_and_offered_first_in_the_next_chat()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Мне тревожно из-за работы");
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Rating, "8", "8"));
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Practice, "Начнём", "Grounding"));
        _progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new CompletionDTO { ItemKey = "Grounding", CompletedAt = _clock.Utc.AddMinutes(4) }]);
        await service.CheckPracticeFollowUpAsync(id);

        ChatTurnResult better = await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Rating, "3", "3"));

        Assert.Contains(better.NewMessages, m => m.Text.StartsWith("Запомню", StringComparison.Ordinal));
        CompanionProfile profile = await service.GetProfileAsync();
        Assert.Equal(1, profile.PracticesTried);
        Assert.Equal(1, profile.PracticesHelped);
        Assert.Equal(TechniqueId.Grounding, profile.Practices[0].Technique);

        long next = (await service.StartNewChatAsync()).Session.Id;
        ChatTurnResult offer = await service.SendTextAsync(next, "Мне тревожно из-за завтрашней встречи");
        offer = await service.SendTextAsync(next, "Я думаю о ней весь день");
        offer = await service.SendTextAsync(next, "И ночью тоже не могу перестать");

        Assert.Contains(offer.NewMessages, m => m.Text.Contains("В прошлый раз вам помогла практика", StringComparison.Ordinal));
        Assert.Equal("Grounding", offer.NewMessages[^1].QuickReplies.First(q => q.Kind == ChatQuickReplyKinds.Practice).Payload);
    }

    [Fact]
    public async Task The_profile_counts_chats_messages_and_days()
    {
        ChatService service = CreateService();
        long first = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(first, "Мне тревожно из-за работы");
        await service.SendTextAsync(first, "Начальник опять недоволен");
        _clock.Utc = _clock.Utc.AddDays(1);
        long second = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(second, "Я так устала");

        CompanionProfile profile = await service.GetProfileAsync();

        Assert.Equal(2, profile.Chats);
        Assert.Equal(3, profile.UserMessages);
        Assert.Equal(2, profile.Days);
        Assert.Equal(2, profile.StreakDays);
        Assert.True(profile.HasHistory);
    }

    [Fact]
    public async Task Deleting_all_chats_keeps_the_memory_and_forgetting_keeps_the_chats()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Меня зовут Аня");

        await service.ForgetMemoryAsync();
        Assert.Null((await service.GetProfileAsync()).UserName);
        Assert.Single(await service.GetChatsAsync());

        await service.SetUserNameAsync("Аня");
        await service.DeleteAllChatsAsync();

        Assert.Empty(await service.GetChatsAsync());
        Assert.Equal("Аня", (await service.GetProfileAsync()).UserName);
    }

    [Fact]
    public async Task A_low_mood_logged_today_is_picked_up_by_a_new_chats_greeting()
    {
        ChatService service = CreateService();
        _progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new MoodEntryDTO { MoodLevel = 2, Note = "тяжёлый день на работе", RecordedAt = _clock.Utc.AddHours(-2) }]);

        ChatTurnResult result = await service.StartNewChatAsync();

        Assert.Contains("тяжёлый день на работе", result.NewMessages[0].Text);
    }

    [Fact]
    public async Task Yesterdays_mood_does_not_leak_into_todays_greeting()
    {
        ChatService service = CreateService();
        _progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new MoodEntryDTO { MoodLevel = 1, Note = "вчера было плохо", RecordedAt = _clock.Utc.AddDays(-1) }]);

        ChatTurnResult result = await service.StartNewChatAsync();

        Assert.DoesNotContain("вчера было плохо", result.NewMessages[0].Text);
    }

    [Fact]
    public async Task An_ordinary_mood_today_does_not_change_the_greeting()
    {
        ChatService service = CreateService();
        _progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new MoodEntryDTO { MoodLevel = 4, RecordedAt = _clock.Utc }]);

        ChatTurnResult result = await service.StartNewChatAsync();

        Assert.Equal(2, result.NewMessages.Count);
    }

    [Fact]
    public async Task Tapping_the_journal_chip_writes_a_mood_entry_and_the_action_never_reaches_the_ui()
    {
        ChatService service = CreateService();
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Меня бесит начальник, опять раскритиковал при всех");
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Rating, "7", "7"));
        ChatTurnResult thanks = await service.SendTextAsync(id, "Спасибо");
        ChatQuickReply journalChip = thanks.NewMessages[^1].QuickReplies.First(c => c.Payload == "journal:log");

        ChatTurnResult logged = await service.SendQuickReplyAsync(id, journalChip);

        Assert.Null(logged.Action);
        _progress.Verify(p => p.RecordMoodAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task A_recent_stress_test_result_is_referenced_when_the_companion_would_suggest_a_test()
    {
        ChatService service = CreateService();
        _progress.Setup(p => p.GetLatestTestResultAsync(CompanionResourceContent.StressTestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResultDTO { TestId = "pss10", Summary = "Умеренный стресс", CompletedAt = _clock.Utc.AddDays(-5) });
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Так устала, что уже сил ни на что нет");
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Rating, "6", "6"));
        await service.SendTextAsync(id, "Даже дома продолжаю об этом думать весь вечер");

        ChatTurnResult offer = await service.SendTextAsync(id, "И на следующий день сил всё равно нет совсем");

        Assert.Contains(offer.NewMessages, m => m.Text.Contains("Умеренный стресс"));
    }

    [Fact]
    public async Task A_stress_test_result_older_than_a_month_is_not_referenced()
    {
        ChatService service = CreateService();
        _progress.Setup(p => p.GetLatestTestResultAsync(CompanionResourceContent.StressTestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResultDTO { TestId = "pss10", Summary = "Умеренный стресс", CompletedAt = _clock.Utc.AddDays(-45) });
        long id = (await service.StartNewChatAsync()).Session.Id;
        await service.SendTextAsync(id, "Так устала, что уже сил ни на что нет");
        await service.SendQuickReplyAsync(id, new ChatQuickReply(ChatQuickReplyKinds.Rating, "6", "6"));
        await service.SendTextAsync(id, "Даже дома продолжаю об этом думать весь вечер");

        ChatTurnResult offer = await service.SendTextAsync(id, "И на следующий день сил всё равно нет совсем");

        Assert.DoesNotContain(offer.NewMessages, m => m.Text.Contains("Умеренный стресс"));
    }
}
