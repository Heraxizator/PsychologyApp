namespace PsychologyApp.Domain.Exposure;

/// <param name="Reported">Tasks with a report (interrupted ones included).</param>
/// <param name="PredictionsTested">Reported tasks with an outcome: the predictions actually checked.</param>
/// <param name="PredictionsCameTrue">Of those, how many came true in full.</param>
/// <param name="StepsMastered">Steps of the ladder that no longer predict harm.</param>
/// <param name="AverageBeliefDrop">Mean drop of belief in the prediction from before to after a task, in points; null before the first report.</param>
public sealed record ExposureProgress(
    int Reported,
    int PredictionsTested,
    int PredictionsCameTrue,
    int StepsMastered,
    int StepsTotal,
    double? AverageBeliefDrop);

/// <summary>
/// How the person moves up the fear ladder, following inhibitory learning (Craske et al., 2014): a step is not done when the fear
/// is gone, but when the fear has stopped predicting harm. See docs/exposure-engine.md.
/// </summary>
public static class ExposureLadder
{
    /// <summary>Tasks on a step where the prediction did not fully come true, needed to call the step mastered.</summary>
    public const int DisconfirmedTasksToMaster = 2;

    /// <summary>The belief in the prediction after the last task must be this low or lower.</summary>
    public const int MasteredBeliefMax = 30;

    /// <summary>A peak of distress this high in the last tasks means the step is too steep for now.</summary>
    public const int OverwhelmingDistress = 90;

    public static IReadOnlyList<ExposureStep> Ordered(ExposurePlan plan) =>
        plan.Steps
            .Select((step, index) => (step, index))
            .OrderBy(item => item.step.ExpectedDistress)
            .ThenBy(item => item.index)
            .Select(item => item.step)
            .ToList();

    public static IReadOnlyList<ExposureSession> SessionsOf(ExposurePlan plan, string stepId) =>
        plan.Sessions.Where(session => session.StepId == stepId).OrderBy(session => session.CreatedAt).ToList();

    /// <summary>
    /// Mastered: at least <see cref="DisconfirmedTasksToMaster"/> tasks where the prediction did not fully come true, and the last
    /// reported task was done without the safety behaviour, with the belief down to <see cref="MasteredBeliefMax"/>% or lower.
    /// Distress is not part of the rule on purpose: waiting for the fear to vanish teaches that the fear had to be waited out.
    /// </summary>
    public static bool IsMastered(ExposurePlan plan, ExposureStep step)
    {
        IReadOnlyList<ExposureSession> tested = SessionsOf(plan, step.Id).Where(session => session.Outcome is not null).ToList();
        if (tested.Count(session => session.Outcome != PredictionOutcome.Happened) < DisconfirmedTasksToMaster)
        {
            return false;
        }

        ExposureSession last = tested[^1];
        return !last.UsedSafetyBehaviour && last.BeliefAfter is { } belief && belief <= MasteredBeliefMax;
    }

    /// <summary>The lowest step not yet mastered; null when the whole ladder is done.</summary>
    public static ExposureStep? CurrentStep(ExposurePlan plan) =>
        Ordered(plan).FirstOrDefault(step => !IsMastered(plan, step));

    /// <summary>A task begun but not reported yet: the next screen asks how it went instead of starting a new one.</summary>
    public static ExposureSession? Pending(ExposurePlan plan) =>
        plan.Sessions.Where(session => !session.IsReported).OrderByDescending(session => session.CreatedAt).FirstOrDefault();

    /// <summary>
    /// The step is too steep for now: the last two reported tasks on it were both interrupted or both peaked at
    /// <see cref="OverwhelmingDistress"/> or more. The screen then offers an easier step in between, without blame.
    /// </summary>
    public static bool NeedsEasierStep(ExposurePlan plan, ExposureStep step)
    {
        List<ExposureSession> reported = SessionsOf(plan, step.Id).Where(session => session.IsReported).ToList();
        return reported.Count >= 2
               && reported.TakeLast(2).All(session => session.Interrupted || session.PeakDistress >= OverwhelmingDistress);
    }

    /// <summary>How much the task weakened the prediction: belief before minus belief after; null without a report.</summary>
    public static int? BeliefDrop(ExposureSession session) =>
        session.BeliefAfter is { } after && session.Outcome is not null ? session.BeliefBefore - after : null;

    public static ExposureProgress Progress(ExposurePlan plan)
    {
        List<ExposureSession> reported = plan.Sessions.Where(session => session.IsReported).ToList();
        List<ExposureSession> tested = reported.Where(session => session.Outcome is not null).ToList();
        List<int> drops = tested.Select(BeliefDrop).OfType<int>().ToList();
        return new ExposureProgress(
            reported.Count,
            tested.Count,
            tested.Count(session => session.Outcome == PredictionOutcome.Happened),
            plan.Steps.Count(step => IsMastered(plan, step)),
            plan.Steps.Count,
            drops.Count == 0 ? null : drops.Average());
    }
}
