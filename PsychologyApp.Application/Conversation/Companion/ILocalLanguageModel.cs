namespace PsychologyApp.Application.Conversation.Companion;

public enum LlmRole
{
    User,
    Assistant
}

public sealed record LlmMessage(LlmRole Role, string Content);

public sealed record LlmRequest(
    string SystemPrompt,
    IReadOnlyList<LlmMessage> Messages,
    int MaxNewTokens = 140,
    float Temperature = 0.5f);

/// <summary>
/// On-device language model. Nothing here may touch the network: the user's words never leave the phone.
/// Implementations return <c>null</c> (or throw) when the model is missing or fails; callers must fall back to scripted text.
/// </summary>
public interface ILocalLanguageModel
{
    /// <summary>True when a model is installed and can answer right now.</summary>
    bool IsAvailable { get; }

    Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default);
}

public sealed class NullLanguageModel : ILocalLanguageModel
{
    public bool IsAvailable => false;

    public Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);
}
