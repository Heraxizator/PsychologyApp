namespace PsychologyApp.Application.Models;

public enum RiskLevel
{
    Green = 0,
    Amber = 1,
    Red = 2
}

public enum TherapyProgramType
{
    Anxiety = 0,
    Mood = 1,
    Stress = 2
}

public sealed class RiskAssessmentInput
{
    public bool HasSelfHarmThoughts { get; init; }
    public bool HasSevereDisorientation { get; init; }
    public bool HasSubstanceRisk { get; init; }
    public bool HasSevereInsomnia { get; init; }
    public string Source { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class RiskAssessmentDTO
{
    public long RiskAssessmentId { get; init; }
    public DateTime AssessedAt { get; init; }
    public string Source { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public bool HasSelfHarmThoughts { get; init; }
    public bool HasSevereDisorientation { get; init; }
    public bool HasSubstanceRisk { get; init; }
    public bool HasSevereInsomnia { get; init; }
    public RiskLevel RiskLevel { get; init; }
}

public sealed class TherapyProgramStateDTO
{
    public TherapyProgramType ProgramType { get; init; }
    public DateTime StartedAt { get; init; }
    public int CurrentWeek { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class EscalationEventDTO
{
    public long EscalationEventId { get; init; }
    public DateTime CreatedAt { get; init; }
    public RiskLevel RiskLevel { get; init; }
    public string TriggerSource { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}

public sealed class ClinicalScorecardDTO
{
    public DateOnly WeekStart { get; init; }
    public DateOnly WeekEnd { get; init; }
    public int PracticeCount { get; init; }
    public int MoodEntriesCount { get; init; }
    public int TestCount { get; init; }
    public double AverageMoodLevel { get; init; }
    public RiskLevel RiskLevel { get; init; }
    public string Summary { get; init; } = string.Empty;
}
