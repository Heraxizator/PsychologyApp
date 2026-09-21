using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Shared.Abstractions;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Platform;

/// <summary>Reads dialogue scenarios from <c>Resources/Raw/conversations</c> (<c>Name.json</c> for Russian, <c>Name.en.json</c> for English).</summary>
public sealed class MauiConversationScenarioProvider(
    ITestAssetReader assetReader,
    ILogger<MauiConversationScenarioProvider> logger) : IConversationScenarioProvider
{
    private static readonly HashSet<TechniqueId> DialogueTechniques = [TechniqueId.Observer];

    private readonly ConcurrentDictionary<string, ConversationScenario> _cache = new(StringComparer.Ordinal);

    public bool HasScenario(TechniqueId techniqueId) => DialogueTechniques.Contains(techniqueId);

    public async Task<ConversationScenario?> LoadAsync(TechniqueId techniqueId, CancellationToken cancellationToken = default)
    {
        if (!HasScenario(techniqueId))
        {
            return null;
        }

        string cacheKey = $"{techniqueId}:{AppStrings.Language}";
        if (_cache.TryGetValue(cacheKey, out ConversationScenario? cached))
        {
            return cached;
        }

        try
        {
            await using Stream stream = await assetReader
                .OpenAsync($"conversations/{techniqueId}.json", cancellationToken)
                .ConfigureAwait(false);
            using StreamReader reader = new(stream);
            string json = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

            ParseResult<ConversationScenario> parsed = ConversationScenarioParser.Parse(json);
            if (!parsed.IsSuccess)
            {
                logger.LogError("Dialogue scenario for {TechniqueId} is invalid: {Error}", techniqueId, parsed.Error);
                return null;
            }

            _cache[cacheKey] = parsed.Value!;
            return parsed.Value;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to load dialogue scenario for {TechniqueId}", techniqueId);
            return null;
        }
    }
}
