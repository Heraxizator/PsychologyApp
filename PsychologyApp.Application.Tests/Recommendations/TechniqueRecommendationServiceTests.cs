using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Recommendations;

public sealed class TechniqueRecommendationServiceTests
{
    private readonly TechniqueRecommendationService _service = new();

    [Theory]
    [InlineData(OnboardingConcernKeys.Anxiety, TechniqueId.Spin)]
    [InlineData(OnboardingConcernKeys.Body, TechniqueId.Experience)]
    [InlineData(OnboardingConcernKeys.Mood, TechniqueId.SmallStep)]
    public void ResolveFromOnboardingConcern_MapsKnownConcerns(string concern, TechniqueId expected) =>
        Assert.Equal(expected, _service.ResolveFromOnboardingConcern(concern));

    [Fact]
    public void RecommendForSomaticQuery_ReturnsHeadacheTechniques()
    {
        IReadOnlyList<TechniqueId> techniques = _service.RecommendForSomaticQuery("headache");

        Assert.Contains(TechniqueId.Spin, techniques);
        Assert.Contains(TechniqueId.Paper, techniques);
    }

    [Fact]
    public void ResolveTodayTechnique_UsesRecentTestWithinSevenDays()
    {
        TodayRecommendationDecision decision = _service.ResolveTodayTechnique(new TodayRecommendationContext(
            OnboardingConcernKeys.Explore,
            RecentTestResult: new PsychologyApp.Application.Models.TestResultDTO
            {
                TestId = "beck",
                Score = 12,
                CompletedAt = DateTime.UtcNow.AddDays(-2)
            }));

        Assert.Equal(TechniqueId.Spin, decision.TechniqueId);
        Assert.Equal(TodayRecommendationSource.RecentTest, decision.Source);
    }

    [Fact]
    public void ResolveTodayTechnique_UsesLuscherCoWhenRecentStandardTest()
    {
        TodayRecommendationDecision decision = _service.ResolveTodayTechnique(new TodayRecommendationContext(
            OnboardingConcernKeys.Explore,
            RecentTestResult: new TestResultDTO
            {
                TestId = TestIds.LuscherStandard,
                Score = 25,
                CompletedAt = DateTime.UtcNow.AddDays(-1)
            }));

        Assert.Equal(TechniqueId.Spin, decision.TechniqueId);
        Assert.Equal(TodayRecommendationSource.RecentTest, decision.Source);
        Assert.Equal(TestIds.LuscherStandard, decision.TestId);
    }

    [Fact]
    public void ResolveTodayTechnique_UsesLowMoodWhenNoRecentTest()
    {
        TodayRecommendationDecision decision = _service.ResolveTodayTechnique(new TodayRecommendationContext(
            OnboardingConcernKeys.Body,
            TodayMoodLevel: 2));

        Assert.Equal(TechniqueId.Breathing, decision.TechniqueId);
        Assert.Equal(TodayRecommendationSource.LowMood, decision.Source);
    }

    [Fact]
    public void ResolveTodayTechnique_FallsBackToConcern()
    {
        TodayRecommendationDecision decision = _service.ResolveTodayTechnique(new TodayRecommendationContext(
            OnboardingConcernKeys.Anxiety));

        Assert.Equal(TechniqueId.Spin, decision.TechniqueId);
        Assert.Equal(TodayRecommendationSource.OnboardingConcern, decision.Source);
    }

    [Fact]
    public void ResolveTodayTechnique_PrefersDraftOverTestAndMood()
    {
        TodayRecommendationDecision decision = _service.ResolveTodayTechnique(new TodayRecommendationContext(
            OnboardingConcernKeys.Anxiety,
            RecentTestResult: new TestResultDTO
            {
                TestId = "beck",
                Score = 12,
                CompletedAt = DateTime.UtcNow
            },
            TodayMoodLevel: 1,
            DraftTechniqueId: TechniqueId.Paper));

        Assert.Equal(TechniqueId.Paper, decision.TechniqueId);
        Assert.Equal(TodayRecommendationSource.SessionDraft, decision.Source);
    }

    [Fact]
    public void ResolveTodayTechnique_RotatesAwayFromYesterdayConcernPick()
    {
        Dictionary<string, DateTime> dates = new(StringComparer.Ordinal)
        {
            [TechniqueId.Spin.ToString()] = DateTime.Today.AddDays(-1).ToUniversalTime()
        };

        TodayRecommendationDecision decision = _service.ResolveTodayTechnique(new TodayRecommendationContext(
            OnboardingConcernKeys.Anxiety,
            LastPracticeDatesUtc: dates));

        Assert.NotEqual(TechniqueId.Spin, decision.TechniqueId);
        Assert.Equal(TodayRecommendationSource.OnboardingConcern, decision.Source);
    }

    [Fact]
    public void PickFromPool_PrefersNeverPracticed()
    {
        Dictionary<string, DateTime> dates = new(StringComparer.Ordinal)
        {
            [TechniqueId.Breathing.ToString()] = DateTime.UtcNow.AddDays(-10),
            [TechniqueId.Grounding.ToString()] = DateTime.UtcNow.AddDays(-3)
        };

        TechniqueId picked = TechniqueRecommendationService.PickFromPool(
            [TechniqueId.Breathing, TechniqueId.Grounding, TechniqueId.SmallStep],
            dates);

        Assert.Equal(TechniqueId.SmallStep, picked);
    }

    [Fact]
    public void ResolveNextAfterCompletion_ExcludesCompletedTechnique()
    {
        TodayRecommendationContext context = new(OnboardingConcernKeys.Anxiety);

        TechniqueId next = _service.ResolveNextAfterCompletion(context, TechniqueId.Spin);

        Assert.NotEqual(TechniqueId.Spin, next);
        Assert.Contains(next, new[] { TechniqueId.Breathing, TechniqueId.Grounding, TechniqueId.ThoughtRecord });
    }

    [Fact]
    public void ResolveNextAfterCompletion_UsesLowMoodPoolWhenMoodLow()
    {
        TodayRecommendationContext context = new(
            OnboardingConcernKeys.Anxiety,
            TodayMoodLevel: 1);

        TechniqueId next = _service.ResolveNextAfterCompletion(context, TechniqueId.Breathing);

        Assert.NotEqual(TechniqueId.Breathing, next);
        Assert.Contains(next, new[] { TechniqueId.Grounding, TechniqueId.SmallStep });
    }

    [Fact]
    public void ResolveNextAfterCompletion_FallsBackToExploreWhenConcernPoolFullyCompleted()
    {
        Dictionary<string, DateTime> dates = new(StringComparer.Ordinal)
        {
            [TechniqueId.Breathing.ToString()] = DateTime.UtcNow.AddDays(-5),
            [TechniqueId.Grounding.ToString()] = DateTime.UtcNow.AddDays(-4),
            [TechniqueId.ThoughtRecord.ToString()] = DateTime.UtcNow.AddDays(-3)
        };
        TodayRecommendationContext context = new(
            OnboardingConcernKeys.Anxiety,
            LastPracticeDatesUtc: dates);

        TechniqueId next = _service.ResolveNextAfterCompletion(context, TechniqueId.Spin);

        Assert.NotEqual(TechniqueId.Spin, next);
        Assert.Contains(next, ExploreRotationExcept(TechniqueId.Spin));
    }

    private static IEnumerable<TechniqueId> ExploreRotationExcept(TechniqueId exclude) =>
        new[]
        {
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
        }.Where(id => id != exclude);
}
