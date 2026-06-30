using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Models.Practice;

public sealed record BuiltInTechniqueDefinition(
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
    IReadOnlyList<TechniqueEntrySeed>? Entries = null,
    IReadOnlyList<TheorySection>? TheorySections = null,
    string? PolarityNegativePlaceholder = null,
    string? PolarityPositivePlaceholder = null);
