namespace PsychologyApp.Application.Models;

public sealed class CompletionDTO
{
    public long CompletionId { get; set; }
    public string CompletionKind { get; set; } = string.Empty;
    public string ItemKey { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string PageName { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public int DurationSeconds { get; set; }
}
