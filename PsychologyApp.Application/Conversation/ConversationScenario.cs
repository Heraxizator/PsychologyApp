namespace PsychologyApp.Application.Conversation;

public enum ConversationNodeKind
{
    Say,
    AskText,
    AskChoice,
    AskRating,
    End
}

public sealed record ConversationChoice(string Label, string Next, string? Value = null);

/// <summary>
/// Routes an <see cref="ConversationNodeKind.AskRating"/> answer; the first matching route wins.
/// A route matches on <see cref="MaxRating"/> (answer at most this), <see cref="BelowKey"/> (answer lower than the rating captured under that key),
/// <see cref="SameKey"/> (answer equal to it), or unconditionally when none is set.
/// </summary>
public sealed record ConversationRoute(int? MaxRating, string Next, string? BelowKey = null, string? SameKey = null);

public sealed record ConversationNode(
    string Id,
    ConversationNodeKind Kind,
    IReadOnlyList<string> Text,
    string? Next = null,
    string? Capture = null,
    string? Hint = null,
    IReadOnlyList<ConversationChoice>? Choices = null,
    IReadOnlyList<ConversationRoute>? Routes = null,
    bool IsPause = false);

public sealed record ConversationScenario(
    string Id,
    string Title,
    string Start,
    string CrisisNode,
    IReadOnlyDictionary<string, ConversationNode> Nodes);
