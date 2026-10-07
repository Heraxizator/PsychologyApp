namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>The 0..10 tension scale as the person sees it: whole numbers, and a colour that moves from calm teal through amber to a warm red.</summary>
public static class TensionScale
{
    public const int Min = 0;
    public const int Max = 10;

    // The number is large text on a white or near-black surface, so each theme has its own stops: deeper on white, lighter on dark.
    private static readonly (byte R, byte G, byte B)[] LightStops = [(0x1F, 0x8A, 0x7A), (0xC0, 0x6E, 0x10), (0xC0, 0x39, 0x2B)];
    private static readonly (byte R, byte G, byte B)[] DarkStops = [(0x6F, 0xD3, 0xC2), (0xF2, 0xB8, 0x79), (0xF0, 0x8A, 0x7E)];

    /// <summary>The nearest whole step inside the scale.</summary>
    public static int Snap(double value) => (int)Math.Clamp(Math.Round(value, MidpointRounding.AwayFromZero), Min, Max);

    /// <summary>The colour of a step: teal at 0, amber at 5, red at 10, blended in between.</summary>
    public static (byte R, byte G, byte B) ColorAt(double value, bool dark = false)
    {
        (byte R, byte G, byte B)[] stops = dark ? DarkStops : LightStops;
        double v = Math.Clamp(value, Min, Max);
        return v <= 5 ? Blend(stops[0], stops[1], v / 5) : Blend(stops[1], stops[2], (v - 5) / 5);
    }

    public static string HexAt(double value, bool dark = false)
    {
        (byte r, byte g, byte b) = ColorAt(value, dark);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>"calm", "strong" or neither: a word at the ends of the scale, none in the middle where a word would only judge.</summary>
    public static TensionWord WordAt(int value) => value <= 2 ? TensionWord.Calm : value >= 8 ? TensionWord.Strong : TensionWord.None;

    private static (byte R, byte G, byte B) Blend((byte R, byte G, byte B) a, (byte R, byte G, byte B) b, double t) => (
        (byte)Math.Round(a.R + (b.R - a.R) * t),
        (byte)Math.Round(a.G + (b.G - a.G) * t),
        (byte)Math.Round(a.B + (b.B - a.B) * t));
}

public enum TensionWord
{
    None,
    Calm,
    Strong
}
