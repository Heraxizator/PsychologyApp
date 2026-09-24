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
    int ActiveProgramWeek = 0,
    /// <summary>Average SUDS drop (pre minus post) per technique key, for techniques with enough sessions to trust the number.</summary>
    IReadOnlyDictionary<string, double>? TechniqueEffectiveness = null);

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
