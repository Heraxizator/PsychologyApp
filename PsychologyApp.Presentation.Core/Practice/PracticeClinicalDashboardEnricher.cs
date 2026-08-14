using Microsoft.Extensions.Logging;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.ClinicalCare;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed record PracticeClinicalDashboardSnapshot(
    string TherapyProgramBanner,
    string ClinicalRiskBanner);

public sealed class PracticeClinicalDashboardEnricher(
    IClinicalCareService clinicalCareService,
    ILogger<PracticeClinicalDashboardEnricher> logger)
{
    public async Task<PracticeClinicalDashboardSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await clinicalCareService.AdjustProgramFromScorecardAsync(cancellationToken);
            TherapyProgramStateDTO? program = await clinicalCareService.GetActiveProgramAsync(cancellationToken);
            RiskAssessmentDTO? latestRisk = await clinicalCareService.GetLatestRiskAssessmentAsync(cancellationToken);
            TherapyProgramAdherence? adherence = await clinicalCareService.GetActiveWeekAdherenceAsync(cancellationToken);

            string therapyBanner = adherence is not null
                ? FormatProgramBanner(
                    adherence.Program,
                    adherence.CompletedDistinctTechniques,
                    adherence.WeekPlan.TechniquePool.Count)
                : program is { IsActive: true }
                    ? FormatProgramBanner(program)
                    : string.Empty;

            string riskBanner = latestRisk is null
                ? string.Empty
                : FormatRiskBanner(latestRisk.RiskLevel);

            return new PracticeClinicalDashboardSnapshot(therapyBanner, riskBanner);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Clinical care dashboard enrichment failed.");
            return new PracticeClinicalDashboardSnapshot(
                string.Empty,
                AppStrings.ClinicalStatusUnavailableBanner);
        }
    }

    public static string FormatProgramBanner(
        TherapyProgramStateDTO program,
        int completedInWeek = 0,
        int targetPractices = 0)
    {
        string name = program.ProgramType switch
        {
            TherapyProgramType.Anxiety => AppStrings.TherapyProgramAnxiety,
            TherapyProgramType.Mood => AppStrings.TherapyProgramMood,
            _ => AppStrings.TherapyProgramStress
        };
        string goal = AppStrings.TherapyProgramWeekGoal(program.CurrentWeek);
        return targetPractices > 0
            ? AppStrings.TherapyProgramBanner(name, program.CurrentWeek, goal, completedInWeek, targetPractices)
            : AppStrings.TherapyProgramBanner(name, program.CurrentWeek, goal);
    }

    public static string FormatRiskBanner(RiskLevel level) => level switch
    {
        RiskLevel.Red => AppStrings.ClinicalRedBanner,
        RiskLevel.Amber => AppStrings.ClinicalAmberBanner,
        _ => string.Empty
    };
}
