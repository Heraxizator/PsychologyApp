using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using MauiIcons.Material;
using MauiIcons.Material.Outlined;

namespace PsychologyApp.Presentation.App;

/// <summary>
/// Tab bar icons: the selected tab gets the filled glyph, the others the outlined one, so the current tab reads at a glance and the bar
/// stays light. The bottom navigation tints the icons, so only the glyph and its font matter here.
/// </summary>
internal static class TabIcons
{
    // Registered by UseMaterialMauiIcons() and UseMaterialOutlinedMauiIcons().
    private const string FilledFont = "MaterialIcons";
    private const string OutlinedFont = "MaterialOutlinedIcons";
    private const double Size = 24;

    private static readonly ConcurrentDictionary<(string Name, bool Filled), FontImageSource> Cache = new();

    public static ImageSource Get(string name, bool filled) =>
        Cache.GetOrAdd((name, filled), key => new FontImageSource
        {
            Glyph = Glyph(key.Name, key.Filled),
            FontFamily = key.Filled ? FilledFont : OutlinedFont,
            Size = Size
        });

    // The libraries keep each glyph in the enum member's [Description].
    private static string Glyph(string name, bool filled)
    {
        Type icons = filled ? typeof(MaterialIcons) : typeof(MaterialOutlinedIcons);
        return icons.GetField(name, BindingFlags.Public | BindingFlags.Static)?.GetCustomAttribute<DescriptionAttribute>()?.Description
            ?? throw new ArgumentException($"No Material icon named {name}.", nameof(name));
    }
}
