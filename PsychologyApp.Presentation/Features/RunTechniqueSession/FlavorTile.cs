using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>The icon tile colours of a card in the current theme: the group's pair, or the accent tint for a card with no group.</summary>
public static class FlavorTile
{
    /// <returns>The tile colour, and the icon colour or <c>null</c> to keep the icon's own accent.</returns>
    public static (Color Tile, Color? Icon) Colours(TechniqueFlavor flavor)
    {
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        if (TechniqueFlavors.Colors(flavor, dark) is { } colours)
        {
            return (Color.FromArgb(colours.Tile), Color.FromArgb(colours.Icon));
        }

        return (AppResources.ThemedColor("PrimaryTint", "PrimaryTintDark", Colors.Transparent), null);
    }
}
