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
}
