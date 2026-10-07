namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public enum BreathPhaseKind
{
    Inhale,
    HoldFull,
    Exhale,
    HoldEmpty
}

/// <summary>One step of a breathing cycle: what to do, for how long, and how big the circle is at its start and end (0..1).</summary>
public sealed record BreathPhase(BreathPhaseKind Kind, int Seconds, double ScaleFrom, double ScaleTo);

/// <summary>Where the person is in the exercise at a given moment.</summary>
public sealed record BreathPosition(int Cycle, BreathPhase Phase, int SecondsLeft, double Progress, bool Finished);

/// <summary>
/// The rhythm of a breathing exercise, kept apart from the screen so it can be tested: square breathing is four counts in, four held,
/// four out, four held, repeated four times. The circle grows while breathing in, stays large while holding, shrinks while breathing out.
/// </summary>
public sealed class BreathingPattern
{
    public const double SmallScale = 0.55;
    public const double LargeScale = 1.0;

    public BreathingPattern(IReadOnlyList<BreathPhase> phases, int cycles)
    {
        Phases = phases;
        Cycles = cycles;
    }

    public IReadOnlyList<BreathPhase> Phases { get; }

    public int Cycles { get; }

    public int CycleSeconds => Phases.Sum(p => p.Seconds);

    public int TotalSeconds => CycleSeconds * Cycles;

    /// <summary>Square (box) breathing, four counts each, four cycles.</summary>
    public static BreathingPattern Square { get; } = new(
        [
            new BreathPhase(BreathPhaseKind.Inhale, 4, SmallScale, LargeScale),
            new BreathPhase(BreathPhaseKind.HoldFull, 4, LargeScale, LargeScale),
            new BreathPhase(BreathPhaseKind.Exhale, 4, LargeScale, SmallScale),
            new BreathPhase(BreathPhaseKind.HoldEmpty, 4, SmallScale, SmallScale)
        ],
        4);

    /// <summary>The position after <paramref name="elapsedSeconds"/> from the start (negative counts as zero).</summary>
    public BreathPosition At(double elapsedSeconds)
    {
        double t = Math.Max(0, elapsedSeconds);
        if (t >= TotalSeconds)
        {
            return new BreathPosition(Cycles, Phases[^1], 0, 1, Finished: true);
        }

        int cycle = (int)(t / CycleSeconds);
        double inCycle = t - cycle * CycleSeconds;
        foreach (BreathPhase phase in Phases)
        {
            if (inCycle < phase.Seconds)
            {
                return new BreathPosition(cycle + 1, phase, (int)Math.Ceiling(phase.Seconds - inCycle), inCycle / phase.Seconds, Finished: false);
            }

            inCycle -= phase.Seconds;
        }

        return new BreathPosition(Cycles, Phases[^1], 0, 1, Finished: true);
    }

    /// <summary>Size of the circle (0..1) at a point of a phase, eased so the movement starts and ends gently.</summary>
    public static double ScaleAt(BreathPhase phase, double progress)
    {
        double p = Math.Clamp(progress, 0, 1);
        double eased = 0.5 - Math.Cos(p * Math.PI) / 2;
        return phase.ScaleFrom + (phase.ScaleTo - phase.ScaleFrom) * eased;
    }

    /// <summary>How far through the whole exercise the person is, 0..1: the colour moves from tense to calm with it.</summary>
    public double CalmAt(double elapsedSeconds) => Math.Clamp(elapsedSeconds / TotalSeconds, 0, 1);
}
