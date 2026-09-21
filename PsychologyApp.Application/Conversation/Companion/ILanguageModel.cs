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
public interface ILanguageModel
{
    /// <summary>True when a model is installed and can answer right now.</summary>
    bool IsAvailable { get; }

    Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default);

    /// <summary>Loads the model ahead of the first request so the first reply is not slowed by loading. Safe to ignore.</summary>
    Task WarmUpAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <summary>Frees the loaded model (memory and file handles), e.g. before its files are deleted or replaced. It reloads on demand.</summary>
    Task ReleaseAsync() => Task.CompletedTask;
}

public sealed class NullLanguageModel : ILanguageModel
{
    public bool IsAvailable => false;

    public Task<string?> GenerateAsync(LlmRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);
}
