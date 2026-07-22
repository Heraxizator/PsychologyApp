using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.ClinicalCare;

public sealed record TherapyProgramWeekPlan(
    TherapyProgramType ProgramType,
    int Week,
    IReadOnlyList<TechniqueId> TechniquePool,
    string GoalKey);

public static class TherapyProgramCatalog
{
    public const int TotalWeeks = 4;

    public static TherapyProgramWeekPlan GetWeekPlan(TherapyProgramType programType, int week)
    {
        int normalizedWeek = Math.Clamp(week, 1, TotalWeeks);
        return new TherapyProgramWeekPlan(
            programType,
            normalizedWeek,
            ResolvePool(programType, normalizedWeek),
            $"week_{normalizedWeek}");
    }

    public static IReadOnlyList<TechniqueId> ResolvePool(TherapyProgramType programType, int week) =>
        (programType, Math.Clamp(week, 1, TotalWeeks)) switch
        {
            (TherapyProgramType.Anxiety, 1) => [TechniqueId.Spin, TechniqueId.Breathing, TechniqueId.Grounding],
            (TherapyProgramType.Anxiety, 2) => [TechniqueId.ThoughtRecord, TechniqueId.Observer, TechniqueId.Anchor],
            (TherapyProgramType.Anxiety, 3) => [TechniqueId.Breathing, TechniqueId.ThoughtRecord, TechniqueId.Hack],
            (TherapyProgramType.Anxiety, _) => [TechniqueId.Observer, TechniqueId.Anchor, TechniqueId.Grounding],

            (TherapyProgramType.Mood, 1) => [TechniqueId.SmallStep, TechniqueId.SelfCompassion, TechniqueId.Breathing],
            (TherapyProgramType.Mood, 2) => [TechniqueId.ThoughtRecord, TechniqueId.Paper, TechniqueId.Future],
            (TherapyProgramType.Mood, 3) => [TechniqueId.Hack, TechniqueId.Comparison, TechniqueId.SmallStep],
            (TherapyProgramType.Mood, _) => [TechniqueId.SelfCompassion, TechniqueId.Observer, TechniqueId.Future],

            (TherapyProgramType.Stress, 1) => [TechniqueId.Experience, TechniqueId.Breathing, TechniqueId.Grounding],
            (TherapyProgramType.Stress, 2) => [TechniqueId.Check, TechniqueId.Hack, TechniqueId.Observer],
            (TherapyProgramType.Stress, 3) => [TechniqueId.Paper, TechniqueId.Anchor, TechniqueId.Breathing],
            _ => [TechniqueId.Grounding, TechniqueId.Experience, TechniqueId.SmallStep]
        };
}
