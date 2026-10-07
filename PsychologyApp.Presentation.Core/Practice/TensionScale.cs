namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>The 0..10 tension scale as the person sees it: whole numbers, and a colour that moves from calm teal through amber to a warm red.</summary>
public static class TensionScale
{
    public const int Min = 0;
    public const int Max = 10;

    private static readonly (byte R, byte G, byte B) Calm = (0x4F, 0xB6, 0xA6);
    private static readonly (byte R, byte G, byte B) Middle = (0xE8, 0xA2, 0x5A);
    private static readonly (byte R, byte G, byte B) Strong = (0xD9, 0x5A, 0x4E);

    /// <summary>The nearest whole step inside the scale.</summary>
    public static int Snap(double value) => (int)Math.Clamp(Math.Round(value, MidpointRounding.AwayFromZero), Min, Max);

    /// <summary>The colour of a step: teal at 0, amber at 5, red at 10, blended in between.</summary>
    public static (byte R, byte G, byte B) ColorAt(double value)
    {
        double v = Math.Clamp(value, Min, Max);
        return v <= 5 ? Blend(Calm, Middle, v / 5) : Blend(Middle, Strong, (v - 5) / 5);
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
