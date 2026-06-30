using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record TestTrendSnapshot(TestTrendKind Kind, string Label, bool HasBadge);

public sealed class TestTrendResolver(ITestTrendService trendService)
{
    public Task<ScoreDirection> ResolveDirectionAsync(string testId, CancellationToken cancellationToken = default) =>
        trendService.ResolveDirectionAsync(testId, cancellationToken);

    public TestTrendKind Compare(int? current, int? previous, ScoreDirection direction) =>
        trendService.Compare(current, previous, direction);

    public async Task<TestTrendSnapshot?> LoadLatestTrendAsync(
        string testId,
        IUserProgressService userProgressService,
        CancellationToken cancellationToken = default)
    {
        TestTrendEvaluation? snapshot = await trendService.LoadLatestTrendAsync(
            testId,
            userProgressService,
            cancellationToken);

        if (snapshot is null)
        {
            return null;
        }

        return new TestTrendSnapshot(
            snapshot.Kind,
            TestTrendComparer.ToLabel(snapshot.Kind),
            snapshot.HasBadge);
    }

    public IReadOnlyList<TestScoreChartPoint> BuildChartPoints(IReadOnlyList<TestResultDTO> history) =>
        trendService.BuildChartPoints(history);
}
