using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.ClinicalCare;

public interface IClinicalCareService
{
    Task<RiskAssessmentDTO> AssessRiskAsync(RiskAssessmentInput input, CancellationToken cancellationToken = default);
    Task<RiskAssessmentDTO?> GetLatestRiskAssessmentAsync(CancellationToken cancellationToken = default);
    Task<bool> IsRiskCheckDueAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
    Task<bool> ShouldRouteToCrisisHubAsync(CancellationToken cancellationToken = default);

    Task<TherapyProgramStateDTO> EnsureProgramAsync(string onboardingConcern, CancellationToken cancellationToken = default);
    Task<TherapyProgramStateDTO?> GetActiveProgramAsync(CancellationToken cancellationToken = default);
    Task<TherapyProgramStateDTO?> AdvanceProgramWeekIfDueAsync(CancellationToken cancellationToken = default);
    Task<TherapyProgramWeekPlan?> GetActiveWeekPlanAsync(CancellationToken cancellationToken = default);
    Task<TherapyProgramAdherence?> GetActiveWeekAdherenceAsync(CancellationToken cancellationToken = default);

    Task<ClinicalScorecardDTO> BuildWeeklyScorecardAsync(CancellationToken cancellationToken = default);
    Task<TherapyProgramStateDTO?> AdjustProgramFromScorecardAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EscalationEventDTO>> GetRecentEscalationsAsync(int limit = 20, CancellationToken cancellationToken = default);
}
