using System.Text.Json;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Services.UserProgress;

namespace PsychologyApp.Presentation.Common;

public static class SessionDraftStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SaveAsync<T>(IUserProgressService service, string techniqueKey, T payload, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(payload, SerializerOptions);
        await service.SaveSessionDraftAsync(techniqueKey, json, cancellationToken);
    }

    public static async Task<T?> LoadAsync<T>(
        IUserProgressService service,
        string techniqueKey,
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
            return JsonSerializer.Deserialize<T>(json, SerializerOptions);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to deserialize session draft for {TechniqueKey}.", techniqueKey);
            return default;
        }
    }
}
