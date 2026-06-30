using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;
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
