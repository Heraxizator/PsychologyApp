using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Practice;

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
        ["сердц"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["heart"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["груд"] = [TechniqueId.Breathing, TechniqueId.Grounding],
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
        ["дых"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["breath"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["тревог"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["anxiety"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["паник"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["panic"] = [TechniqueId.Breathing, TechniqueId.Grounding],
        ["устал"] = [TechniqueId.SmallStep, TechniqueId.Paper],
        ["tired"] = [TechniqueId.SmallStep, TechniqueId.Paper],
        ["апати"] = [TechniqueId.SmallStep, TechniqueId.Paper],
        ["apathy"] = [TechniqueId.SmallStep, TechniqueId.Paper],
    };

    public static IReadOnlyList<TechniqueId> RecommendForQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [TechniqueId.Breathing, TechniqueId.Grounding];
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
