using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class BuiltInTechniqueCatalogRegistry
{
    /// <summary>The catalogue in a language by its key ("ru", "en", "fr"…); a language without its own catalogue gets the English one.</summary>
    internal static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> For(string? language) =>
        language?.ToLowerInvariant() switch
        {
            "ru" => Russian,
            "en" => English,
            "fr" => French,
            "de" => German,
            "es" => Spanish,
            "pt" => Portuguese,
            "zh" => Chinese,
            _ => English
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> Russian { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentRu.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentRu.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentRu.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentRu.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentRu.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentRu.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentRu.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentRu.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentRu.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentRu.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentRu.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentRu.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentRu.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentRu.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentRu.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentRu.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentRu.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentRu.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentRu.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentRu.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentRu.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentRu.Balloons
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> English { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentEn.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentEn.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentEn.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentEn.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentEn.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentEn.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentEn.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentEn.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentEn.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentEn.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentEn.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentEn.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentEn.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentEn.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentEn.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentEn.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentEn.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentEn.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentEn.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentEn.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentEn.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentEn.Balloons
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> French { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentFr.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentFr.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentFr.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentFr.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentFr.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentFr.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentFr.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentFr.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentFr.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentFr.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentFr.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentFr.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentFr.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentFr.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentFr.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentFr.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentFr.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentFr.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentFr.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentFr.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentFr.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentFr.Balloons
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> German { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentDe.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentDe.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentDe.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentDe.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentDe.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentDe.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentDe.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentDe.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentDe.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentDe.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentDe.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentDe.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentDe.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentDe.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentDe.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentDe.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentDe.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentDe.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentDe.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentDe.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentDe.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentDe.Balloons
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> Spanish { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentEs.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentEs.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentEs.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentEs.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentEs.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentEs.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentEs.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentEs.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentEs.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentEs.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentEs.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentEs.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentEs.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentEs.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentEs.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentEs.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentEs.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentEs.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentEs.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentEs.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentEs.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentEs.Balloons
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> Portuguese { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentPt.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentPt.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentPt.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentPt.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentPt.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentPt.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentPt.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentPt.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentPt.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentPt.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentPt.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentPt.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentPt.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentPt.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentPt.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentPt.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentPt.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentPt.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentPt.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentPt.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentPt.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentPt.Balloons
        };

    private static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> Chinese { get; } =
        new Dictionary<TechniqueId, BuiltInTechniqueDefinition>
        {
            [TechniqueId.Spin] = TechniqueCatalogContentZh.Spin,
            [TechniqueId.Comparison] = TechniqueCatalogContentZh.Comparison,
            [TechniqueId.Polarity] = TechniqueCatalogContentZh.Polarity,
            [TechniqueId.Paper] = TechniqueCatalogContentZh.Paper,
            [TechniqueId.Future] = TechniqueCatalogContentZh.Future,
            [TechniqueId.Hack] = TechniqueCatalogContentZh.Hack,
            [TechniqueId.Experience] = TechniqueCatalogContentZh.Experience,
            [TechniqueId.Copied] = TechniqueCatalogContentZh.Copied,
            [TechniqueId.Extend] = TechniqueCatalogContentZh.Extend,
            [TechniqueId.Resize] = TechniqueCatalogContentZh.Resize,
            [TechniqueId.Check] = TechniqueCatalogContentZh.Check,
            [TechniqueId.Observer] = TechniqueCatalogContentZh.Observer,
            [TechniqueId.Anchor] = TechniqueCatalogContentZh.Anchor,
            [TechniqueId.Grounding] = TechniqueCatalogContentZh.Grounding,
            [TechniqueId.Breathing] = TechniqueCatalogContentZh.Breathing,
            [TechniqueId.SmallStep] = TechniqueCatalogContentZh.SmallStep,
            [TechniqueId.ThoughtRecord] = TechniqueCatalogContentZh.ThoughtRecord,
            [TechniqueId.SelfCompassion] = TechniqueCatalogContentZh.SelfCompassion,
            [TechniqueId.BodyScan] = TechniqueCatalogContentZh.BodyScan,
            [TechniqueId.Flashback] = TechniqueCatalogContentZh.Flashback,
            [TechniqueId.Nightmare] = TechniqueCatalogContentZh.Nightmare,
            [TechniqueId.Balloons] = TechniqueCatalogContentZh.Balloons
        };
}
