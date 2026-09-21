namespace PsychologyApp.Application.Chat;

public enum ChatRole
{
    User = 0,
    Companion = 1
}

/// <summary>A tappable suggestion shown under the latest companion message.</summary>
/// <param name="Kind">One of <see cref="ChatQuickReplyKinds"/>.</param>
/// <param name="Payload">Kind-specific value: technique id, rating 0..10, emotion name or check-in answer.</param>
public sealed record ChatQuickReply(string Kind, string Label, string? Payload = null);

public static class ChatQuickReplyKinds
{
    public const string Practice = "practice";
    public const string More = "more";
    public const string Rating = "rating";
    public const string CheckIn = "checkin";
    public const string Emotion = "emotion";
}

public sealed class ChatMessageDTO
{
    public long Id { get; init; }
    public long SessionId { get; init; }
    public ChatRole Role { get; init; }
    public string Text { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<ChatQuickReply> QuickReplies { get; init; } = [];
}

public sealed class ChatSessionDTO
{
    public long Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>Last recognised state (a <c>CompanionEmotion</c> name) or null while unknown.</summary>
    public string? Emotion { get; set; }
    public string? Theme { get; set; }
    public int? FirstIntensity { get; set; }
    public int? LastIntensity { get; set; }

    /// <summary>Serialized dialogue state, so a chat can be resumed exactly where it stopped.</summary>
    public string? StateJson { get; set; }

    /// <summary>Text of the newest message, for the chat list. Filled by queries, not stored.</summary>
    public string? Preview { get; init; }
    public int MessageCount { get; init; }
}
