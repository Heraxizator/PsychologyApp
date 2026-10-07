using PsychologyApp.Presentation.Entities.Technique;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>
/// The built-in practices split into the sections of the catalog (body, thoughts, heart, action), each under its own heading, in a fixed order.
/// A section with no practices is not shown; a practice that belongs to no group is kept, under no heading, after the sections, so none can vanish.
/// </summary>
public static class TechniqueSections
{
    private static readonly TechniqueFlavor[] Order = [TechniqueFlavor.Body, TechniqueFlavor.Mind, TechniqueFlavor.Heart, TechniqueFlavor.Action];

    public static IReadOnlyList<TechniqueGroup> Build(IReadOnlyList<TechniqueItem> items, Func<TechniqueFlavor, string> titleOf)
    {
        List<TechniqueGroup> sections = [];
        foreach (TechniqueFlavor flavor in Order)
        {
            TechniqueItem[] inSection = [.. items.Where(item => TechniqueFlavors.Parse(item.Flavor) == flavor)];
            if (inSection.Length > 0)
            {
                sections.Add(new TechniqueGroup(titleOf(flavor), inSection));
            }
        }

        TechniqueItem[] ungrouped = [.. items.Where(item => TechniqueFlavors.Parse(item.Flavor) == TechniqueFlavor.None)];
        if (ungrouped.Length > 0)
        {
            sections.Add(new TechniqueGroup(string.Empty, ungrouped));
        }

        return sections;
    }
}
