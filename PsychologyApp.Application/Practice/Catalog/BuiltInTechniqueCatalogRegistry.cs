using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class BuiltInTechniqueCatalogRegistry
{
    internal static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> Russian { get; } =
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
            [TechniqueId.Grounding] = TechniqueCatalogContentRu.Grounding
        };

    internal static IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> English { get; } =
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
            [TechniqueId.Grounding] = TechniqueCatalogContentEn.Grounding
        };
}
