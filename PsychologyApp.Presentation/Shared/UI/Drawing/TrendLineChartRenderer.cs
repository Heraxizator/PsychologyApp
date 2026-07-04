using PsychologyApp.Presentation.Core.Charts;

namespace PsychologyApp.Presentation.Shared.UI.Drawing;

public sealed record TrendLineChartOptions(
    IReadOnlyList<TrendChartPoint> Points,
    int DomainMin,
    int DomainMax,
    Func<DateTime, string> FormatDate,
    Func<int, string> FormatScore);

public static class TrendLineChartRenderer
{
    public static void Draw(ICanvas canvas, RectF dirtyRect, TrendLineChartOptions options)
    {
        if (options.Points.Count == 0)
        {
            return;
        }

        TrendLineChartLayoutResult layout = TrendLineChartLayout.Build(
            options.Points,
            options.DomainMin,
            options.DomainMax,
            options.FormatDate);

        (float plotLeft, float plotTop, float plotWidth, float plotHeight) =
            TrendLineChartLayout.GetPlotRect(dirtyRect.Left, dirtyRect.Top, dirtyRect.Width, dirtyRect.Height);

        float plotRight = plotLeft + plotWidth;
        float plotBottom = plotTop + plotHeight;

        Color primary = ResolveColor("Primary", Colors.SteelBlue);
        Color primaryTint = ResolveColor("PrimaryTint", primary.WithAlpha(0.25f));
        Color gridColor = ResolveColor("NeutralBorder", Colors.Gray.WithAlpha(0.35f));
        Color labelColor = ResolveColor("TextSecondary", Colors.Gray);
        Color surfaceColor = ResolveColor("SurfaceElevatedLight", Colors.White);

        DrawGrid(canvas, plotLeft, plotTop, plotWidth, plotHeight, plotRight, plotBottom, layout, gridColor, labelColor, options.FormatScore);

        PointF[] canvasPoints = layout.MappedPoints
            .Select(point =>
            {
                (float x, float y) = TrendLineChartLayout.ToCanvasPoint(point, plotLeft, plotTop, plotWidth, plotHeight);
                return new PointF(x, y);
            })
            .ToArray();

        if (canvasPoints.Length > 1)
        {
            DrawAreaFill(canvas, canvasPoints, plotBottom, primaryTint);
            DrawLine(canvas, canvasPoints, primary);
        }

        DrawPoints(canvas, canvasPoints, primary, surfaceColor, options.Points.Count == 1 ? 7f : 5f);
        DrawXLabels(canvas, plotLeft, plotWidth, plotBottom, layout.XLabels, labelColor);
    }

    private static void DrawGrid(
        ICanvas canvas,
        float plotLeft,
        float plotTop,
        float plotWidth,
        float plotHeight,
        float plotRight,
        float plotBottom,
        TrendLineChartLayoutResult layout,
        Color gridColor,
        Color labelColor,
        Func<int, string> formatScore)
    {
        canvas.StrokeColor = gridColor;
        canvas.StrokeSize = 1;
        canvas.DrawLine(plotLeft, plotBottom, plotRight, plotBottom);

        float scoreRange = Math.Max(1, layout.YMax - layout.YMin);
        foreach (int tick in layout.YTickValues)
        {
            float normalized = (tick - layout.YMin) / scoreRange;
            float y = plotTop + plotHeight - normalized * plotHeight;

            canvas.StrokeColor = gridColor.WithAlpha(0.6f);
            canvas.DrawLine(plotLeft, y, plotRight, y);

            canvas.FontSize = 10;
            canvas.FontColor = labelColor;
            canvas.DrawString(
                formatScore(tick),
                plotRight + 4,
                y - 7,
                32,
                14,
                HorizontalAlignment.Left,
                VerticalAlignment.Top);
        }
    }

    private static void DrawAreaFill(ICanvas canvas, PointF[] points, float baselineY, Color fillColor)
    {
        if (points.Length < 2)
        {
            return;
        }

        PathF path = new();
        path.MoveTo(points[0].X, baselineY);
        path.LineTo(points[0].X, points[0].Y);
        for (int index = 1; index < points.Length; index++)
        {
            path.LineTo(points[index].X, points[index].Y);
        }

        path.LineTo(points[^1].X, baselineY);
        path.Close();
        canvas.FillColor = fillColor;
        canvas.FillPath(path);
    }

    private static void DrawLine(ICanvas canvas, PointF[] points, Color strokeColor)
    {
        canvas.StrokeColor = strokeColor;
        canvas.StrokeSize = 2.5f;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;
        for (int index = 1; index < points.Length; index++)
        {
            canvas.DrawLine(points[index - 1].X, points[index - 1].Y, points[index].X, points[index].Y);
        }
    }

    private static void DrawPoints(
        ICanvas canvas,
        PointF[] points,
        Color fillColor,
        Color ringColor,
        float radius)
    {
        foreach (PointF point in points)
        {
            canvas.FillColor = ringColor;
            canvas.FillCircle(point.X, point.Y, radius + 1.5f);
            canvas.FillColor = fillColor;
            canvas.FillCircle(point.X, point.Y, radius);
        }
    }

    private static void DrawXLabels(
        ICanvas canvas,
        float plotLeft,
        float plotWidth,
        float plotBottom,
        IReadOnlyList<(int Index, string Label)> labels,
        Color labelColor)
    {
        if (labels.Count == 0)
        {
            return;
        }

        canvas.FontSize = 10;
        canvas.FontColor = labelColor;
        int pointCount = labels.Max(label => label.Index) + 1;

        foreach ((int index, string label) in labels)
        {
            float normalizedX = TrendLineChartLayout.MapNormalizedX(index, pointCount);
            float x = plotLeft + normalizedX * plotWidth;
            canvas.DrawString(
                label,
                x - 28,
                plotBottom + 6,
                56,
                14,
                HorizontalAlignment.Center,
                VerticalAlignment.Top);
        }
    }

    private static Color ResolveColor(string key, Color fallback)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true &&
            value is Color color)
        {
            return color;
        }

        return fallback;
    }
}
