namespace PsychologyApp.Presentation.Core.Charts;

public readonly record struct NormalizedChartPoint(float X, float Y);

public sealed record TrendLineChartLayoutResult(
    IReadOnlyList<NormalizedChartPoint> MappedPoints,
    int YMin,
    int YMax,
    IReadOnlyList<int> YTickValues,
    IReadOnlyList<(int Index, string Label)> XLabels);

public static class TrendLineChartLayout
{
    private const float PlotPaddingLeft = 12f;
    private const float PlotPaddingRight = 36f;
    private const float PlotPaddingTop = 8f;
    private const float PlotPaddingBottom = 28f;
    private const float TwoPointInsetRatio = 0.2f;

    public static TrendLineChartLayoutResult Build(
        IReadOnlyList<TrendChartPoint> points,
        int domainMin,
        int domainMax,
        Func<DateTime, string> formatDate,
        int minSpan = 4)
    {
        if (points.Count == 0)
        {
            return new TrendLineChartLayoutResult([], domainMin, domainMax, [], []);
        }

        IReadOnlyList<int> values = points.Select(point => point.Value).ToList();
        (int yMin, int yMax) = ResolveYRange(values, domainMin, domainMax, minSpan);
        float scoreRange = Math.Max(1, yMax - yMin);

        List<NormalizedChartPoint> mapped = [];
        for (int index = 0; index < points.Count; index++)
        {
            float normalizedX = MapNormalizedX(index, points.Count);
            float normalizedY = (points[index].Value - yMin) / scoreRange;
            mapped.Add(new NormalizedChartPoint(normalizedX, normalizedY));
        }

        IReadOnlyList<int> yTicks = BuildYTicks(yMin, yMax);
        IReadOnlyList<(int Index, string Label)> xLabels = BuildXLabels(points, formatDate);
        return new TrendLineChartLayoutResult(mapped, yMin, yMax, yTicks, xLabels);
    }

    public static (float Left, float Top, float Width, float Height) GetPlotRect(
        float dirtyLeft,
        float dirtyTop,
        float dirtyWidth,
        float dirtyHeight) =>
        (
            dirtyLeft + PlotPaddingLeft,
            dirtyTop + PlotPaddingTop,
            Math.Max(1f, dirtyWidth - PlotPaddingLeft - PlotPaddingRight),
            Math.Max(1f, dirtyHeight - PlotPaddingTop - PlotPaddingBottom));

    public static (float X, float Y) ToCanvasPoint(NormalizedChartPoint normalizedPoint, float plotLeft, float plotTop, float plotWidth, float plotHeight) =>
        (
            plotLeft + normalizedPoint.X * plotWidth,
            plotTop + plotHeight - normalizedPoint.Y * plotHeight);

    public static (int YMin, int YMax) ResolveYRange(
        IReadOnlyList<int> values,
        int domainMin,
        int domainMax,
        int minSpan = 4)
    {
        if (values.Count == 0)
        {
            return (domainMin, domainMax);
        }

        int dataMin = values.Min();
        int dataMax = values.Max();
        int pad = Math.Max(1, (domainMax - domainMin) / 10);

        int yMin = Math.Max(domainMin, dataMin - pad);
        int yMax = Math.Min(domainMax, dataMax + pad);

        if (yMax - yMin < minSpan)
        {
            int center = (dataMin + dataMax) / 2;
            yMin = Math.Max(domainMin, center - minSpan / 2);
            yMax = Math.Min(domainMax, yMin + minSpan);
            if (yMax - yMin < minSpan)
            {
                yMin = Math.Max(domainMin, yMax - minSpan);
            }
        }

        return (yMin, yMax);
    }

    public static float MapNormalizedX(int index, int count)
    {
        if (count <= 1)
        {
            return 0.5f;
        }

        if (count == 2)
        {
            return index == 0 ? TwoPointInsetRatio : 1f - TwoPointInsetRatio;
        }

        return index / (float)(count - 1);
    }

    private static IReadOnlyList<int> BuildYTicks(int yMin, int yMax)
    {
        if (yMax <= yMin)
        {
            return [yMin];
        }

        int span = yMax - yMin;
        int step = span <= 4 ? 1 : span <= 10 ? 2 : span <= 25 ? 5 : 10;
        List<int> ticks = [];
        for (int value = yMin; value <= yMax; value += step)
        {
            ticks.Add(value);
        }

        if (ticks.Count == 0 || ticks[^1] != yMax)
        {
            ticks.Add(yMax);
        }

        return ticks;
    }

    private static IReadOnlyList<(int Index, string Label)> BuildXLabels(
        IReadOnlyList<TrendChartPoint> points,
        Func<DateTime, string> formatDate)
    {
        if (points.Count == 0)
        {
            return [];
        }

        if (points.Count == 1)
        {
            return [(0, formatDate(points[0].OccurredAt))];
        }

        List<(int Index, string Label)> labels =
        [
            (0, formatDate(points[0].OccurredAt)),
            (points.Count - 1, formatDate(points[^1].OccurredAt))
        ];

        if (points.Count == 3)
        {
            labels.Insert(1, (1, formatDate(points[1].OccurredAt)));
        }

        return labels;
    }
}
