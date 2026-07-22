using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IClinicalCareRepository
{
    Task SaveRiskAssessmentAsync(RiskAssessmentDTO assessment, CancellationToken cancellationToken = default);
    Task<RiskAssessmentDTO?> GetLatestRiskAssessmentAsync(CancellationToken cancellationToken = default);

    Task UpsertActiveProgramAsync(TherapyProgramStateDTO program, CancellationToken cancellationToken = default);
    Task<TherapyProgramStateDTO?> GetActiveProgramAsync(CancellationToken cancellationToken = default);

    Task SaveEscalationEventAsync(EscalationEventDTO escalation, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EscalationEventDTO>> GetRecentEscalationsAsync(int limit, CancellationToken cancellationToken = default);
}
