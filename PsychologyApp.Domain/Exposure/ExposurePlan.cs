namespace PsychologyApp.Domain.Exposure;

/// <summary>What the fear is about; it chooses the examples of the ladder and of the safety behaviours.</summary>
public enum FearKind
{
    Social,
    Panic,
    Specific,
    Uncertainty,
    Other
}

/// <summary>Whether what the person predicted before a task came true.</summary>
public enum PredictionOutcome
{
    Happened,
    Partly,
    NotHappened
}

/// <param name="Id">Stable id: sessions refer to the step by it, so a step can be renamed or re-rated.</param>
/// <param name="ExpectedDistress">How frightening the person expects it to be, 0..100; the ladder is ordered by it.</param>
public sealed record ExposureStep(string Id, string Text, int ExpectedDistress);

/// <summary>
/// One exposure task: the prediction written before it and the report after. A task without a report is still pending;
/// an interrupted one is a valid outcome, not a failure.
/// </summary>
public sealed record ExposureSession
{
    public required string Id { get; init; }
    public required string StepId { get; init; }
    public DateTime CreatedAt { get; init; }

    /// <summary>What the person expects to happen, in their words.</summary>
    public string Prediction { get; init; } = string.Empty;

    /// <summary>How sure the person is of the prediction before the task, 0..100%.</summary>
    public int BeliefBefore { get; init; }

    /// <summary>The safety behaviours the person chose to drop this time.</summary>
    public IReadOnlyList<string> DroppedSafetyBehaviours { get; init; } = [];

    public DateTime? ReportedAt { get; init; }
    public string WhatHappened { get; init; } = string.Empty;
    public PredictionOutcome? Outcome { get; init; }

    /// <summary>How sure the person is of the same prediction after the task, 0..100%.</summary>
    public int? BeliefAfter { get; init; }

    public int? PeakDistress { get; init; }
    public bool UsedSafetyBehaviour { get; init; }

    /// <summary>What the person learned, one sentence: the new knowledge that competes with the fear.</summary>
    public string Learned { get; init; } = string.Empty;

    public bool Interrupted { get; init; }

    public bool IsReported => Outcome is not null || Interrupted;
}

/// <summary>The person's fear ladder: the formulation, the steps and every task done on them.</summary>
public sealed record ExposurePlan
{
    public FearKind Kind { get; init; } = FearKind.Other;

    /// <summary>The main prediction: what the person believes would happen in the worst case.</summary>
    public string FearedOutcome { get; init; } = string.Empty;

    /// <summary>How sure the person was of the main prediction when the ladder was made, 0..100%.</summary>
    public int FearedOutcomeBelief { get; init; }

    public IReadOnlyList<string> AvoidedSituations { get; init; } = [];
    public IReadOnlyList<string> SafetyBehaviours { get; init; } = [];
    public IReadOnlyList<ExposureStep> Steps { get; init; } = [];
    public IReadOnlyList<ExposureSession> Sessions { get; init; } = [];

    public ExposureScreeningResult Screening { get; init; } = ExposureScreeningResult.NotScreened;
    public DateTime? ScreenedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
