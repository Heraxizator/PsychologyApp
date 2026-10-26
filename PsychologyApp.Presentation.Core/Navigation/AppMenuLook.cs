using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Shared.Navigation;

/// <summary>The group a menu row belongs to; each has its own tile colour, so the menu reads as groups instead of one blue column.</summary>
public enum AppMenuTone
{
    /// <summary>The person's own pages: profile, what helps, the body.</summary>
    Mine,

    /// <summary>Things to read: for you, courses, articles.</summary>
    Read,

    /// <summary>Things to lean on: anchor thoughts, quotes, prayers.</summary>
    Support,

    /// <summary>Urgent help: warm and tinted, so it is found at a glance.</summary>
    Help,

    /// <summary>The app's own pages: settings.</summary>
    App
}

public static class AppMenuLook
{
    /// <summary>
    /// The tile behind the icon and the icon, for one theme. The three content groups take the tones of the practice groups, so the app
    /// keeps one palette; help gets a warm tone of its own and the app pages a neutral one. The icon is at least 3:1 on its tile, and the
    /// row text at least 4.5:1 on a tile-coloured row (help is drawn that way); both are tested.
    /// </summary>
    public static (string Tile, string Icon) Colors(AppMenuTone tone, bool dark) => tone switch
    {
        AppMenuTone.Mine => TechniqueFlavors.Colors(TechniqueFlavor.Body, dark)!.Value,
        AppMenuTone.Read => TechniqueFlavors.Colors(TechniqueFlavor.Mind, dark)!.Value,
        AppMenuTone.Support => TechniqueFlavors.Colors(TechniqueFlavor.Heart, dark)!.Value,
        AppMenuTone.Help => dark ? ("#4A2420", "#F4B4AB") : ("#FBE0DC", "#B3261E"),
        _ => dark ? ("#2B2F37", "#C2C8D2") : ("#E7E9EE", "#4A5262")
    };
}
