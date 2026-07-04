using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Somatic;
using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Recommendations;

public interface ITechniqueRecommendationService
{
    TechniqueId ResolveFromOnboardingConcern(string concern);

    TodayRecommendationDecision ResolveTodayTechnique(TodayRecommendationContext context);

    IReadOnlyList<TechniqueId> RecommendForSomaticQuery(string query);
}

public sealed class TechniqueRecommendationService : ITechniqueRecommendationService
{
    private static readonly TechniqueId[] ExploreRotation =
    [
        TechniqueId.Spin,
        TechniqueId.Paper,
        TechniqueId.Experience,
        TechniqueId.Breathing,
        TechniqueId.Grounding,
        TechniqueId.SmallStep
    ];

    public TechniqueId ResolveFromOnboardingConcern(string concern) => concern switch
    {
        OnboardingConcernKeys.Anxiety => TechniqueId.Spin,
        OnboardingConcernKeys.Body => TechniqueId.Experience,
        OnboardingConcernKeys.Mood => TechniqueId.SmallStep,
        OnboardingConcernKeys.Explore => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length],
        _ => ExploreRotation[DateTime.UtcNow.DayOfYear % ExploreRotation.Length]
    };

    public TodayRecommendationDecision ResolveTodayTechnique(TodayRecommendationContext context)
    {
        if (context.RecentTestResult is { Score: not null } test &&
            test.CompletedAt >= DateTime.UtcNow.AddDays(-7))
        {
            if (test.TestId == TestIds.LuscherStandard)
            {
                TechniqueId fromLuscher = LuscherScoreRecommendation.RecommendTechnique(test.Score.Value);
                return new TodayRecommendationDecision(fromLuscher, TodayRecommendationSource.RecentTest, test.TestId);
            }

            TechniqueId? fromTest = TestScoreRecommendation.RecommendTechnique(test.TestId, test.Score.Value);
            if (fromTest is TechniqueId techniqueId)
            {
                return new TodayRecommendationDecision(techniqueId, TodayRecommendationSource.RecentTest, test.TestId);
            }
        }

        if (context.TodayMoodLevel is int mood && mood <= 2)
        {
            return new TodayRecommendationDecision(TechniqueId.Breathing, TodayRecommendationSource.LowMood);
        }

        TechniqueId fromConcern = ResolveFromOnboardingConcern(context.Concern);
        TodayRecommendationSource source = context.Concern == OnboardingConcernKeys.Explore
            ? TodayRecommendationSource.Explore
            : TodayRecommendationSource.OnboardingConcern;
        return new TodayRecommendationDecision(fromConcern, source);
    }

    public IReadOnlyList<TechniqueId> RecommendForSomaticQuery(string query) =>
        SomaticTechniqueRecommendation.RecommendForQuery(query);
}
