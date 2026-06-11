using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Somatic;

public static class SomaticTechniqueRecommendation
{
    private static readonly Dictionary<string, TechniqueId[]> KeywordMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["голов"] = [TechniqueId.Spin, TechniqueId.Paper],
        ["head"] = [TechniqueId.Spin, TechniqueId.Paper],
        ["спин"] = [TechniqueId.Resize, TechniqueId.Experience],
        ["back"] = [TechniqueId.Resize, TechniqueId.Experience],
        ["живот"] = [TechniqueId.Polarity, TechniqueId.Comparison],
        ["stomach"] = [TechniqueId.Polarity, TechniqueId.Comparison],
        ["брюх"] = [TechniqueId.Polarity, TechniqueId.Comparison],
        ["сердц"] = [TechniqueId.Future, TechniqueId.Check],
        ["heart"] = [TechniqueId.Future, TechniqueId.Check],
        ["горл"] = [TechniqueId.Copied, TechniqueId.Extend],
        ["throat"] = [TechniqueId.Copied, TechniqueId.Extend],
        ["плеч"] = [TechniqueId.Hack, TechniqueId.Spin],
        ["shoulder"] = [TechniqueId.Hack, TechniqueId.Spin],
        ["шея"] = [TechniqueId.Spin, TechniqueId.Hack],
        ["neck"] = [TechniqueId.Spin, TechniqueId.Hack],
        ["колен"] = [TechniqueId.Experience, TechniqueId.Resize],
        ["knee"] = [TechniqueId.Experience, TechniqueId.Resize],
        ["рук"] = [TechniqueId.Experience, TechniqueId.Comparison],
        ["hand"] = [TechniqueId.Experience, TechniqueId.Comparison],
        ["arm"] = [TechniqueId.Experience, TechniqueId.Comparison],
        ["давлен"] = [TechniqueId.Check, TechniqueId.Polarity],
        ["pressure"] = [TechniqueId.Check, TechniqueId.Polarity],
    };

    public static IReadOnlyList<TechniqueId> RecommendForQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [TechniqueId.Spin, TechniqueId.Paper];
        }

        foreach ((string keyword, TechniqueId[] techniques) in KeywordMap)
        {
            if (query.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return techniques;
            }
        }

        return [TechniqueId.Spin, TechniqueId.Experience];
    }
}
