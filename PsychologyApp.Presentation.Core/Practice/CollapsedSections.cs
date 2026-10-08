namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>Which sections of the practice catalog the person has folded, kept as a short text ("Body,Mind") so it can live in the preferences.</summary>
public static class CollapsedSections
{
    private const char Separator = ',';

    public static IReadOnlySet<string> Parse(string? stored) =>
        (stored ?? string.Empty)
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(key => Enum.TryParse(key, out TechniqueFlavor flavor) && flavor != TechniqueFlavor.None)
            .ToHashSet(StringComparer.Ordinal);

    public static string Format(IEnumerable<string> keys) =>
        string.Join(Separator, keys.Distinct(StringComparer.Ordinal).OrderBy(key => key, StringComparer.Ordinal));

    /// <summary>The set with the section folded if it was open and open if it was folded.</summary>
    public static IReadOnlySet<string> Toggle(IReadOnlySet<string> collapsed, string key)
    {
        HashSet<string> next = [.. collapsed];
        if (!next.Remove(key))
        {
            next.Add(key);
        }

        return next;
    }
}
