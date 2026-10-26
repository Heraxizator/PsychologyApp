namespace PsychologyApp.Presentation.Core.Practice;

public enum MuscleGroup
{
    Hands,
    Shoulders,
    Face,
    Belly,
    Legs
}

/// <summary>Where the person is in the relaxation: which group, whether to tense or let go, and how long is left of that step.</summary>
public sealed record MusclePosition(int GroupNumber, MuscleGroup Group, bool Tense, int SecondsLeft, bool Finished);

/// <summary>
/// A short progressive muscle relaxation, kept apart from the screen so it can be tested: each group is tensed for five seconds and let go
/// for fifteen. Letting go is three times longer on purpose: the relaxation after the contrast is what the person learns to notice.
/// </summary>
public static class MuscleRelaxation
{
    public const int TenseSeconds = 5;
    public const int ReleaseSeconds = 15;

    public static IReadOnlyList<MuscleGroup> Groups { get; } = [MuscleGroup.Hands, MuscleGroup.Shoulders, MuscleGroup.Face, MuscleGroup.Belly, MuscleGroup.Legs];

    public static int GroupSeconds => TenseSeconds + ReleaseSeconds;

    public static int TotalSeconds => GroupSeconds * Groups.Count;

    /// <summary>The position after <paramref name="elapsedSeconds"/> from the start (negative counts as zero).</summary>
    public static MusclePosition At(double elapsedSeconds)
    {
        double t = Math.Max(0, elapsedSeconds);
        if (t >= TotalSeconds)
        {
            return new MusclePosition(Groups.Count, Groups[^1], Tense: false, 0, Finished: true);
        }

        int index = (int)(t / GroupSeconds);
        double inGroup = t - index * GroupSeconds;
        bool tense = inGroup < TenseSeconds;
        double left = tense ? TenseSeconds - inGroup : GroupSeconds - inGroup;
        return new MusclePosition(index + 1, Groups[index], tense, (int)Math.Ceiling(left), Finished: false);
    }
}
