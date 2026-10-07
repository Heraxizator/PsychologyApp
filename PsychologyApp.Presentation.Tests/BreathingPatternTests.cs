using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The rhythm of the breathing circle: counts, phases, cycles and the size of the circle.</summary>
public class BreathingPatternTests
{
    private static readonly BreathingPattern Square = BreathingPattern.Square;

    [Fact]
    public void SquareBreathingIsFourCountsFourTimesFourCycles()
    {
        Assert.Equal(16, Square.CycleSeconds);
        Assert.Equal(64, Square.TotalSeconds);
        Assert.Equal(4, Square.Cycles);
    }

    [Theory]
    [InlineData(0, BreathPhaseKind.Inhale, 1, 4)]
    [InlineData(3.5, BreathPhaseKind.Inhale, 1, 1)]
    [InlineData(4, BreathPhaseKind.HoldFull, 1, 4)]
    [InlineData(8, BreathPhaseKind.Exhale, 1, 4)]
    [InlineData(12, BreathPhaseKind.HoldEmpty, 1, 4)]
    [InlineData(16, BreathPhaseKind.Inhale, 2, 4)]
    [InlineData(63.9, BreathPhaseKind.HoldEmpty, 4, 1)]
    public void ThePositionFollowsTheClock(double seconds, BreathPhaseKind kind, int cycle, int left)
    {
        BreathPosition position = Square.At(seconds);

        Assert.Equal((kind, cycle, left, false), (position.Phase.Kind, position.Cycle, position.SecondsLeft, position.Finished));
    }

    [Fact]
    public void TheExerciseFinishesAfterTheLastCycle()
    {
        BreathPosition done = Square.At(64);

        Assert.True(done.Finished);
        Assert.Equal(4, done.Cycle);
        Assert.True(Square.At(500).Finished);
    }

    [Fact]
    public void ANegativeClockCountsAsTheStart()
    {
        Assert.Equal(BreathPhaseKind.Inhale, Square.At(-5).Phase.Kind);
    }

    [Fact]
    public void TheCircleGrowsOnTheInhaleStaysLargeOnTheHoldAndShrinksOnTheExhale()
    {
        BreathPhase inhale = Square.Phases[0];
        BreathPhase hold = Square.Phases[1];
        BreathPhase exhale = Square.Phases[2];

        Assert.Equal(BreathingPattern.SmallScale, BreathingPattern.ScaleAt(inhale, 0), 6);
        Assert.Equal(BreathingPattern.LargeScale, BreathingPattern.ScaleAt(inhale, 1), 6);
        Assert.True(BreathingPattern.ScaleAt(inhale, 0.5) is > BreathingPattern.SmallScale and < BreathingPattern.LargeScale);
        Assert.Equal(BreathingPattern.LargeScale, BreathingPattern.ScaleAt(hold, 0.5), 6);
        Assert.Equal(BreathingPattern.SmallScale, BreathingPattern.ScaleAt(exhale, 1), 6);
    }

    [Fact]
    public void TheMovementIsEasedSoItStartsAndEndsSlowly()
    {
        BreathPhase inhale = Square.Phases[0];
        double early = BreathingPattern.ScaleAt(inhale, 0.1) - BreathingPattern.ScaleAt(inhale, 0);
        double middle = BreathingPattern.ScaleAt(inhale, 0.55) - BreathingPattern.ScaleAt(inhale, 0.45);

        Assert.True(early < middle);
    }

    [Fact]
    public void CalmGrowsFromZeroToOneOverTheExercise()
    {
        Assert.Equal(0, Square.CalmAt(0));
        Assert.Equal(0.5, Square.CalmAt(32), 6);
        Assert.Equal(1, Square.CalmAt(1000));
    }
}
