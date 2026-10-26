using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The burger menu: its group colours are readable in both themes, and help stands apart.</summary>
public sealed class AppMenuLookTests
{
    // The text colour of BodyStyle (Colors.xaml): the help row sits on its tile colour.
    private static string Text(bool dark) => dark ? "#EEEEEE" : "#343434";

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Every_icon_and_label_is_readable_on_its_tile(bool dark)
    {
        foreach (AppMenuTone tone in Enum.GetValues<AppMenuTone>())
        {
            (string tile, string icon) = AppMenuLook.Colors(tone, dark);
            Assert.True(ColorContrast.Ratio(icon, tile) >= ColorContrast.LargeTextMinimum, $"{tone} icon, dark={dark}");
            Assert.True(ColorContrast.Ratio(Text(dark), tile) >= ColorContrast.BodyTextMinimum, $"{tone} label, dark={dark}");
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Help_has_a_colour_of_its_own(bool dark)
    {
        string help = AppMenuLook.Colors(AppMenuTone.Help, dark).Tile;

        Assert.All(Enum.GetValues<AppMenuTone>().Where(tone => tone != AppMenuTone.Help), tone => Assert.NotEqual(help, AppMenuLook.Colors(tone, dark).Tile));
    }
}
