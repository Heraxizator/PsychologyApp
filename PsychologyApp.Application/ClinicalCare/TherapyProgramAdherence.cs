using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.ClinicalCare;

public sealed record TherapyProgramAdherence(
    TherapyProgramStateDTO Program,
    TherapyProgramWeekPlan WeekPlan,
    int CompletedDistinctTechniques);
