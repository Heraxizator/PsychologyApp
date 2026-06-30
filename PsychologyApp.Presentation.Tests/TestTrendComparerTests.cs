using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Entities.Test;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestTrendComparerTests
{
    [Fact]
    public void CompareScores_LowerIsBetter_DecreaseIsImproved()
    {
        TestTrendKind trend = TestTrendComparer.CompareScores(3, 5, ScoreDirection.LowerIsBetter);
        Assert.Equal(TestTrendKind.Improved, trend);
    }

    [Fact]
    public void CompareScores_HigherIsBetter_IncreaseIsImproved()
    {
        TestTrendKind trend = TestTrendComparer.CompareScores(6, 5, ScoreDirection.HigherIsBetter);
        Assert.Equal(TestTrendKind.Improved, trend);
    }

    [Fact]
    public void CompareScores_None_ReturnsNone()
    {
        TestTrendKind trend = TestTrendComparer.CompareScores(6, 5, ScoreDirection.None);
        Assert.Equal(TestTrendKind.None, trend);
    }
}

