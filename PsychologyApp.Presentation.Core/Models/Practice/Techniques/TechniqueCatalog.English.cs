using PsychologyApp.Application.Models;

namespace PsychologyApp.Presentation.Models.Practice.Techniques;

internal static class TechniqueCatalogEnglish
{
    internal static readonly Dictionary<TechniqueId, TechniqueDefinition> Definitions = new()
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
        [TechniqueId.Check] = TechniqueCatalogContentEn.Check
    };
}
