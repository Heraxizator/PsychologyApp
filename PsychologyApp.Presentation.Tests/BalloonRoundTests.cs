using PsychologyApp.Presentation.Core.Practice;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class BalloonRoundTests
{
    private const double Width = 400;
    private const double Height = 800;

    private static void Run(BalloonRound round, double seconds)
    {
        for (double t = 0; t < seconds; t += 0.05)
        {
            round.Update(0.05);
        }
    }

    // An endless animation drains the battery and keeps screen readers waiting: the round must stop by itself.
    [Fact]
    public void An_untouched_round_ends_with_every_balloon_escaped()
    {
        BalloonRound round = new(count: 5, random: new Random(1));

        Run(round, 30);

        Assert.True(round.IsOver);
        Assert.Equal(5, round.Escaped);
        Assert.Equal(0, round.Popped);
    }

    [Fact]
    public void A_tap_on_a_balloon_pops_it_and_a_tap_beside_it_does_not()
    {
        BalloonRound round = new(count: 1, random: new Random(1));
        Run(round, 1.5);
        Balloon balloon = Assert.Single(round.Balloons);

        Assert.False(round.TryPop(balloon.X * Width + Width * 0.4, balloon.Y * Height, Width, Height));
        Assert.True(round.TryPop(balloon.X * Width + 5, balloon.Y * Height - 5, Width, Height));
        Assert.False(round.TryPop(balloon.X * Width, balloon.Y * Height, Width, Height));
        Assert.Equal(1, round.Popped);
    }

    [Fact]
    public void The_round_waits_for_the_last_burst_before_it_ends()
    {
        BalloonRound round = new(count: 1, reduceMotion: true);
        Balloon balloon = round.Balloons[0];
        round.TryPop(balloon.X * Width, balloon.Y * Height, Width, Height);

        Assert.False(round.IsOver);
        round.Update(BalloonRound.BurstSeconds);
        Assert.True(round.IsOver);
    }

    [Fact]
    public void With_reduced_motion_the_balloons_stand_still_and_wait()
    {
        BalloonRound round = new(count: 8, reduceMotion: true);
        double[] before = [.. round.Balloons.Select(balloon => balloon.Y)];

        Run(round, 60);

        Assert.Equal(before, round.Balloons.Select(balloon => balloon.Y));
        Assert.Equal(0, round.Escaped);
        Assert.False(round.IsOver);
    }

    [Fact]
    public void A_screen_reader_can_pop_every_balloon_without_aiming()
    {
        BalloonRound round = new(count: 3, reduceMotion: true);

        while (round.PopNext())
        {
        }

        round.Update(BalloonRound.BurstSeconds);
        Assert.Equal(3, round.Popped);
        Assert.True(round.IsOver);
    }
}
