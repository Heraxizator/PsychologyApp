using PsychologyApp.Presentation.Core.Charts;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>Colours of the breathing circle: warm at the start, cool at the end, each in a light-theme and a dark-theme version with text that stays readable on all of them.</summary>
public static class BreathingColors
{
    public const string LightText = "#262626";
    public const string DarkText = "#F2F2F2";
    public const string LightSurface = "#FFFFFF";
    public const string DarkSurface = "#1E1E1E";

    private static readonly (byte R, byte G, byte B) LightTense = (0xE8, 0xA2, 0x5A);
    private static readonly (byte R, byte G, byte B) LightCalm = (0x4F, 0xB6, 0xA6);
    private static readonly (byte R, byte G, byte B) DarkTense = (0x8A, 0x54, 0x16);
    private static readonly (byte R, byte G, byte B) DarkCalm = (0x1F, 0x6E, 0x5C);

    public static string TextHex(bool dark) => dark ? DarkText : LightText;

    /// <summary>The circle colour after <paramref name="calm"/> (0 at the start .. 1 at the end) of the exercise.</summary>
    public static (byte R, byte G, byte B) At(double calm, bool dark)
    {
        double t = Math.Clamp(calm, 0, 1);
        (byte R, byte G, byte B) a = dark ? DarkTense : LightTense;
        (byte R, byte G, byte B) b = dark ? DarkCalm : LightCalm;
        return ((byte)Math.Round(a.R + (b.R - a.R) * t), (byte)Math.Round(a.G + (b.G - a.G) * t), (byte)Math.Round(a.B + (b.B - a.B) * t));
    }

    /// <summary>How opaque the circle is at a given size (0.55 .. 1): small and faint on the empty breath, full on the full one.</summary>
    public static double AlphaAt(double scale) =>
        0.55 + 0.4 * Math.Clamp((scale - BreathingPattern.SmallScale) / (BreathingPattern.LargeScale - BreathingPattern.SmallScale), 0, 1);

    /// <summary>The colour the eye actually sees: the circle over the card surface.</summary>
    public static string Composite((byte R, byte G, byte B) colour, double alpha, bool dark)
    {
        (byte sr, byte sg, byte sb) = ColorContrast.Parse(dark ? DarkSurface : LightSurface);
        byte Mix(byte c, byte s) => (byte)Math.Round(c * alpha + s * (1 - alpha));
        return $"#{Mix(colour.R, sr):X2}{Mix(colour.G, sg):X2}{Mix(colour.B, sb):X2}";
    }
}
