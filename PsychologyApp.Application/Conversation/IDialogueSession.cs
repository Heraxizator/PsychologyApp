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
    OpenQuotes,
    /// <summary>Opens a specific test's history so the person can compare with an earlier result.</summary>
    OpenTestHistory,
    /// <summary>A mood entry should be written to the journal. Handled entirely by ChatService — the tension the person
    /// already gave the companion becomes the journal's own 1..5 scale, with a short note. Never reaches the UI as a
    /// navigation instruction.</summary>
    LogMood
}

/// <summary>Something the UI must do once the dialogue has said its last words.</summary>
/// <param name="TestId">Set with <see cref="DialogueActionKind.OpenTestHistory"/>.</param>
/// <param name="MoodLevel">Set with <see cref="DialogueActionKind.LogMood"/>: 1 (hard day) to 5 (good day).</param>
/// <param name="Note">Set with <see cref="DialogueActionKind.LogMood"/>: a short, human note of what the entry is about.</param>
public sealed record DialogueAction(
    DialogueActionKind Kind,
    TechniqueId? TechniqueId = null,
    string? TestId = null,
    int? MoodLevel = null,
    string? Note = null);

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
