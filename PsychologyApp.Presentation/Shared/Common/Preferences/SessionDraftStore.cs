using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Presentation.Shared.Common;

public static class SessionDraftStore
{
    public static async Task SaveAsync<T>(
        IUserProgressService service,
        string techniqueKey,
        T payload,
        JsonTypeInfo<T> typeInfo,
        CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(payload, typeInfo);
        await service.SaveSessionDraftAsync(techniqueKey, json, cancellationToken);
    }

    public static async Task<T?> LoadAsync<T>(
        IUserProgressService service,
        string techniqueKey,
        JsonTypeInfo<T> typeInfo,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string? json = await service.GetSessionDraftAsync(techniqueKey, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize(json, typeInfo);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to deserialize session draft for {TechniqueKey}.", techniqueKey);
            return default;
        }
    }
}
