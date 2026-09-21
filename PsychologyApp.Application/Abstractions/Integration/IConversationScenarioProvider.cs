using PsychologyApp.Application.Conversation;

namespace PsychologyApp.Application.Abstractions.Integration;

public interface IConversationScenarioProvider
{
    /// <summary>Returns the dialogue scenario for a technique in the current UI language, or null when the technique has none.</summary>
    Task<ConversationScenario?> LoadAsync(TechniqueId techniqueId, CancellationToken cancellationToken = default);

    bool HasScenario(TechniqueId techniqueId);
}
