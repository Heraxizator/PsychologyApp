using System.Windows.Input;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.App;

/// <summary>
/// One row of the burger menu: a Material icon on a tile of its group's colour, the label and what a tap opens. Help is drawn on its
/// tinted row with a stronger label, so it is found at a glance.
/// </summary>
public sealed class AppMenuEntry
{
    public AppMenuEntry(string icon, string title, ICommand command, AppMenuTone tone, bool dark)
    {
        Icon = icon;
        Title = title;
        Command = command;
        IsEmphasized = tone == AppMenuTone.Help;
        (string tile, string iconColor) = AppMenuLook.Colors(tone, dark);
        TileColor = Color.FromArgb(tile);
        IconColor = Color.FromArgb(iconColor);
    }

    public string Icon { get; }

    public string Title { get; }

    public ICommand Command { get; }

    public Color TileColor { get; }

    public Color IconColor { get; }

    public bool IsEmphasized { get; }

    public Color RowColor => IsEmphasized ? TileColor : Colors.Transparent;
}
