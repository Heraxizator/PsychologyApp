using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Domain.Practice;
using System.Text.Json;
using PsychologyApp.Application.Serialization;

namespace PsychologyApp.Application.Tests;

public static class LuscherScoreRecommendation
{
    public static TechniqueId RecommendTechnique(int coValue) => coValue switch
    {
        >= 23 => TechniqueId.Spin,
        >= 17 => TechniqueId.Breathing,
        >= 12 => TechniqueId.Grounding,
        _ => TechniqueId.Experience
    };
}

public sealed class LuscherDetailReader
{
    public LuscherStandardResultDetail? TryParseStandard(string? detailJson)
    {
        if (string.IsNullOrWhiteSpace(detailJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize(detailJson, TestJsonSerializerContext.Default.LuscherStandardResultDetail);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public LuscherBriefResultDetail? TryParseBrief(string? detailJson)
    {
        if (string.IsNullOrWhiteSpace(detailJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize(detailJson, TestJsonSerializerContext.Default.LuscherBriefResultDetail);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
