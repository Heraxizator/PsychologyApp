using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Presentation.Common;

/// <summary>What the person is told after an exposure task, in this order.</summary>
public enum ExposureInsight
{
    /// <summary>The prediction did not come true (fully or at all): the fear was wrong, and that is the learning.</summary>
    Disconfirmed,

    /// <summary>It came true: they lived through it; how bad was it really, what helped (decatastrophising).</summary>
    CameTrue,

    /// <summary>Stopping early is allowed; the step may be too steep for now.</summary>
    Interrupted,

    /// <summary>The safety behaviour takes the credit; next time try without it.</summary>
    SafetyBehaviourUsed,

    StepMastered,
    LadderDone,

    /// <summary>Two overwhelming tasks in a row: add a step in between.</summary>
    NeedsEasierStep
}

/// <summary>The feedback after a task, from the ladder rules (see docs/exposure-engine.md).</summary>
public static class ExposureInsights
{
    /// <param name="plan">The plan with <paramref name="session"/> already reported.</param>
    public static IReadOnlyList<ExposureInsight> After(ExposurePlan plan, ExposureSession session)
    {
        List<ExposureInsight> insights = [];
        if (session.Interrupted)
        {
            insights.Add(ExposureInsight.Interrupted);
        }
        else
        {
            insights.Add(session.Outcome == PredictionOutcome.Happened ? ExposureInsight.CameTrue : ExposureInsight.Disconfirmed);
            if (session.UsedSafetyBehaviour)
            {
                insights.Add(ExposureInsight.SafetyBehaviourUsed);
            }
        }

        ExposureStep? step = plan.Steps.FirstOrDefault(candidate => candidate.Id == session.StepId);
        if (step is null)
        {
            return insights;
        }

        if (ExposureLadder.IsMastered(plan, step))
        {
            insights.Add(ExposureLadder.CurrentStep(plan) is null ? ExposureInsight.LadderDone : ExposureInsight.StepMastered);
        }
        else if (ExposureLadder.NeedsEasierStep(plan, step))
        {
            insights.Add(ExposureInsight.NeedsEasierStep);
        }

        return insights;
    }
}
