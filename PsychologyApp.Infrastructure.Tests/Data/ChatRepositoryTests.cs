using PsychologyApp.Application.Chat;
using PsychologyApp.Infrastructure.Data.Repositories.Chat;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public sealed class ChatRepositoryTests
{
    private readonly ChatRepository _repository = new(new SharedMemoryConnectionFactory(), RepositoryTestContext.Settings);

    private static readonly DateTime T0 = new(2026, 9, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Created_session_can_be_read_back_with_empty_history()
    {
        long id = await _repository.CreateSessionAsync("Новый чат", T0);

        ChatSessionDTO? session = await _repository.GetSessionAsync(id);

        Assert.NotNull(session);
        Assert.Equal("Новый чат", session!.Title);
        Assert.Equal(0, session.MessageCount);
        Assert.Null(session.Preview);
        Assert.Equal(T0, session.CreatedAt);
    }

    [Fact]
    public async Task Messages_keep_order_role_and_quick_replies()
    {
        long id = await _repository.CreateSessionAsync("t", T0);
        await _repository.AddMessageAsync(new ChatMessageDTO { SessionId = id, Role = ChatRole.User, Text = "Мне тревожно", CreatedAt = T0.AddMinutes(1) });
        await _repository.AddMessageAsync(new ChatMessageDTO
        {
            SessionId = id,
            Role = ChatRole.Companion,
            Text = "Слышу вас",
            CreatedAt = T0.AddMinutes(2),
            QuickReplies = [new ChatQuickReply(ChatQuickReplyKinds.Practice, "Начнём: Заземление", "Grounding"), new ChatQuickReply(ChatQuickReplyKinds.More, "Хочу ещё рассказать")]
        });

        IReadOnlyList<ChatMessageDTO> messages = await _repository.GetMessagesAsync(id);

        Assert.Equal(["Мне тревожно", "Слышу вас"], messages.Select(m => m.Text));
        Assert.Equal([ChatRole.User, ChatRole.Companion], messages.Select(m => m.Role));
        Assert.Empty(messages[0].QuickReplies);
        Assert.Equal("Grounding", messages[1].QuickReplies[0].Payload);
        Assert.Null(messages[1].QuickReplies[1].Payload);
        Assert.Equal(T0.AddMinutes(2), messages[1].CreatedAt);
    }

    [Fact]
    public async Task A_batch_of_messages_is_stored_in_order_and_returns_matching_ids()
    {
        long id = await _repository.CreateSessionAsync("t", T0);

        IReadOnlyList<long> ids = await _repository.AddMessagesAsync(
        [
            new ChatMessageDTO { SessionId = id, Role = ChatRole.User, Text = "первое", CreatedAt = T0 },
            new ChatMessageDTO { SessionId = id, Role = ChatRole.Companion, Text = "второе", CreatedAt = T0 },
            new ChatMessageDTO { SessionId = id, Role = ChatRole.Companion, Text = "третье", CreatedAt = T0 }
        ]);

        IReadOnlyList<ChatMessageDTO> messages = await _repository.GetMessagesAsync(id);
        Assert.Equal(ids, messages.Select(m => m.Id));
        Assert.Equal(["первое", "второе", "третье"], messages.Select(m => m.Text));
        Assert.Empty(await _repository.AddMessagesAsync([]));
    }

    [Fact]
    public async Task Sessions_are_listed_by_latest_activity_with_preview_and_count()
    {
        long older = await _repository.CreateSessionAsync("Старый", T0);
        long newer = await _repository.CreateSessionAsync("Новый", T0.AddHours(1));
        await _repository.AddMessageAsync(new ChatMessageDTO { SessionId = older, Role = ChatRole.User, Text = "первое", CreatedAt = T0 });
        await _repository.AddMessageAsync(new ChatMessageDTO { SessionId = older, Role = ChatRole.Companion, Text = "последнее", CreatedAt = T0.AddMinutes(1) });

        ChatSessionDTO touched = (await _repository.GetSessionAsync(older))!;
        touched.UpdatedAt = T0.AddHours(2);
        await _repository.UpdateSessionAsync(touched);

        IReadOnlyList<ChatSessionDTO> sessions = await _repository.GetSessionsAsync();

        Assert.Equal([older, newer], sessions.Select(s => s.Id));
        Assert.Equal("последнее", sessions[0].Preview);
        Assert.Equal(2, sessions[0].MessageCount);
        Assert.Equal(0, sessions[1].MessageCount);
    }

    [Fact]
    public async Task Update_persists_title_summary_and_state()
    {
        long id = await _repository.CreateSessionAsync("t", T0);
        ChatSessionDTO session = (await _repository.GetSessionAsync(id))!;
        session.Title = "Тревога: работа";
        session.Emotion = "Anxiety";
        session.Theme = "Work";
        session.FirstIntensity = 8;
        session.LastIntensity = 4;
        session.StateJson = "{\"turns\":3}";
        session.UpdatedAt = T0.AddMinutes(5);

        await _repository.UpdateSessionAsync(session);
        ChatSessionDTO reloaded = (await _repository.GetSessionAsync(id))!;

        Assert.Equal("Тревога: работа", reloaded.Title);
        Assert.Equal("Anxiety", reloaded.Emotion);
        Assert.Equal("Work", reloaded.Theme);
        Assert.Equal(8, reloaded.FirstIntensity);
        Assert.Equal(4, reloaded.LastIntensity);
        Assert.Equal("{\"turns\":3}", reloaded.StateJson);
        Assert.Equal(T0.AddMinutes(5), reloaded.UpdatedAt);
    }

    [Fact]
    public async Task Delete_removes_the_session_and_only_its_messages()
    {
        long doomed = await _repository.CreateSessionAsync("a", T0);
        long kept = await _repository.CreateSessionAsync("b", T0);
        await _repository.AddMessageAsync(new ChatMessageDTO { SessionId = doomed, Role = ChatRole.User, Text = "x", CreatedAt = T0 });
        await _repository.AddMessageAsync(new ChatMessageDTO { SessionId = kept, Role = ChatRole.User, Text = "y", CreatedAt = T0 });

        await _repository.DeleteSessionAsync(doomed);

        Assert.Null(await _repository.GetSessionAsync(doomed));
        Assert.Empty(await _repository.GetMessagesAsync(doomed));
        Assert.Single(await _repository.GetMessagesAsync(kept));
    }

    [Fact]
    public void Quick_reply_json_round_trips_and_tolerates_garbage()
    {
        IReadOnlyList<ChatQuickReply> original = [new(ChatQuickReplyKinds.Rating, "7", "7"), new(ChatQuickReplyKinds.CheckIn, "Стало легче", "better")];

        IReadOnlyList<ChatQuickReply> restored = ChatQuickReplyJson.Deserialize(ChatQuickReplyJson.Serialize(original));

        Assert.Equal(original, restored);
        Assert.Empty(ChatQuickReplyJson.Deserialize("not json"));
        Assert.Empty(ChatQuickReplyJson.Deserialize(null));
        Assert.Null(ChatQuickReplyJson.Serialize([]));
    }
}
