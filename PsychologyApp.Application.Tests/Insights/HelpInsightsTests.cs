using PsychologyApp.Application.Insights;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Insights;

public sealed class HelpInsightsTests
{
    private static readonly DateTime Now = new(2026, 10, 18, 12, 0, 0, DateTimeKind.Utc);

    private static SessionResultDTO Session(string key, int? before, int? after) =>
        new() { ItemKey = key, PreIntensity = before, PostIntensity = after, CompletedAt = Now };

    private static TestResultDTO Test(string id, int score, int daysAgo) =>
        new() { TestId = id, Score = score, CompletedAt = Now.AddDays(-daysAgo) };

    [Fact]
    public void Practices_that_lowered_the_tension_more_than_once_come_first()
    {
        IReadOnlyList<PracticeEffect> effects = HelpInsights.PracticeEffects(
        [
            Session("Grounding", 7, 3), Session("Grounding", 6, 4),
            Session("Breathing", 5, 4), Session("Breathing", 6, 4),
            Session("Paper", 8, 1),
            Session("Observer", 5, 6), Session("Observer", 4, 5),
            Session("Anchor", 6, null), Session("Anchor", 6, 2)
        ]);

        Assert.Equal(["Grounding", "Breathing"], effects.Select(effect => effect.ItemKey));
        Assert.Equal(3, effects[0].AverageDrop);
        Assert.Equal(2, effects[0].Sessions);
    }

    [Fact]
    public void A_test_compares_the_latest_result_with_the_first_by_its_own_direction()
    {
        Func<string, ScoreDirection> direction = id => id switch
        {
            "rosenberg" => ScoreDirection.HigherIsBetter,
            "luscher" => ScoreDirection.None,
            _ => ScoreDirection.LowerIsBetter
        };

        IReadOnlyList<TestProgress> progress = HelpInsights.TestProgress(
        [
            Test("phq9", 14, 40), Test("phq9", 11, 20), Test("phq9", 9, 3),
            Test("rosenberg", 22, 30), Test("rosenberg", 18, 15),
            Test("luscher", 3, 2),
            Test("gad7", 8, 1)
        ], direction, Now);

        Assert.Equal(["gad7", "phq9", "rosenberg"], progress.Select(test => test.TestId));
        TestProgress phq = progress.Single(test => test.TestId == "phq9");
        Assert.Equal((14, 9, 3, TestTrendKind.Improved, false), (phq.First, phq.Latest, phq.Results, phq.Trend, phq.RetestDue));
        TestProgress rosenberg = progress.Single(test => test.TestId == "rosenberg");
        Assert.Equal((TestTrendKind.Worse, true), (rosenberg.Trend, rosenberg.RetestDue));
        Assert.Equal(TestTrendKind.None, progress.Single(test => test.TestId == "gad7").Trend);
    }

    [Fact]
    public void A_factor_counts_only_with_enough_entries_on_both_sides_and_a_real_difference()
    {
        MoodWithFactors Entry(int mood, params string[] factors) => new(mood, factors);
        MoodWithFactors[] entries =
        [
            Entry(5, "walk"), Entry(4, "walk", "people"), Entry(4, "walk"),
            Entry(2, "work"), Entry(2, "work"), Entry(3, "work", "people"),
            Entry(3), Entry(3),
            Entry(4, "sleep"), Entry(3, "sleep")
        ];

        IReadOnlyList<FactorEffect> effects = HelpInsights.FactorEffects(entries, ["walk", "work", "people", "sleep"]);

        Assert.Equal(["walk", "work"], effects.Select(effect => effect.Factor));
        Assert.True(effects[0].Difference > 0);
        Assert.True(effects[1].Difference < 0);
        Assert.Equal(3, effects[0].Entries);
    }
}
