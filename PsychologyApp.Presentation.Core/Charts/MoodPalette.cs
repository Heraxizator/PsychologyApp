namespace PsychologyApp.Presentation.Core.Charts;

/// <summary>
/// Colours of the five mood levels (1 low .. 5 good) for the journal calendar: warm for a hard day, calm teal for a good one, the same
/// direction as the tension scale. The light set is pale with dark text; the dark set is deep and muted with light text, so neither glares
/// and the day number stays readable on every level (tested against WCAG 4.5:1).
/// </summary>
public static class MoodPalette
{
    private static readonly string[] LightFills = ["#F2B8B0", "#F6CFA3", "#E6E2D3", "#BFE3D6", "#8FD1BE"];
    private static readonly string[] DarkFills = ["#7A3B37", "#7A5A32", "#4A4A44", "#2F6B5C", "#1F6E5C"];

    public const string LightText = "#262626";
    public const string DarkText = "#F2F2F2";
    public const string LightEmptyFill = "#FFFFFF";
    public const string DarkEmptyFill = "#1E1E1E";

    /// <summary>The fill of a day with this mood; days without a note get the plain surface.</summary>
    public static string Fill(int? level, bool dark)
    {
        if (level is not { } l)
        {
            return dark ? DarkEmptyFill : LightEmptyFill;
        }

        int index = Math.Clamp(l, 1, 5) - 1;
        return dark ? DarkFills[index] : LightFills[index];
    }

    public static string Text(bool dark) => dark ? DarkText : LightText;
}
