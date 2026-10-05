using PsychologyApp.Application.Chat;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IChatRepository
{
    Task<long> CreateSessionAsync(string title, DateTime nowUtc, CancellationToken cancellationToken = default);

    /// <summary>Newest activity first, each with its last message as <see cref="ChatSessionDTO.Preview"/>.</summary>
    Task<IReadOnlyList<ChatSessionDTO>> GetSessionsAsync(CancellationToken cancellationToken = default);

    Task<ChatSessionDTO?> GetSessionAsync(long sessionId, CancellationToken cancellationToken = default);

    Task UpdateSessionAsync(ChatSessionDTO session, CancellationToken cancellationToken = default);

    Task DeleteSessionAsync(long sessionId, CancellationToken cancellationToken = default);

    Task<long> AddMessageAsync(ChatMessageDTO message, CancellationToken cancellationToken = default);

    /// <summary>Stores the messages in order in one transaction. Returns their ids in the same order.</summary>
    Task<IReadOnlyList<long>> AddMessagesAsync(IReadOnlyList<ChatMessageDTO> messages, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(long sessionId, CancellationToken cancellationToken = default);

    /// <summary>Removes every chat and message. What the companion remembers about the person is kept, see <see cref="ClearMemoryAsync"/>.</summary>
    Task DeleteAllSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>Everything the companion remembers between chats, by key.</summary>
    Task<IReadOnlyDictionary<string, string>> GetMemoryAsync(CancellationToken cancellationToken = default);

    Task SetMemoryAsync(string key, string value, CancellationToken cancellationToken = default);

    /// <summary>Adds one to a counter kept under <paramref name="key"/> (created at 1).</summary>
    Task IncrementMemoryAsync(string key, CancellationToken cancellationToken = default);

    Task DeleteMemoryAsync(string key, CancellationToken cancellationToken = default);

    Task ClearMemoryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Everything one turn changes, saved together: the memory it revealed, the messages (user and companion) and the session
    /// (state, title, activity time). Either all of it is stored or none of it, so a chat can never show messages whose state was
    /// lost, or counters for a turn that was not saved. Returns the message ids in order. Implementations should make it one
    /// transaction; this default applies the steps one after another.
    /// </summary>
    async Task<IReadOnlyList<long>> SaveTurnAsync(
        ChatSessionDTO session,
        IReadOnlyList<ChatMessageDTO> messages,
        IReadOnlyList<ChatMemoryChange> memory,
        CancellationToken cancellationToken = default)
    {
        foreach (ChatMemoryChange change in memory)
        {
            if (change.Increment)
            {
                await IncrementMemoryAsync(change.Key, cancellationToken).ConfigureAwait(false);
            }
            else if (change.Value is null)
            {
                await DeleteMemoryAsync(change.Key, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await SetMemoryAsync(change.Key, change.Value, cancellationToken).ConfigureAwait(false);
            }
        }

        IReadOnlyList<long> ids = await AddMessagesAsync(messages, cancellationToken).ConfigureAwait(false);
        await UpdateSessionAsync(session, cancellationToken).ConfigureAwait(false);
        return ids;
    }
}

/// <summary>One change to what the companion remembers: set a value, add one to a counter, or (null value, no increment) forget a key.</summary>
public sealed record ChatMemoryChange(string Key, string? Value = null, bool Increment = false);
