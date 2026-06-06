namespace PsychologyApp.Presentation.Modules.Practice.Techniques;

public sealed record TechniqueListEntry(
    TechniqueId TechniqueId,
    string Number,
    string Date,
    string Title,
    string Subtitle,
    string Theme,
    string Author);

public static class TechniqueListCatalog
{
    public static IReadOnlyList<TechniqueListEntry> BuiltIn { get; } =
        Enum.GetValues<TechniqueId>()
            .Select(id =>
            {
                TechniqueDefinition d = TechniqueCatalog.Get(id);
                return new TechniqueListEntry(
                    id,
                    d.ListNumber,
                    d.ListDate,
                    d.ListTitle,
                    d.ListSubtitle,
                    d.Theme,
                    d.Author);
            })
            .ToList();
}
