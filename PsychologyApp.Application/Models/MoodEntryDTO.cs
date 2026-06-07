namespace PsychologyApp.Application.Models;

public sealed class MoodEntryDTO
{
    public long MoodEntryId { get; set; }
    public int MoodLevel { get; set; }
    public string? Note { get; set; }
    public DateTime RecordedAt { get; set; }
}
