using PsychologyApp.Presentation.Entities.Technique;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>
/// The built-in practices split into the sections of the catalog (body, thoughts, heart, action), each under its own heading, in a fixed order.
/// With a group chosen in the filter only that section remains. A section with no practices is not shown, and a filter that would leave
/// nothing shows everything, so the list is never empty.
/// </summary>
public static class TechniqueSections
{
    private static readonly TechniqueFlavor[] Order = [TechniqueFlavor.Body, TechniqueFlavor.Mind, TechniqueFlavor.Heart, TechniqueFlavor.Action];

    public static IReadOnlyList<TechniqueGroup> Build(IReadOnlyList<TechniqueItem> items, string? filterKey, Func<TechniqueFlavor, string> titleOf)
    {
        List<TechniqueGroup> sections = Sections(items, filterKey, titleOf);
        return sections.Count > 0 ? sections : Sections(items, TechniqueFlavors.FilterAll, titleOf);
    }

    private static List<TechniqueGroup> Sections(IReadOnlyList<TechniqueItem> items, string? filterKey, Func<TechniqueFlavor, string> titleOf)
    {
        List<TechniqueGroup> sections = [];
        foreach (TechniqueFlavor flavor in Order)
        {
            if (filterKey is not (null or "" or TechniqueFlavors.FilterAll) && !string.Equals(filterKey, flavor.ToString(), StringComparison.Ordinal))
            {
                continue;
            }

            TechniqueItem[] inSection = [.. items.Where(item => TechniqueFlavors.Parse(item.Flavor) == flavor)];
            if (inSection.Length > 0)
            {
                sections.Add(new TechniqueGroup(titleOf(flavor), inSection));
            }
        }

        // Practices without a group (none are built in today) stay visible rather than vanish, under no heading, after the sections.
        TechniqueItem[] ungrouped = [.. items.Where(item => TechniqueFlavors.Parse(item.Flavor) == TechniqueFlavor.None)];
        if (ungrouped.Length > 0 && filterKey is null or "" or TechniqueFlavors.FilterAll)
        {
            sections.Add(new TechniqueGroup(string.Empty, ungrouped));
        }

        return sections;
    }
}
