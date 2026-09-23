namespace PsychologyApp.Application.ClinicalCare;

public interface ISpecialistSummaryService
{
    /// <summary>Plain-text summary of the last 30 days, meant to be shared with a psychologist or doctor.</summary>
    Task<string> BuildSummaryAsync(bool english, CancellationToken cancellationToken = default);
}
