using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

/// <param name="WhatHappened">What actually happened, in the person's words.</param>
/// <param name="BeliefAfter">How sure the person is now of the same prediction, 0..100%.</param>
/// <param name="PeakDistress">The highest distress during the task, 0..100.</param>
/// <param name="Learned">What the person learned, one sentence.</param>
public sealed record ExposureReport(
    string WhatHappened,
    PredictionOutcome Outcome,
    int BeliefAfter,
    int PeakDistress,
    bool UsedSafetyBehaviour,
    string Learned);

/// <param name="Crisis">The words read as a crisis: the screen leads to help instead of the next step.</param>
public sealed record ExposureReportResult(ExposurePlan Plan, bool Crisis);

public interface IExposureService
{
    Task<ExposurePlan?> GetPlanAsync(CancellationToken cancellationToken = default);

    /// <summary>Records the safety screening; creates the plan on the first one.</summary>
    Task<ExposureScreeningResult> ScreenAsync(ExposureScreeningAnswers answers, CancellationToken cancellationToken = default);

    Task<ExposurePlan> SaveFormulationAsync(
        FearKind kind,
        string fearedOutcome,
        int belief,
        IReadOnlyList<string> avoidedSituations,
        IReadOnlyList<string> safetyBehaviours,
        CancellationToken cancellationToken = default);

    /// <summary>Replaces the ladder. Steps keep their ids, so the tasks done on them stay attached.</summary>
    Task<ExposurePlan> SaveStepsAsync(IReadOnlyList<ExposureStep> steps, CancellationToken cancellationToken = default);

    Task<ExposureSession> StartTaskAsync(
        string stepId,
        string prediction,
        int beliefBefore,
        IReadOnlyList<string> droppedSafetyBehaviours,
        CancellationToken cancellationToken = default);

    Task<ExposureReportResult> ReportTaskAsync(string sessionId, ExposureReport report, CancellationToken cancellationToken = default);

    /// <summary>Ends a task early. Stopping is allowed and is not a failure; it only tells the ladder the step may be too steep.</summary>
    Task<ExposurePlan> InterruptTaskAsync(string sessionId, int? peakDistress, CancellationToken cancellationToken = default);

    /// <summary>Deletes the ladder and everything recorded on it.</summary>
    Task DeletePlanAsync(CancellationToken cancellationToken = default);
}

public sealed class ExposureService(IExposureRepository repository, ICrisisDetector crisisDetector, TimeProvider? timeProvider = null) : IExposureService
{
    public const int MaxStepText = 200;
    public const int MaxFreeText = 600;

    private readonly TimeProvider _time = timeProvider ?? TimeProvider.System;

    // The plan is one document: two quick taps must not each read the old version and overwrite the other's change.
    private readonly SemaphoreSlim _writeGate = new(1, 1);

    public Task<ExposurePlan?> GetPlanAsync(CancellationToken cancellationToken = default) => repository.GetAsync(cancellationToken);

    public async Task<ExposureScreeningResult> ScreenAsync(ExposureScreeningAnswers answers, CancellationToken cancellationToken = default)
    {
        ExposureScreeningResult result = ExposureScreening.Decide(answers);
        await UpdateAsync(plan => plan with { Screening = result, ScreenedAt = Now }, cancellationToken).ConfigureAwait(false);
        return result;
    }

    public Task<ExposurePlan> SaveFormulationAsync(
        FearKind kind,
        string fearedOutcome,
        int belief,
        IReadOnlyList<string> avoidedSituations,
        IReadOnlyList<string> safetyBehaviours,
        CancellationToken cancellationToken = default) =>
        UpdateAsync(plan => plan with
        {
            Kind = kind,
            FearedOutcome = Clean(fearedOutcome, MaxFreeText),
            FearedOutcomeBelief = Percent(belief),
            AvoidedSituations = CleanList(avoidedSituations),
            SafetyBehaviours = CleanList(safetyBehaviours)
        }, cancellationToken);

    public Task<ExposurePlan> SaveStepsAsync(IReadOnlyList<ExposureStep> steps, CancellationToken cancellationToken = default) =>
        UpdateAsync(plan => plan with
        {
            Steps = steps
                .Select(step => step with
                {
                    Id = string.IsNullOrWhiteSpace(step.Id) ? NewId() : step.Id,
                    Text = Clean(step.Text, MaxStepText),
                    ExpectedDistress = Percent(step.ExpectedDistress)
                })
                .Where(step => step.Text.Length > 0)
                .GroupBy(step => step.Id)
                .Select(group => group.First())
                .ToList()
        }, cancellationToken);

