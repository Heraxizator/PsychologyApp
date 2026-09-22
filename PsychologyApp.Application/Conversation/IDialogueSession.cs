namespace PsychologyApp.Application.Conversation;

public enum DialogueActionKind
{
    StartTechnique,
    OpenCrisisHub,
    /// <summary>Opens the Tests tab so the person can pick a short self-assessment on their own.</summary>
    OpenTests,
    /// <summary>Opens the body/psychosomatic explorer.</summary>
    OpenSomatic,
    /// <summary>Opens the prayers/calming audio tab.</summary>
    OpenPrayers,
    /// <summary>Opens the quotes tab for more like the one just shared.</summary>
    OpenQuotes
}

/// <summary>Something the UI must do once the dialogue has said its last words.</summary>
public sealed record DialogueAction(DialogueActionKind Kind, TechniqueId? TechniqueId = null);

/// <summary>
/// A turn-based dialogue the chat UI can drive without knowing whether it is a scripted scenario or the companion.
/// Asynchronous because the companion may wait for an on-device language model.
/// </summary>
public interface IDialogueSession
{
    ConversationStatus Status { get; }

    Task<ConversationTurn> StartAsync(CancellationToken cancellationToken = default);

    Task<ConversationTurn> SubmitTextAsync(string text, CancellationToken cancellationToken = default);

    Task<ConversationTurn> SubmitChoiceAsync(int index, CancellationToken cancellationToken = default);

    Task<ConversationTurn> SubmitRatingAsync(int rating, CancellationToken cancellationToken = default);
}
