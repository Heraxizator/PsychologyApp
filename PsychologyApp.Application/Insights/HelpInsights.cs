using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Application.Insights;

/// <param name="AverageDrop">How much the tension fell on average, before minus after, on the 0..10 scale.</param>
public sealed record PracticeEffect(string ItemKey, int Sessions, double AverageDrop);

/// <param name="RetestDue">The last result is old enough for a new one to show a change.</param>
public sealed record TestProgress(string TestId, int First, int Latest, int Results, DateTime LatestUtc, TestTrendKind Trend, bool RetestDue);

/// <param name="Difference">Average mood on entries with the factor minus without it, on the 1..5 mood scale; negative lowers it.</param>
public sealed record FactorEffect(string Factor, int Entries, double Difference);

/// <param name="Factors">The journal factors marked on that entry (sleep, walk, people…).</param>
public sealed record MoodWithFactors(int Mood, IReadOnlyCollection<string> Factors);

/// <summary>
/// "What helps me": the person's own data read back to them. Practices that lowered their tension, how their test scores moved,
/// what goes with a better or a worse mood. Thresholds keep a single lucky session or day from reading as a pattern.
/// </summary>
public static class HelpInsights
{
    public const int MinSessions = 2;
    public const int MaxPractices = 3;

    /// <summary>Questionnaires such as PHQ-9 and GAD-7 ask about the last two weeks: an earlier retake would overlap the last one.</summary>
    public static readonly TimeSpan RetestAfter = TimeSpan.FromDays(14);

    public const int MinFactorEntries = 3;
    public const double MinDifference = 0.3;
    public const int MaxFactors = 3;

    /// <summary>Practices with at least <see cref="MinSessions"/> rated sessions that lowered the tension, the strongest first.</summary>
    public static IReadOnlyList<PracticeEffect> PracticeEffects(IEnumerable<SessionResultDTO> results) =>
        results
            .Where(result => !string.IsNullOrWhiteSpace(result.ItemKey) && result.PreIntensity is not null && result.PostIntensity is not null)
            .GroupBy(result => result.ItemKey)
            .Where(group => group.Count() >= MinSessions)
            .Select(group => new PracticeEffect(
                group.Key,
                group.Count(),
                group.Average(result => (double)(result.PreIntensity!.Value - result.PostIntensity!.Value))))
            .Where(effect => effect.AverageDrop > 0)
            .OrderByDescending(effect => effect.AverageDrop)
            .ThenByDescending(effect => effect.Sessions)
            .ThenBy(effect => effect.ItemKey, StringComparer.Ordinal)
            .Take(MaxPractices)
            .ToList();

    /// <summary>
    /// Each scored test: the first and the latest result and whether that is better, judged by the test's own direction. Tests
    /// without a direction (a colour test) are left out. The most recently taken first.
    /// </summary>
    public static IReadOnlyList<TestProgress> TestProgress(IEnumerable<TestResultDTO> results, Func<string, ScoreDirection> direction, DateTime nowUtc) =>
        results
            .Where(result => result.Score is not null && !string.IsNullOrWhiteSpace(result.TestId))
            .GroupBy(result => result.TestId)
            .Select(group => (Direction: direction(group.Key), Ordered: group.OrderBy(result => result.CompletedAt).ToList()))
            .Where(test => test.Direction != ScoreDirection.None)
            .Select(test =>
            {
                TestResultDTO first = test.Ordered[0];
                TestResultDTO latest = test.Ordered[^1];
                TestTrendKind trend = test.Ordered.Count > 1
                    ? TestTrendEvaluator.CompareScores(latest.Score, first.Score, test.Direction)
                    : TestTrendKind.None;
                return new TestProgress(
                    first.TestId,
                    first.Score!.Value,
                    latest.Score!.Value,
                    test.Ordered.Count,
                    latest.CompletedAt,
                    trend,
                    nowUtc - latest.CompletedAt >= RetestAfter);
            })
            .OrderByDescending(progress => progress.LatestUtc)
            .ToList();

    /// <summary>
    /// Journal factors that go with a different mood: compared with the entries without them, both sides with at least
    /// <see cref="MinFactorEntries"/> entries, a difference of at least <see cref="MinDifference"/>. The largest first, either way.
    /// </summary>
    public static IReadOnlyList<FactorEffect> FactorEffects(IReadOnlyList<MoodWithFactors> entries, IEnumerable<string> factors) =>
        factors
            .Select(factor =>
            {
                List<int> with = [.. entries.Where(entry => entry.Factors.Contains(factor)).Select(entry => entry.Mood)];
                List<int> without = [.. entries.Where(entry => !entry.Factors.Contains(factor)).Select(entry => entry.Mood)];
                return with.Count >= MinFactorEntries && without.Count >= MinFactorEntries
                    ? new FactorEffect(factor, with.Count, with.Average() - without.Average())
                    : null;
            })
            .OfType<FactorEffect>()
            .Where(effect => Math.Abs(effect.Difference) >= MinDifference)
            .OrderByDescending(effect => Math.Abs(effect.Difference))
            .ThenBy(effect => effect.Factor, StringComparer.Ordinal)
            .Take(MaxFactors)
            .ToList();
}
