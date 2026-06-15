using PsychologyApp.Domain.Colour.ValueObjects;
using Microsoft.Maui.Graphics;

namespace PsychologyApp.Presentation.Services.Tests;

public static class LuscherColorMapper
{
    public static Color ToMauiColor(ColourValue colour) => colour.Code switch
    {
        "#000000" => Colors.Black,
        "#FF0000" => Colors.Red,
        "#0000FF" => Colors.Blue,
        "#FF00FF" => Colors.Purple,
        "#FFFF00" => Colors.Yellow,
        "#964B00" => Colors.Brown,
        "#00FF00" => Colors.Green,
        "#888888" => Colors.Gray,
        _ => Colors.Transparent
    };
}
