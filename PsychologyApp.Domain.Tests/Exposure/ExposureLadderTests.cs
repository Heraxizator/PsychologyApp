using PsychologyApp.Domain.Exposure;
using Xunit;

namespace PsychologyApp.Domain.Tests.Exposure;

public sealed class ExposureLadderTests
{
    private static readonly DateTime Start = new(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);

    private static readonly ExposureStep Shop = new("shop", "Ask a shop assistant a question", 30);
    private static readonly ExposureStep Call = new("call", "Call to book an appointment", 45);
    private static readonly ExposureStep Meeting = new("meeting", "Speak up at a meeting", 80);

    private static ExposurePlan Plan(params ExposureSession[] sessions) => new()
    {
        // Written out of order: the ladder is ordered by expected distress, not by input.
        Steps = [Meeting, Shop, Call],
        Sessions = sessions
    };

    private static int _sessionNumber;

    private static ExposureSession Task(
        ExposureStep step,
        PredictionOutcome? outcome,
        int beliefBefore = 80,
        int? beliefAfter = 40,
        bool safety = false,
        int? peak = 60,
        bool interrupted = false)
    {
        int number = Interlocked.Increment(ref _sessionNumber);
        return new ExposureSession
        {
            Id = $"s{number}",
            StepId = step.Id,
            CreatedAt = Start.AddHours(number),
            BeliefBefore = beliefBefore,
            Outcome = outcome,
            BeliefAfter = outcome is null ? null : beliefAfter,
            UsedSafetyBehaviour = safety,
            PeakDistress = peak,
            Interrupted = interrupted
        };
    }

    [Fact]
    public void The_ladder_starts_at_the_least_frightening_step()
    {
        Assert.Equal(Shop, ExposureLadder.CurrentStep(Plan()));
        Assert.Equal([Shop, Call, Meeting], ExposureLadder.Ordered(Plan()));
    }

    [Fact]
    public void A_step_is_mastered_after_two_disconfirmed_predictions_with_low_belief_and_no_safety_behaviour()
    {
        ExposurePlan plan = Plan(
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 50),
            Task(Shop, PredictionOutcome.Partly, beliefAfter: 25));

        Assert.True(ExposureLadder.IsMastered(plan, Shop));
        Assert.Equal(Call, ExposureLadder.CurrentStep(plan));
    }

    [Fact]
    public void One_disconfirmed_prediction_is_not_enough()
    {
        ExposurePlan plan = Plan(Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 10));

        Assert.False(ExposureLadder.IsMastered(plan, Shop));
    }

    [Fact]
    public void A_step_done_with_the_safety_behaviour_is_not_mastered_however_calm_it_felt()
    {
        // The "insurance" takes the credit: the brain learns "it went fine because I had my water", not "it was safe".
        ExposurePlan plan = Plan(
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 10),
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 5, safety: true, peak: 10));

        Assert.False(ExposureLadder.IsMastered(plan, Shop));
    }

    [Fact]
    public void A_step_is_not_mastered_while_the_person_still_believes_the_prediction()
    {
        ExposurePlan plan = Plan(
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 60),
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 55));

        Assert.False(ExposureLadder.IsMastered(plan, Shop));
    }

    [Fact]
    public void Distress_does_not_decide_mastery()
    {
        // Inhibitory learning: the step is done when the fear stops predicting harm, even if it is still frightening.
        ExposurePlan plan = Plan(
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 30, peak: 85),
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 20, peak: 80));

        Assert.True(ExposureLadder.IsMastered(plan, Shop));
    }

    [Fact]
    public void A_prediction_that_came_true_does_not_count_toward_mastery()
    {
        ExposurePlan plan = Plan(
            Task(Shop, PredictionOutcome.Happened, beliefAfter: 20),
            Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 20));

        Assert.False(ExposureLadder.IsMastered(plan, Shop));
    }

    [Fact]
    public void Two_overwhelming_tasks_in_a_row_ask_for_an_easier_step()
    {
        ExposurePlan plan = Plan(
            Task(Call, PredictionOutcome.Partly, peak: 95),
            Task(Call, null, interrupted: true, peak: null));

        Assert.True(ExposureLadder.NeedsEasierStep(plan, Call));
    }

    [Fact]
    public void One_hard_task_or_a_calmer_last_task_does_not_ask_for_an_easier_step()
    {
        Assert.False(ExposureLadder.NeedsEasierStep(Plan(Task(Call, PredictionOutcome.Partly, peak: 95)), Call));
        Assert.False(ExposureLadder.NeedsEasierStep(
            Plan(Task(Call, PredictionOutcome.Partly, peak: 95), Task(Call, PredictionOutcome.NotHappened, peak: 70)),
            Call));
    }

    [Fact]
    public void A_task_without_a_report_is_pending()
    {
        ExposureSession pending = Task(Call, null);
        ExposurePlan plan = Plan(Task(Shop, PredictionOutcome.NotHappened), pending);

        Assert.Equal(pending, ExposureLadder.Pending(plan));
        Assert.Null(ExposureLadder.Pending(Plan(Task(Shop, PredictionOutcome.NotHappened))));
    }

    [Fact]
    public void Progress_counts_tested_predictions_and_how_many_came_true()
    {
        ExposurePlan plan = Plan(
            Task(Shop, PredictionOutcome.NotHappened, beliefBefore: 80, beliefAfter: 30),
            Task(Shop, PredictionOutcome.NotHappened, beliefBefore: 50, beliefAfter: 20),
            Task(Call, PredictionOutcome.Happened, beliefBefore: 70, beliefAfter: 70),
            Task(Call, null, interrupted: true),
            Task(Meeting, null));

        ExposureProgress progress = ExposureLadder.Progress(plan);

        Assert.Equal(4, progress.Reported);
        Assert.Equal(3, progress.PredictionsTested);
        Assert.Equal(1, progress.PredictionsCameTrue);
        Assert.Equal(1, progress.StepsMastered);
        Assert.Equal(3, progress.StepsTotal);
        Assert.Equal((50 + 30 + 0) / 3.0, progress.AverageBeliefDrop);
    }

    [Fact]
    public void A_finished_ladder_has_no_current_step()
    {
        ExposurePlan plan = new()
        {
            Steps = [Shop],
            Sessions = [Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 20), Task(Shop, PredictionOutcome.NotHappened, beliefAfter: 10)]
        };

        Assert.Null(ExposureLadder.CurrentStep(plan));
    }
}
