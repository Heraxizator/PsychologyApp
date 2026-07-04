using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Recommendations;

public sealed record TodayRecommendationContext(
    string Concern,
    TestResultDTO? RecentTestResult = null,
    int? TodayMoodLevel = null);

public enum TodayRecommendationSource
{
    RecentTest,
    LowMood,
    OnboardingConcern,
    Explore
}

public sealed record TodayRecommendationDecision(
    TechniqueId TechniqueId,
    TodayRecommendationSource Source,
    string? TestId = null);
