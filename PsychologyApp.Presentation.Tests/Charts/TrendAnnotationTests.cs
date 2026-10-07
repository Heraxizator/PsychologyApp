using PsychologyApp.Presentation.Core.Charts;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Charts;

/// <summary>Practices marked on the mood line: where they fall and whether the mood rose after them.</summary>
public class TrendAnnotationTests
{
    private static readonly DateTime Day1 = new(2026, 10, 1, 8, 0, 0);

    private static TrendChartPoint[] Line(params int[] values) =>
        [.. values.Select((v, i) => new TrendChartPoint(Day1.AddDays(i), v))];

    [Fact]
    public void APracticeBetweenTwoReadingsSitsBetweenThemInProportionToTheTime()
    {
        TrendChartPoint[] points = Line(2, 4, 4);

        IReadOnlyList<PlacedAnnotation> placed = TrendLineChartLayout.PlaceAnnotations(points, [new TrendAnnotation(Day1.AddHours(12))], 1, 5);

        PlacedAnnotation mark = Assert.Single(placed);
        Assert.Equal(0.25f, mark.X, 3);
    }

    [Fact]
    public void ItIsMarkedAsHelpfulWhenTheNextReadingIsHigher()
    {
        TrendChartPoint[] points = Line(2, 4, 3);

        PlacedAnnotation up = Assert.Single(TrendLineChartLayout.PlaceAnnotations(points, [new TrendAnnotation(Day1.AddHours(6))], 1, 5));
        PlacedAnnotation down = Assert.Single(TrendLineChartLayout.PlaceAnnotations(points, [new TrendAnnotation(Day1.AddDays(1).AddHours(6))], 1, 5));

        Assert.True(up.Helped);
        Assert.False(down.Helped);
    }

    [Fact]
    public void APracticeOutsideTheReadingsOrWithTooFewReadingsIsDropped()
    {
        TrendChartPoint[] points = Line(2, 4);

        Assert.Empty(TrendLineChartLayout.PlaceAnnotations(points, [new TrendAnnotation(Day1.AddDays(-1)), new TrendAnnotation(Day1.AddDays(5))], 1, 5));
        Assert.Empty(TrendLineChartLayout.PlaceAnnotations(Line(3), [new TrendAnnotation(Day1)], 1, 5));
        Assert.Empty(TrendLineChartLayout.PlaceAnnotations(points, [], 1, 5));
    }

    [Fact]
    public void TheMarkSitsOnTheLineAtItsHeight()
    {
        TrendChartPoint[] points = Line(1, 5, 5);

        PlacedAnnotation mark = Assert.Single(TrendLineChartLayout.PlaceAnnotations(points, [new TrendAnnotation(Day1.AddHours(12))], 1, 5));

        Assert.InRange(mark.Y, 0.4f, 0.6f);
    }
}
