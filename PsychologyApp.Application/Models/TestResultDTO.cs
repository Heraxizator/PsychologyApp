namespace PsychologyApp.Application.Models;

public sealed class TestResultDTO
{
    public long TestResultId { get; set; }
    public string TestId { get; set; } = string.Empty;
    public int? Score { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string? DetailJson { get; set; }
    public DateTime CompletedAt { get; set; }
}
