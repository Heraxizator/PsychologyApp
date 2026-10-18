using PsychologyApp.Domain.Exposure;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>What the person hears after a task follows the ladder rules: never blame, always the next sensible move.</summary>
public sealed class ExposureInsightsTests
{
    private static readonly ExposureStep Call = new("call", "Call the clinic", 40);
    private static readonly ExposureStep Talk = new("talk", "Give a talk", 85);

    private static ExposureSession Task(string id, ExposureStep step, PredictionOutcome? outcome, int beliefAfter = 20, bool safety = false,
        int? peak = 50, bool interrupted = false, int hour = 0) => new()
    {
        Id = id,
        StepId = step.Id,
        CreatedAt = new DateTime(2026, 10, 1, hour, 0, 0, DateTimeKind.Utc),
        BeliefBefore = 80,
        Outcome = outcome,
        BeliefAfter = outcome is null ? null : beliefAfter,
        UsedSafetyBehaviour = safety,
        PeakDistress = peak,
        Interrupted = interrupted
    };

    private static ExposurePlan Plan(params ExposureSession[] sessions) => new() { Steps = [Call, Talk], Sessions = sessions };

    [Fact]
    public void A_disconfirmed_prediction_is_named_as_the_learning()
    {
        ExposureSession first = Task("a", Call, PredictionOutcome.NotHappened, beliefAfter: 50);

        Assert.Equal([ExposureInsight.Disconfirmed], ExposureInsights.After(Plan(first), first));
    }

    [Fact]
    public void A_prediction_that_came_true_gets_the_decatastrophising_questions()
    {
        ExposureSession task = Task("a", Call, PredictionOutcome.Happened);

        Assert.Equal([ExposureInsight.CameTrue], ExposureInsights.After(Plan(task), task));
    }

    [Fact]
    public void A_safety_behaviour_is_pointed_out()
    {
        ExposureSession task = Task("a", Call, PredictionOutcome.NotHappened, safety: true);

        Assert.Equal([ExposureInsight.Disconfirmed, ExposureInsight.SafetyBehaviourUsed], ExposureInsights.After(Plan(task), task));
    }

    [Fact]
    public void The_second_disconfirmation_masters_the_step()
    {
        ExposureSession first = Task("a", Call, PredictionOutcome.NotHappened, hour: 1);
        ExposureSession second = Task("b", Call, PredictionOutcome.Partly, beliefAfter: 10, hour: 2);

        Assert.Equal([ExposureInsight.Disconfirmed, ExposureInsight.StepMastered], ExposureInsights.After(Plan(first, second), second));
    }

    [Fact]
    public void Mastering_the_last_step_finishes_the_ladder()
    {
        ExposureSession[] sessions =
        [
            Task("a", Call, PredictionOutcome.NotHappened, hour: 1),
            Task("b", Call, PredictionOutcome.NotHappened, hour: 2),
            Task("c", Talk, PredictionOutcome.NotHappened, hour: 3),
            Task("d", Talk, PredictionOutcome.NotHappened, hour: 4)
        ];

        Assert.Contains(ExposureInsight.LadderDone, ExposureInsights.After(Plan(sessions), sessions[^1]));
    }

    [Fact]
    public void Stopping_twice_suggests_an_easier_step_without_blame()
    {
        ExposureSession first = Task("a", Talk, null, interrupted: true, peak: 95, hour: 1);
        ExposureSession second = Task("b", Talk, null, interrupted: true, peak: 92, hour: 2);

        Assert.Equal([ExposureInsight.Interrupted, ExposureInsight.NeedsEasierStep], ExposureInsights.After(Plan(first, second), second));
    }
}
