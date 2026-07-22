using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Recommendations;

public sealed record TodayRecommendationContext(
    string Concern,
    TestResultDTO? RecentTestResult = null,
    int? TodayMoodLevel = null,
    IReadOnlyDictionary<string, DateTime>? LastPracticeDatesUtc = null,
    TechniqueId? DraftTechniqueId = null,
    TherapyProgramType? ActiveProgramType = null,
    int ActiveProgramWeek = 0);

public enum TodayRecommendationSource
{
    SessionDraft,
    RecentTest,
    LowMood,
    OnboardingConcern,
    Explore
}

public sealed record TodayRecommendationDecision(
    TechniqueId TechniqueId,
    TodayRecommendationSource Source,
    string? TestId = null);
