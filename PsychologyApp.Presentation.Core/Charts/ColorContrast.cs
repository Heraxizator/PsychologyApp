using System.Globalization;

namespace PsychologyApp.Presentation.Core.Charts;

/// <summary>WCAG contrast between two colours given as "#RRGGBB": 4.5 is the bar for body text, 3 for large text and for graphics that carry meaning.</summary>
public static class ColorContrast
{
    public const double BodyTextMinimum = 4.5;
    public const double LargeTextMinimum = 3.0;

    public static double Ratio(string foreground, string background)
    {
        double a = Luminance(foreground);
        double b = Luminance(background);
        double lighter = Math.Max(a, b);
        double darker = Math.Min(a, b);
        return (lighter + 0.05) / (darker + 0.05);
    }

    public static double Luminance(string hex)
    {
        (byte r, byte g, byte b) = Parse(hex);
        return 0.2126 * Linear(r) + 0.7152 * Linear(g) + 0.0722 * Linear(b);
    }

    /// <summary>
    /// The accent colour itself when it is readable as text on the background; otherwise the nearest shade of it that is, found by moving it toward black
    /// (on a light background) or white (on a dark one) in steps of 5%. A yellow accent is lovely on a button and unreadable as a word on white.
    /// </summary>
    public static string Readable(string accent, string background, double minimum = BodyTextMinimum)
    {
        if (Ratio(accent, background) >= minimum)
        {
            return Normalize(accent);
        }

        bool lightBackground = Luminance(background) > 0.5;
        (byte r, byte g, byte b) = Parse(accent);
        for (int step = 1; step <= 20; step++)
        {
            double t = step * 0.05;
            byte Move(byte c) => (byte)Math.Round(lightBackground ? c * (1 - t) : c + (255 - c) * t);
            string candidate = $"#{Move(r):X2}{Move(g):X2}{Move(b):X2}";
            if (Ratio(candidate, background) >= minimum)
            {
                return candidate;
            }
        }

        return lightBackground ? "#000000" : "#FFFFFF";
    }

    /// <summary>
    /// What to put on an accent-coloured button: white text when the accent is deep enough, otherwise the accent darkened by up to 25% so white text
    /// is readable, otherwise (a light accent like amber) dark text on the accent as it is.
    /// </summary>
    public static (string Fill, string OnFill) ForFill(string accent)
    {
        const string white = "#FFFFFF";
        const string dark = "#262626";
        if (Ratio(white, accent) >= BodyTextMinimum)
        {
            return (Normalize(accent), white);
        }

        (byte r, byte g, byte b) = Parse(accent);
        for (int step = 1; step <= 5; step++)
        {
            double t = step * 0.05;
            string candidate = $"#{(byte)Math.Round(r * (1 - t)):X2}{(byte)Math.Round(g * (1 - t)):X2}{(byte)Math.Round(b * (1 - t)):X2}";
            if (Ratio(white, candidate) >= BodyTextMinimum)
            {
                return (candidate, white);
            }
        }

        return Ratio(dark, accent) >= BodyTextMinimum ? (Normalize(accent), dark) : (Readable(accent, white), white);
    }

    private static string Normalize(string hex)
    {
        (byte r, byte g, byte b) = Parse(hex);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    public static (byte R, byte G, byte B) Parse(string hex)
    {
        string h = hex.TrimStart('#');
        if (h.Length == 8)
        {
            h = h[2..];
        }

        return (
            byte.Parse(h[0..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(h[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture),
            byte.Parse(h[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
    }

    private static double Linear(byte channel)
    {
        double c = channel / 255.0;
        return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }
}
