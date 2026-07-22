using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.ClinicalCare;

public sealed class ClinicalCareService(
    IClinicalCareRepository repository,
    IUserProgressService userProgressService) : IClinicalCareService
{
    public static readonly TimeSpan DefaultRiskCheckInterval = TimeSpan.FromDays(7);

    public async Task<RiskAssessmentDTO> AssessRiskAsync(
        RiskAssessmentInput input,
        CancellationToken cancellationToken = default)
    {
        RiskLevel riskLevel = ClassifyRisk(input);
        RiskAssessmentDTO assessment = new()
        {
            AssessedAt = DateTime.UtcNow,
            Source = string.IsNullOrWhiteSpace(input.Source) ? "unknown" : input.Source,
            Notes = input.Notes ?? string.Empty,
            HasSelfHarmThoughts = input.HasSelfHarmThoughts,
            HasSevereDisorientation = input.HasSevereDisorientation,
            HasSubstanceRisk = input.HasSubstanceRisk,
            HasSevereInsomnia = input.HasSevereInsomnia,
            RiskLevel = riskLevel
        };

        await repository.SaveRiskAssessmentAsync(assessment, cancellationToken);

        if (riskLevel is RiskLevel.Red)
        {
            await repository.SaveEscalationEventAsync(
                new EscalationEventDTO
                {
                    CreatedAt = DateTime.UtcNow,
                    RiskLevel = riskLevel,
                    TriggerSource = assessment.Source,
                    Action = EscalationActions.RouteToCrisisHub,
                    Notes = assessment.Notes
                },
                cancellationToken);
        }
        else if (riskLevel is RiskLevel.Amber)
        {
            await repository.SaveEscalationEventAsync(
                new EscalationEventDTO
                {
                    CreatedAt = DateTime.UtcNow,
                    RiskLevel = riskLevel,
                    TriggerSource = assessment.Source,
                    Action = EscalationActions.OfferSpecialistHelp,
                    Notes = assessment.Notes
                },
                cancellationToken);
        }

        return assessment;
    }

    public Task<RiskAssessmentDTO?> GetLatestRiskAssessmentAsync(CancellationToken cancellationToken = default) =>
        repository.GetLatestRiskAssessmentAsync(cancellationToken);

    public async Task<bool> IsRiskCheckDueAsync(TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
        RiskAssessmentDTO? latest = await repository.GetLatestRiskAssessmentAsync(cancellationToken);
        if (latest is null)
        {
            return true;
        }

        return DateTime.UtcNow - latest.AssessedAt > maxAge;
    }

    public async Task<bool> ShouldRouteToCrisisHubAsync(CancellationToken cancellationToken = default)
    {
        RiskAssessmentDTO? latest = await repository.GetLatestRiskAssessmentAsync(cancellationToken);
        return latest?.RiskLevel is RiskLevel.Red;
    }

    public async Task<TherapyProgramStateDTO> EnsureProgramAsync(
        string onboardingConcern,
        CancellationToken cancellationToken = default)
    {
        TherapyProgramStateDTO? existing = await repository.GetActiveProgramAsync(cancellationToken);
        if (existing is not null)
        {
            return await AdvanceProgramWeekIfDueAsync(cancellationToken) ?? existing;
        }

        TherapyProgramStateDTO program = new()
        {
            ProgramType = ResolveProgram(onboardingConcern),
            StartedAt = DateTime.UtcNow,
            CurrentWeek = 1,
            IsActive = true
        };
        await repository.UpsertActiveProgramAsync(program, cancellationToken);
        return program;
    }

    public Task<TherapyProgramStateDTO?> GetActiveProgramAsync(CancellationToken cancellationToken = default) =>
        repository.GetActiveProgramAsync(cancellationToken);

    public async Task<TherapyProgramStateDTO?> AdvanceProgramWeekIfDueAsync(CancellationToken cancellationToken = default)
    {
        TherapyProgramStateDTO? existing = await repository.GetActiveProgramAsync(cancellationToken);
        if (existing is null)
        {
            return null;
        }

        int elapsedWeeks = Math.Max(0, (DateTime.UtcNow.Date - existing.StartedAt.ToUniversalTime().Date).Days / 7) + 1;
        int targetWeek = Math.Clamp(elapsedWeeks, 1, TherapyProgramCatalog.TotalWeeks);
        if (targetWeek == existing.CurrentWeek)
        {
            return existing;
        }

        TherapyProgramStateDTO advanced = new()
        {
            ProgramType = existing.ProgramType,
            StartedAt = existing.StartedAt,
            CurrentWeek = targetWeek,
            IsActive = existing.IsActive
        };
        await repository.UpsertActiveProgramAsync(advanced, cancellationToken);
        return advanced;
    }

    public async Task<TherapyProgramWeekPlan?> GetActiveWeekPlanAsync(CancellationToken cancellationToken = default)
    {
        TherapyProgramStateDTO? program = await AdvanceProgramWeekIfDueAsync(cancellationToken);
        if (program is null || !program.IsActive)
        {
            return null;
        }

        return TherapyProgramCatalog.GetWeekPlan(program.ProgramType, program.CurrentWeek);
    }

    public async Task<ClinicalScorecardDTO> BuildWeeklyScorecardAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly weekStart = today.AddDays(-6);

        IReadOnlyList<CompletionDTO> completions =
            await userProgressService.GetRecentTechniqueCompletionsAsync(50, cancellationToken);
        int practiceCount = completions.Count(entry =>
            DateOnly.FromDateTime(entry.CompletedAt.ToLocalTime()) >= weekStart);

        IReadOnlyList<MoodEntryDTO> moods = await userProgressService.GetRecentMoodsAsync(21, cancellationToken);
        IReadOnlyList<MoodEntryDTO> weekMoods = moods
            .Where(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()) >= weekStart)
            .ToList();
        double avgMood = weekMoods.Count == 0 ? 0 : weekMoods.Average(entry => entry.MoodLevel);

        long testCountTotal = await userProgressService.CountTestResultsAsync(cancellationToken);
        int testCount = (int)Math.Min(int.MaxValue, testCountTotal);

        RiskAssessmentDTO? latestRisk = await repository.GetLatestRiskAssessmentAsync(cancellationToken);
        RiskLevel riskLevel = latestRisk?.RiskLevel ?? DeriveRiskFromSignals(avgMood, practiceCount);

        return new ClinicalScorecardDTO
        {
            WeekStart = weekStart,
            WeekEnd = today,
            PracticeCount = practiceCount,
            MoodEntriesCount = weekMoods.Count,
            TestCount = testCount,
            AverageMoodLevel = avgMood,
            RiskLevel = riskLevel,
            Summary = BuildSummary(practiceCount, weekMoods.Count, avgMood, riskLevel)
        };
    }

    public async Task<TherapyProgramStateDTO?> AdjustProgramFromScorecardAsync(CancellationToken cancellationToken = default)
    {
        ClinicalScorecardDTO scorecard = await BuildWeeklyScorecardAsync(cancellationToken);
        TherapyProgramStateDTO? program = await repository.GetActiveProgramAsync(cancellationToken);
        if (program is null)
        {
            return null;
        }

        if (scorecard.RiskLevel is RiskLevel.Red)
        {
            await repository.SaveEscalationEventAsync(
                new EscalationEventDTO
                {
                    CreatedAt = DateTime.UtcNow,
                    RiskLevel = RiskLevel.Red,
                    TriggerSource = "weekly_scorecard",
                    Action = EscalationActions.RouteToCrisisHub,
                    Notes = scorecard.Summary
                },
                cancellationToken);
            return program;
        }

        if (scorecard.RiskLevel is RiskLevel.Amber)
        {
            await repository.SaveEscalationEventAsync(
                new EscalationEventDTO
                {
                    CreatedAt = DateTime.UtcNow,
                    RiskLevel = RiskLevel.Amber,
                    TriggerSource = "weekly_scorecard",
                    Action = EscalationActions.OfferSpecialistHelp,
                    Notes = scorecard.Summary
                },
                cancellationToken);

            // Hold at current week (gentler pace) when signals worsen.
            TherapyProgramStateDTO held = new()
            {
                ProgramType = program.ProgramType,
                StartedAt = program.StartedAt,
                CurrentWeek = Math.Max(1, program.CurrentWeek),
                IsActive = true
            };
            await repository.UpsertActiveProgramAsync(held, cancellationToken);
            return held;
        }

        return await AdvanceProgramWeekIfDueAsync(cancellationToken);
    }

    public Task<IReadOnlyList<EscalationEventDTO>> GetRecentEscalationsAsync(
        int limit = 20,
        CancellationToken cancellationToken = default) =>
        repository.GetRecentEscalationsAsync(limit, cancellationToken);

    private static TherapyProgramType ResolveProgram(string concern) =>
        concern switch
        {
            OnboardingConcernKeys.Anxiety => TherapyProgramType.Anxiety,
            OnboardingConcernKeys.Mood => TherapyProgramType.Mood,
            _ => TherapyProgramType.Stress
        };

    public static RiskLevel ClassifyRisk(RiskAssessmentInput input)
    {
        if (input.HasSelfHarmThoughts || input.HasSevereDisorientation || input.HasSubstanceRisk)
        {
            return RiskLevel.Red;
        }

        if (input.HasSevereInsomnia)
        {
            return RiskLevel.Amber;
        }

        return RiskLevel.Green;
    }

    private static RiskLevel DeriveRiskFromSignals(double avgMood, int practiceCount)
    {
        if (avgMood > 0 && avgMood <= 2.0)
        {
            return RiskLevel.Amber;
        }

        if (avgMood > 0 && avgMood <= 2.6 && practiceCount <= 1)
        {
            return RiskLevel.Amber;
        }

        return RiskLevel.Green;
    }

    private static string BuildSummary(int practiceCount, int moodCount, double avgMood, RiskLevel riskLevel)
    {
        string moodText = moodCount == 0 ? "no mood check-ins" : $"avg mood {avgMood:F1}";
        return $"Week: {practiceCount} practices, {moodText}, risk {riskLevel}.";
    }
}

public static class EscalationActions
{
    public const string RouteToCrisisHub = "route_to_crisis_hub";
    public const string OfferSpecialistHelp = "offer_specialist_help";
}
