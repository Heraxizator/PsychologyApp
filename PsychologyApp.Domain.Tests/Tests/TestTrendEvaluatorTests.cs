using PsychologyApp.Domain.Tests;
using Xunit;

namespace PsychologyApp.Domain.Tests.Tests;

public sealed class TestTrendEvaluatorTests
{
    [Theory]
    [InlineData(3, 8, ScoreDirection.LowerIsBetter, TestTrendKind.Improved)]
    [InlineData(8, 3, ScoreDirection.LowerIsBetter, TestTrendKind.Worse)]
    [InlineData(20, 10, ScoreDirection.HigherIsBetter, TestTrendKind.Improved)]
    [InlineData(5, 5, ScoreDirection.LowerIsBetter, TestTrendKind.Same)]
    public void CompareScores_RespectsDirection(int current, int previous, ScoreDirection direction, TestTrendKind expected) =>
        Assert.Equal(expected, TestTrendEvaluator.CompareScores(current, previous, direction));

    [Fact]
    public void CompareScores_ReturnsNone_WhenScoreMissing() =>
        Assert.Equal(TestTrendKind.None, TestTrendEvaluator.CompareScores(null, 5, ScoreDirection.LowerIsBetter));
}
