using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Core.Charts;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Charts;

public sealed class TrendLineChartLayoutTests
{
    [Fact]
    public void ResolveYRange_WithCloseValues_UsesMinimumSpan()
    {
        (int yMin, int yMax) = TrendLineChartLayout.ResolveYRange([7, 8], 0, 63);

        Assert.True(yMax - yMin >= 4);
        Assert.InRange(7, yMin, yMax);
        Assert.InRange(8, yMin, yMax);
    }

    [Fact]
    public void MapNormalizedX_WithTwoPoints_UsesInset()
    {
        Assert.Equal(0.2f, TrendLineChartLayout.MapNormalizedX(0, 2));
        Assert.Equal(0.8f, TrendLineChartLayout.MapNormalizedX(1, 2));
    }

    [Fact]
    public void MapNormalizedX_WithSinglePoint_Centers()
    {
        Assert.Equal(0.5f, TrendLineChartLayout.MapNormalizedX(0, 1));
    }

    [Fact]
    public void Build_WithMoodDomain_UsesFixedRangeTicks()
    {
        TrendLineChartLayoutResult layout = TrendLineChartLayout.Build(
            [new TrendChartPoint(new DateTime(2026, 1, 1), 3), new TrendChartPoint(new DateTime(2026, 1, 2), 4)],
            1,
            5,
            static date => date.ToString("dd MMM"));

        Assert.Equal(1, layout.YMin);
        Assert.Equal(5, layout.YMax);
        Assert.Equal(2, layout.MappedPoints.Count);
    }

    [Fact]
    public void ResolveChartSubtitle_ReturnsExpectedText()
    {
        Assert.False(string.IsNullOrWhiteSpace(AppStrings.ResolveChartSubtitle(1)));
        Assert.False(string.IsNullOrWhiteSpace(AppStrings.ResolveChartSubtitle(2)));
        Assert.Equal(string.Empty, AppStrings.ResolveChartSubtitle(5));
    }
}
