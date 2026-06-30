using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Application.Tests;

public sealed record TestTrendEvaluation(TestTrendKind Kind, bool HasBadge);

public interface ITestTrendService
{
    TestTrendKind Compare(int? current, int? previous, ScoreDirection direction);

    Task<ScoreDirection> ResolveDirectionAsync(string testId, CancellationToken cancellationToken = default);

    Task<TestTrendEvaluation?> LoadLatestTrendAsync(
        string testId,
        IUserProgressService userProgressService,
        CancellationToken cancellationToken = default);

    IReadOnlyList<TestScoreChartPoint> BuildChartPoints(IReadOnlyList<TestResultDTO> history);
}

public sealed class TestTrendService(ITestCatalogService testCatalogService) : ITestTrendService
{
    public TestTrendKind Compare(int? current, int? previous, ScoreDirection direction) =>
        TestTrendEvaluator.CompareScores(current, previous, direction);

    public async Task<ScoreDirection> ResolveDirectionAsync(string testId, CancellationToken cancellationToken = default)
    {
        TestDefinition? definition = await testCatalogService.GetByIdAsync(testId, cancellationToken);
        return definition?.ScoreDirection ?? ScoreDirection.LowerIsBetter;
    }

    public async Task<TestTrendEvaluation?> LoadLatestTrendAsync(
        string testId,
        IUserProgressService userProgressService,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TestResultDTO> history =
            await userProgressService.GetTestResultHistoryAsync(testId, 2, cancellationToken);

        if (history.Count < 2)
        {
            return null;
        }

        ScoreDirection direction = await ResolveDirectionAsync(testId, cancellationToken);
        TestTrendKind kind = Compare(history[0].Score, history[1].Score, direction);
        return new TestTrendEvaluation(kind, kind is not TestTrendKind.None);
    }

    public IReadOnlyList<TestScoreChartPoint> BuildChartPoints(IReadOnlyList<TestResultDTO> history)
    {
        List<TestScoreChartPoint> points = [];
        for (int i = history.Count - 1; i >= 0; i--)
        {
            TestResultDTO item = history[i];
            if (item.Score is int score)
            {
                points.Add(new TestScoreChartPoint(item.CompletedAt, score));
            }
        }

        return points;
    }
}
