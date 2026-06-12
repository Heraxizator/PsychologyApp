using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.UI.Components;

namespace PsychologyApp.Presentation.Models.Practice.Techniques;

public sealed record TechniqueDefinition(
    string ModuleName,
    string PageName,
    IReadOnlyList<string> Algorithm,
    string TheoryInfo,
    TechniqueUiKind UiKind,
    string ListNumber,
    string ListDate,
    string ListTitle,
    string ListSubtitle,
    string Theme,
    string Author,
    int ListDurationMinutes,
    string ListIcon,
    IReadOnlyList<EntryItem>? Entries = null,
    IReadOnlyList<TheorySection>? TheorySections = null,
    string? PolarityNegativePlaceholder = null,
    string? PolarityPositivePlaceholder = null);

public static class TechniqueCatalog
{
    private static readonly Dictionary<TechniqueId, TechniqueDefinition> Definitions = new()
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
        [TechniqueId.Check] = TechniqueCatalogContentRu.Check
    };

    public static IReadOnlyList<TechniqueDefinition> All =>
        IsEnglish ? TechniqueCatalogEnglish.Definitions.Values.ToList() : Definitions.Values.ToList();

    public static TechniqueDefinition Get(TechniqueId id) =>
        IsEnglish ? TechniqueCatalogEnglish.Definitions[id] : Definitions[id];

    private static bool IsEnglish => AppStrings.IsEnglish(AppStrings.Language);
}