    public async Task<ExposureSession> StartTaskAsync(
        string stepId,
        string prediction,
        int beliefBefore,
        IReadOnlyList<string> droppedSafetyBehaviours,
        CancellationToken cancellationToken = default)
    {
        ExposureSession session = new()
        {
            Id = NewId(),
            StepId = stepId,
            CreatedAt = Now,
            Prediction = Clean(prediction, MaxFreeText),
            BeliefBefore = Percent(beliefBefore),
            DroppedSafetyBehaviours = CleanList(droppedSafetyBehaviours)
        };

        await UpdateAsync(plan =>
        {
            if (plan.Steps.All(step => step.Id != stepId))
            {
                throw new InvalidOperationException($"The ladder has no step '{stepId}'.");
            }

            return plan with { Sessions = [.. plan.Sessions, session] };
        }, cancellationToken).ConfigureAwait(false);
        return session;
    }

    public async Task<ExposureReportResult> ReportTaskAsync(string sessionId, ExposureReport report, CancellationToken cancellationToken = default)
    {
        string happened = Clean(report.WhatHappened, MaxFreeText);
        string learned = Clean(report.Learned, MaxFreeText);
        ExposurePlan plan = await ReplaceSessionAsync(sessionId, session => session with
        {
            ReportedAt = Now,
            WhatHappened = happened,
            Outcome = report.Outcome,
            BeliefAfter = Percent(report.BeliefAfter),
            PeakDistress = Percent(report.PeakDistress),
            UsedSafetyBehaviour = report.UsedSafetyBehaviour,
            Learned = learned,
            Interrupted = false
        }, cancellationToken).ConfigureAwait(false);

        // Check the original answers: storage truncation must not remove a crisis signal.
        return new ExposureReportResult(plan, crisisDetector.IsCrisis(report.WhatHappened) || crisisDetector.IsCrisis(report.Learned));
    }

    public Task<ExposurePlan> InterruptTaskAsync(string sessionId, int? peakDistress, CancellationToken cancellationToken = default) =>
        ReplaceSessionAsync(sessionId, session => session with
        {
            ReportedAt = Now,
            Interrupted = true,
            Outcome = null,
            PeakDistress = peakDistress is { } peak ? Percent(peak) : null
        }, cancellationToken);

    public async Task DeletePlanAsync(CancellationToken cancellationToken = default)
    {
        await _writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await repository.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _writeGate.Release();
        }
    }

    private Task<ExposurePlan> ReplaceSessionAsync(string sessionId, Func<ExposureSession, ExposureSession> change, CancellationToken cancellationToken) =>
        UpdateAsync(plan =>
        {
            if (plan.Sessions.All(session => session.Id != sessionId))
            {
                throw new InvalidOperationException($"The ladder has no task '{sessionId}'.");
            }

            return plan with { Sessions = plan.Sessions.Select(session => session.Id == sessionId ? change(session) : session).ToList() };
        }, cancellationToken);

    private async Task<ExposurePlan> UpdateAsync(Func<ExposurePlan, ExposurePlan> change, CancellationToken cancellationToken)
    {
        await _writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ExposurePlan current = await repository.GetAsync(cancellationToken).ConfigureAwait(false) ?? new ExposurePlan { CreatedAt = Now };
            ExposurePlan changed = change(current);
            await repository.SaveAsync(changed, cancellationToken).ConfigureAwait(false);
            return changed;
        }
        finally
        {
            _writeGate.Release();
        }
    }

    private DateTime Now => _time.GetUtcNow().UtcDateTime;

    private static string NewId() => Guid.NewGuid().ToString("N");

    private static int Percent(int value) => Math.Clamp(value, 0, 100);

    private static string Clean(string? text, int maxLength)
    {
        string trimmed = (text ?? string.Empty).Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength].TrimEnd();
    }

    private static List<string> CleanList(IReadOnlyList<string> items) =>
        items.Select(item => Clean(item, MaxStepText))
            .Where(item => item.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
