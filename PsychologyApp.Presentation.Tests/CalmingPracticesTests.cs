using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Core.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The rhythms of the cyclic sigh, the muscle relaxation and "keep the mind busy": what the person is guided through, and that each ends.</summary>
public sealed class CalmingPracticesTests
{
    private static readonly BreathingPattern Sigh = BreathingPattern.CyclicSigh;

    // The technique is a double breath in and a long breath out, with no hold: a hold would make it box breathing again.
    [Fact]
    public void The_cyclic_sigh_is_a_breath_in_a_second_one_on_top_and_a_longer_breath_out()
    {
        Assert.Equal([BreathPhaseKind.Inhale, BreathPhaseKind.TopUp, BreathPhaseKind.Exhale], Sigh.Phases.Select(phase => phase.Kind));
        int breathIn = Sigh.Phases.Where(phase => phase.Kind != BreathPhaseKind.Exhale).Sum(phase => phase.Seconds);
        Assert.True(Sigh.Phases[^1].Seconds >= breathIn * 2);
        Assert.Equal(180, Sigh.TotalSeconds);
    }

    [Fact]
    public void The_second_breath_in_keeps_filling_the_circle_from_where_the_first_stopped()
    {
        Assert.Equal(Sigh.Phases[0].ScaleTo, Sigh.Phases[1].ScaleFrom);
        Assert.True(Sigh.Phases[1].ScaleTo > Sigh.Phases[1].ScaleFrom);
        Assert.True(Sigh.At(Sigh.TotalSeconds).Finished);
    }

    [Theory]
    [InlineData(0, 1, MuscleGroup.Hands, true, 5)]
    [InlineData(4.5, 1, MuscleGroup.Hands, true, 1)]
    [InlineData(5, 1, MuscleGroup.Hands, false, 15)]
    [InlineData(19.9, 1, MuscleGroup.Hands, false, 1)]
    [InlineData(20, 2, MuscleGroup.Shoulders, true, 5)]
    [InlineData(85, 5, MuscleGroup.Legs, false, 15)]
    public void The_relaxation_tenses_each_group_and_then_lets_it_go(double seconds, int number, MuscleGroup group, bool tense, int left)
    {
        MusclePosition position = MuscleRelaxation.At(seconds);

        Assert.Equal(number, position.GroupNumber);
        Assert.Equal(group, position.Group);
        Assert.Equal(tense, position.Tense);
        Assert.Equal(left, position.SecondsLeft);
        Assert.False(position.Finished);
    }

    [Fact]
    public void The_relaxation_lets_go_longer_than_it_tenses_and_ends_after_the_last_group()
    {
        Assert.True(MuscleRelaxation.ReleaseSeconds > MuscleRelaxation.TenseSeconds);
        Assert.Equal(MuscleRelaxation.Groups.Count, MuscleRelaxation.Groups.Distinct().Count());
        Assert.True(MuscleRelaxation.At(MuscleRelaxation.TotalSeconds).Finished);
        Assert.Equal(1, MuscleRelaxation.At(-3).GroupNumber);
    }

    // The round is bounded so the practice hands the person back to the rating; a task seen once is not shown again in it.
    [Fact]
    public void A_round_of_tasks_ends_after_its_length_without_repeating_a_task()
    {
        BusyMindRound round = new(random: new Random(3));
        List<int> seen = [round.Current];

        round.Skip();
        seen.Add(round.Current);
        Assert.Equal(1, round.Step);

        while (true)
        {
            round.Next();
            if (round.IsOver)
            {
                break;
            }

            seen.Add(round.Current);
        }

        Assert.Equal(BusyMindRound.DefaultLength, round.Step);
        Assert.Equal(seen.Count, seen.Distinct().Count());
        Assert.All(seen, task => Assert.InRange(task, 1, BusyMindRound.TaskCount));
    }

    [Fact]
    public void Skipping_every_task_runs_out_instead_of_showing_one_again()
    {
        BusyMindRound round = new(random: new Random(5));
        int skips = 0;
        while (round.Skip())
        {
            skips++;
        }

        Assert.Equal(BusyMindRound.TaskCount - 1, skips);
        round.Next();
        Assert.True(round.IsOver);
    }

    // A task number without a text would show its resource key to the person.
    [Theory]
    [InlineData("ru")]
    [InlineData("en")]
    [InlineData("zh")]
    public void Every_task_has_a_text(string language)
    {
        for (int number = 1; number <= BusyMindRound.TaskCount; number++)
        {
            string key = $"BusyMindTask{number:00}";
            Assert.NotEqual(key, AppStrings.In(language, key));
        }
    }
}
