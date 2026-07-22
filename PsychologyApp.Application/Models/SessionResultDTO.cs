namespace PsychologyApp.Application.Models;

public sealed class SessionResultDTO
{
    public long SessionResultId { get; set; }

    public string ItemKey { get; set; } = string.Empty;

    public DateTime CompletedAt { get; set; }

    public int DurationSeconds { get; set; }

    public string? PayloadJson { get; set; }

    public int? PreIntensity { get; set; }

    public int? PostIntensity { get; set; }

    public string? ProgramType { get; set; }

    public int? ProgramWeek { get; set; }
}
