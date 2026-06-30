using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.UI.Components;

namespace PsychologyApp.Presentation.Shared.Practice;

public static class TechniqueDefinitionMapper
{
    public static TechniqueDefinition ToPresentation(BuiltInTechniqueDefinition definition) =>
        new(
            definition.ModuleName,
            definition.PageName,
            definition.Algorithm,
            definition.TheoryInfo,
            definition.UiKind,
            definition.ListNumber,
            definition.ListDate,
            definition.ListTitle,
            definition.ListSubtitle,
            definition.Theme,
            definition.Author,
            definition.ListDurationMinutes,
            definition.ListIcon,
            definition.Entries?.Select(TechniqueEntryMapper.ToPresentation).ToList(),
            definition.TheorySections,
            definition.PolarityNegativePlaceholder,
            definition.PolarityPositivePlaceholder);
}

public static class TechniqueEntryMapper
{
    public static EntryItem ToPresentation(TechniqueEntrySeed seed) => new()
    {
        Title = seed.Title,
        Placeholder = seed.Placeholder,
        Kind = seed.Kind
    };
}
