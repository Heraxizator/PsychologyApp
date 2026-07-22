using PsychologyApp.Application.ClinicalCare;
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

    TechniqueId ResolveNextAfterCompletion(TodayRecommendationContext context, TechniqueId completedTechniqueId);

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
        TechniqueId.SmallStep,
        TechniqueId.Future,
        TechniqueId.Hack,
        TechniqueId.Observer,
        TechniqueId.Anchor,
        TechniqueId.Comparison
    ];

    private static readonly TechniqueId[] AnxietyPool =
    [
        TechniqueId.Spin,
        TechniqueId.Breathing,
        TechniqueId.Grounding,
        TechniqueId.ThoughtRecord,
        TechniqueId.Observer,
        TechniqueId.Anchor
    ];

    private static readonly TechniqueId[] BodyPool =
    [
        TechniqueId.Experience,
        TechniqueId.Grounding,
        TechniqueId.Breathing,
        TechniqueId.Check,
        TechniqueId.Hack,
        TechniqueId.Observer
    ];

    private static readonly TechniqueId[] MoodPool =
    [
        TechniqueId.SmallStep,
        TechniqueId.ThoughtRecord,
        TechniqueId.SelfCompassion,
        TechniqueId.Paper,
        TechniqueId.Future,
        TechniqueId.Hack,
        TechniqueId.Comparison
    ];

    private static readonly TechniqueId[] LowMoodPool =
    [
        TechniqueId.Breathing,
        TechniqueId.Grounding,
        TechniqueId.SmallStep,
        TechniqueId.SelfCompassion,
        TechniqueId.Observer
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
        if (context.DraftTechniqueId is TechniqueId draftId)
        {
            return new TodayRecommendationDecision(draftId, TodayRecommendationSource.SessionDraft);
        }

        if (context.RecentTestResult is { Score: not null } test &&
            test.CompletedAt >= DateTime.UtcNow.AddDays(-7))
        {
            TechniqueId fromTest = test.TestId == TestIds.LuscherStandard
                ? LuscherScoreRecommendation.RecommendTechnique(test.Score.Value)
                : TestScoreRecommendation.RecommendTechnique(test.TestId, test.Score.Value)
                    ?? TechniqueId.Experience;

            TechniqueId picked = PickFromPool([fromTest], context.LastPracticeDatesUtc);
            return new TodayRecommendationDecision(picked, TodayRecommendationSource.RecentTest, test.TestId);
        }

        if (context.TodayMoodLevel is int mood && mood <= 2)
        {
            TechniqueId picked = PickFromPool(LowMoodPool, context.LastPracticeDatesUtc);
            return new TodayRecommendationDecision(picked, TodayRecommendationSource.LowMood);
        }

        if (context.ActiveProgramType is TherapyProgramType programType && context.ActiveProgramWeek > 0)
        {
            IReadOnlyList<TechniqueId> programPool =
                TherapyProgramCatalog.ResolvePool(programType, context.ActiveProgramWeek);
            TechniqueId fromProgram = PickFromPool(programPool, context.LastPracticeDatesUtc);
            return new TodayRecommendationDecision(fromProgram, TodayRecommendationSource.OnboardingConcern);
        }

        TechniqueId[] pool = ResolveConcernPool(context.Concern);
        TechniqueId fromPool = PickFromPool(pool, context.LastPracticeDatesUtc);
        TodayRecommendationSource source = context.Concern == OnboardingConcernKeys.Explore
            ? TodayRecommendationSource.Explore
            : TodayRecommendationSource.OnboardingConcern;
        return new TodayRecommendationDecision(fromPool, source);
    }

    public TechniqueId ResolveNextAfterCompletion(
        TodayRecommendationContext context,
        TechniqueId completedTechniqueId)
    {
        TechniqueId[] pool;
        if (context.TodayMoodLevel is int mood && mood <= 2)
        {
            pool = LowMoodPool;
        }
        else if (context.ActiveProgramType is TherapyProgramType programType && context.ActiveProgramWeek > 0)
        {
            pool = TherapyProgramCatalog.ResolvePool(programType, context.ActiveProgramWeek).ToArray();
        }
        else
        {
            pool = ResolveConcernPool(context.Concern);
        }

        TechniqueId[] filtered = pool
            .Where(id => id != completedTechniqueId)
            .ToArray();

        if (filtered.Length == 0)
        {
            filtered = ExploreRotation
                .Where(id => id != completedTechniqueId)
                .ToArray();
        }

        return PickFromPool(filtered, context.LastPracticeDatesUtc);
    }

    public IReadOnlyList<TechniqueId> RecommendForSomaticQuery(string query) =>
        SomaticTechniqueRecommendation.RecommendForQuery(query);

    private static TechniqueId[] ResolveConcernPool(string concern) => concern switch
    {
        OnboardingConcernKeys.Anxiety => AnxietyPool,
        OnboardingConcernKeys.Body => BodyPool,
        OnboardingConcernKeys.Mood => MoodPool,
        _ => ExploreRotation
    };

    /// <summary>
    /// Prefers never/least-recently practiced; avoids yesterday's or today's pick when an alternative exists.
    /// </summary>
    public static TechniqueId PickFromPool(
        IReadOnlyList<TechniqueId> pool,
        IReadOnlyDictionary<string, DateTime>? lastPracticeDatesUtc)
    {
        if (pool.Count == 0)
        {
            return TechniqueId.Spin;
        }

        if (pool.Count == 1)
        {
            return pool[0];
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly yesterday = today.AddDays(-1);

        List<(TechniqueId Id, int Index)> ranked = pool
            .Select((id, index) => (Id: id, Index: index))
            .OrderBy(item => GetLastLocalDate(item.Id, lastPracticeDatesUtc) ?? DateOnly.MinValue)
            .ThenBy(item => item.Index)
            .ToList();

        TechniqueId best = ranked[0].Id;
        DateOnly? bestDate = GetLastLocalDate(best, lastPracticeDatesUtc);
        if (bestDate is null || (bestDate != yesterday && bestDate != today))
        {
            return best;
        }

        foreach ((TechniqueId candidate, _) in ranked.Skip(1))
        {
            DateOnly? date = GetLastLocalDate(candidate, lastPracticeDatesUtc);
            if (date is null || (date != yesterday && date != today))
            {
                return candidate;
            }
        }

        return ranked[1].Id;
    }

    private static DateOnly? GetLastLocalDate(
        TechniqueId techniqueId,
        IReadOnlyDictionary<string, DateTime>? lastPracticeDatesUtc)
    {
        if (lastPracticeDatesUtc is null
            || !lastPracticeDatesUtc.TryGetValue(techniqueId.ToString(), out DateTime utc))
        {
            return null;
        }

        return DateOnly.FromDateTime(utc.ToLocalTime());
    }
}
