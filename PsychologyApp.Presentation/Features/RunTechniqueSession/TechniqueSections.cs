using System.Windows.Input;
using PsychologyApp.Presentation.Entities.Technique;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>
/// The built-in practices split into the sections of the catalog (body, thoughts, heart, action), each under its own heading, in a fixed order.
/// A folded section keeps its heading and its count but holds no cards. A section with no practices is not shown; a practice that belongs to no
/// group is kept, under no heading, after the sections, so none can vanish.
/// </summary>
public static class TechniqueSections
{
    private static readonly TechniqueFlavor[] Order = [TechniqueFlavor.Body, TechniqueFlavor.Mind, TechniqueFlavor.Heart, TechniqueFlavor.Action];

    public static IReadOnlyList<TechniqueGroup> Build(
        IReadOnlyList<TechniqueItem> items,
        Func<TechniqueFlavor, string> titleOf,
        IReadOnlySet<string>? collapsed = null,
        Func<string, ICommand>? toggleFor = null)
    {
        List<TechniqueGroup> sections = [];
        foreach (TechniqueFlavor flavor in Order)
        {
            TechniqueItem[] inSection = [.. items.Where(item => TechniqueFlavors.Parse(item.Flavor) == flavor)];
            if (inSection.Length == 0)
            {
                continue;
            }

            string key = flavor.ToString();
            bool folded = collapsed?.Contains(key) == true;
            sections.Add(new TechniqueGroup(titleOf(flavor), folded ? [] : inSection)
            {
                Key = key,
                IsCollapsed = folded,
                TotalCount = inSection.Length,
                ToggleCommand = toggleFor?.Invoke(key)
            });
        }

        TechniqueItem[] ungrouped = [.. items.Where(item => TechniqueFlavors.Parse(item.Flavor) == TechniqueFlavor.None)];
        if (ungrouped.Length > 0)
        {
            sections.Add(new TechniqueGroup(string.Empty, ungrouped));
        }

        return sections;
    }
}
