using PsychologyApp.Application.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class TestScoreRangeTests
{
    [Theory]
    [InlineData("beck", 0, 63)]
    [InlineData("gad7", 0, 21)]
    [InlineData("who5", 0, 25)]
    [InlineData("swls", 5, 35)]
    [InlineData("rses", 0, 30)]
    public void GetScoreRange_ReturnsKnownDomain(string analyzerId, int expectedMin, int expectedMax)
    {
        ScoreRange? range = TestScoreInterpreter.GetScoreRange(analyzerId);

        Assert.NotNull(range);
        Assert.Equal(expectedMin, range.Value.Min);
        Assert.Equal(expectedMax, range.Value.Max);
    }

    [Fact]
    public void GetScoreRange_ReturnsNull_ForUnknownAnalyzer() =>
        Assert.Null(TestScoreInterpreter.GetScoreRange("unknown"));
}
