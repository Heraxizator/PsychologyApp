namespace PsychologyApp.Presentation.Core.Charts;

public enum SkeletonKind
{
    /// <summary>Cards with a title, two lines of text and a small note: practices, tests.</summary>
    Cards,

    /// <summary>Rows with a round picture and two lines: history, quotes, results.</summary>
    Rows,

    /// <summary>Chat bubbles, alternating sides.</summary>
    Bubbles,

    /// <summary>A round picture and a name, then two cards: the companion profile.</summary>
    Profile
}

public enum SkeletonAlign
{
    Start,
    Center,
    End
}

/// <summary>One grey placeholder: how much of the width it takes (0..1), how tall it is, where it sits, and whether it is round.</summary>
public sealed record SkeletonBlock(double WidthFraction, double Height, SkeletonAlign Align = SkeletonAlign.Start, bool Circle = false);

/// <summary>Placeholders that belong together: stacked in a card, or beside a round picture.</summary>
public sealed record SkeletonGroup(IReadOnlyList<SkeletonBlock> Blocks, bool InCard = false, bool LeadingCircle = false);

/// <summary>
/// The shape of the grey placeholders shown while a screen loads, kept apart from the screen so it can be tested. A skeleton says "this is
/// what will appear here", so each kind copies the real content's layout; it is fixed (no randomness) so it never flickers between loads.
/// </summary>
public static class SkeletonLayout
{
    public const int MaxCount = 12;

    private static readonly double[] BubbleWidths = [0.62, 0.48, 0.74, 0.55, 0.68, 0.42];
    private static readonly double[] BubbleHeights = [64, 44, 80, 44, 64, 44];

    public static IReadOnlyList<SkeletonGroup> Build(SkeletonKind kind, int count)
    {
        int n = Math.Clamp(count, 1, MaxCount);
        return kind switch
        {
            SkeletonKind.Cards => [.. Enumerable.Range(0, n).Select(_ => Card())],
            SkeletonKind.Rows => [.. Enumerable.Range(0, n).Select(_ => Row())],
            SkeletonKind.Bubbles => [.. Enumerable.Range(0, n).Select(Bubble)],
            _ => Profile()
        };
    }

    public static int DefaultCount(SkeletonKind kind) => kind switch
    {
        SkeletonKind.Cards => 4,
        SkeletonKind.Rows => 7,
        SkeletonKind.Bubbles => 6,
        _ => 1
    };

    private static SkeletonGroup Card() => new(
        [new SkeletonBlock(0.55, 18), new SkeletonBlock(0.92, 12), new SkeletonBlock(0.72, 12), new SkeletonBlock(0.3, 10)],
        InCard: true);

    private static SkeletonGroup Row() => new(
        [new SkeletonBlock(0.6, 14), new SkeletonBlock(0.85, 10)],
        LeadingCircle: true);

    private static SkeletonGroup Bubble(int index) => new(
        [new SkeletonBlock(BubbleWidths[index % BubbleWidths.Length], BubbleHeights[index % BubbleHeights.Length], index % 2 == 0 ? SkeletonAlign.Start : SkeletonAlign.End)]);

    private static IReadOnlyList<SkeletonGroup> Profile() =>
    [
        new([new SkeletonBlock(0, 88, SkeletonAlign.Center, Circle: true), new SkeletonBlock(0.4, 18, SkeletonAlign.Center)]),
        new([new SkeletonBlock(0.5, 16), new SkeletonBlock(0.95, 12), new SkeletonBlock(0.8, 12)], InCard: true),
        new([new SkeletonBlock(0.4, 16), new SkeletonBlock(0.95, 12), new SkeletonBlock(0.9, 12), new SkeletonBlock(0.6, 12)], InCard: true)
    ];

    /// <summary>The placeholder grey: light on the light theme, a little lighter than the surface on the dark one, so it reads as "empty" and never as text.</summary>
    public static string FillHex(bool dark) => dark ? "#2C2F34" : "#E4E7EB";
}
