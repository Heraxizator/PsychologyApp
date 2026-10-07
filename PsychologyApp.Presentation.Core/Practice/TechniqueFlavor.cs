using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>What a practice works with. It gives each card its own colour, so the list reads as groups at a glance instead of one blue column.</summary>
public enum TechniqueFlavor
{
    /// <summary>No group (a practice the person made): the usual accent.</summary>
    None,

    /// <summary>Calms the body: breathing, grounding, an anchor.</summary>
    Body,

    /// <summary>Sorts thoughts: records, comparisons, polarities, checks.</summary>
    Mind,

    /// <summary>Kindness and distance: self-compassion, the observer.</summary>
    Heart,

    /// <summary>Gets things moving: a small step, a plan, a reduction.</summary>
    Action
}

public static class TechniqueFlavors
{
    public static TechniqueFlavor For(TechniqueId id) => id switch
    {
        TechniqueId.Breathing or TechniqueId.Grounding or TechniqueId.Anchor => TechniqueFlavor.Body,
        TechniqueId.ThoughtRecord or TechniqueId.Comparison or TechniqueId.Polarity or TechniqueId.Check
            or TechniqueId.Spin or TechniqueId.Paper or TechniqueId.Experience => TechniqueFlavor.Mind,
        TechniqueId.SelfCompassion or TechniqueId.Observer => TechniqueFlavor.Heart,
        TechniqueId.SmallStep or TechniqueId.Future or TechniqueId.Hack or TechniqueId.Extend
            or TechniqueId.Resize or TechniqueId.Copied => TechniqueFlavor.Action,
        _ => TechniqueFlavor.None
    };

    /// <summary>The soft tile behind the icon and the icon itself, for one theme. The icon is at least 3:1 against its tile (it is a graphic that carries meaning).</summary>
    public static (string Tile, string Icon)? Colors(TechniqueFlavor flavor, bool dark) => (flavor, dark) switch
    {
        (TechniqueFlavor.Body, false) => ("#D9F0EB", "#1B7A6B"),
        (TechniqueFlavor.Body, true) => ("#1D3A35", "#6FD3C2"),
        (TechniqueFlavor.Mind, false) => ("#DCE8FA", "#2656A8"),
        (TechniqueFlavor.Mind, true) => ("#1F3050", "#8DB4F2"),
        (TechniqueFlavor.Heart, false) => ("#F8DDE5", "#A93660"),
        (TechniqueFlavor.Heart, true) => ("#47242F", "#F0A0B9"),
        (TechniqueFlavor.Action, false) => ("#FBE8CC", "#9A5A0C"),
        (TechniqueFlavor.Action, true) => ("#43331C", "#F2BE78"),
        _ => null
    };

    public static TechniqueFlavor Parse(string? name) => Enum.TryParse(name, out TechniqueFlavor flavor) ? flavor : TechniqueFlavor.None;
}
