using PsychologyApp.Presentation.Core.Charts;

namespace PsychologyApp.Presentation.Shared.UI.Drawing;

public sealed record TrendLineChartOptions(
    IReadOnlyList<TrendChartPoint> Points,
    int DomainMin,
    int DomainMax,
    Func<DateTime, string> FormatDate,
    Func<int, string> FormatScore,
    IReadOnlyList<TrendAnnotation>? Annotations = null);

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

        Color primary = AppResources.Color("Primary", Colors.SteelBlue);
        Color primaryTint = AppResources.ThemedColor("PrimaryTint", "PrimaryTintDark", primary.WithAlpha(0.25f));
        Color gridColor = AppResources.ThemedColor("NeutralBorder", "InputBorderDark", Colors.Gray.WithAlpha(0.35f));
        Color labelColor = AppResources.ThemedColor("TextSecondaryLight", "TextSecondaryDark", Colors.Gray);
        Color surfaceColor = AppResources.ThemedColor("SurfaceElevatedLight", "SurfaceElevatedDark", Colors.White);

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

        DrawAnnotations(canvas, options, plotLeft, plotTop, plotWidth, plotHeight, plotBottom, gridColor);
        DrawPoints(canvas, canvasPoints, primary, surfaceColor, options.Points.Count == 1 ? 7f : 5f);
        DrawXLabels(canvas, plotLeft, plotWidth, plotBottom, dirtyRect.Left, dirtyRect.Right, layout.XLabels, labelColor);
    }

    /// <summary>A practice is a dashed line up from the axis to the line and a leaf-shaped mark on it: green when the next reading was better, quiet blue otherwise.</summary>
    private static void DrawAnnotations(
        ICanvas canvas,
        TrendLineChartOptions options,
        float plotLeft,
        float plotTop,
        float plotWidth,
        float plotHeight,
        float plotBottom,
        Color lineColor)
    {
        if (options.Annotations is not { Count: > 0 } annotations)
        {
            return;
        }

        Color good = AppResources.ThemedColor("Success", "SuccessDark", Colors.SeaGreen);
        Color quiet = AppResources.Color("Primary", Colors.SteelBlue);
        foreach (PlacedAnnotation mark in TrendLineChartLayout.PlaceAnnotations(options.Points, annotations, options.DomainMin, options.DomainMax))
        {
            (float x, float y) = TrendLineChartLayout.ToCanvasPoint(new NormalizedChartPoint(mark.X, mark.Y), plotLeft, plotTop, plotWidth, plotHeight);
            Color colour = mark.Helped ? good : quiet;

            canvas.StrokeColor = lineColor;
            canvas.StrokeSize = 1;
            canvas.StrokeDashPattern = [3f, 3f];
            canvas.DrawLine(x, plotBottom, x, y);
            canvas.StrokeDashPattern = null;

            float size = 7f;
            PathF diamond = new();
            diamond.MoveTo(x, y - 14 - size);
            diamond.LineTo(x + size, y - 14);
            diamond.LineTo(x, y - 14 + size);
            diamond.LineTo(x - size, y - 14);
            diamond.Close();
            canvas.FillColor = colour;
            canvas.FillPath(diamond);
        }
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

    private const float LabelBoxWidth = 56;

    private static void DrawXLabels(
        ICanvas canvas,
        float plotLeft,
        float plotWidth,
        float plotBottom,
        float canvasLeft,
        float canvasRight,
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
            // The first and the last date sit on the ends of the plot; their boxes are kept inside the canvas, or the outer letters are cut.
            // A box pushed inward aligns its text to the canvas edge, so the date stays next to its point instead of drifting to the middle.
            float boxX = Math.Clamp(x - LabelBoxWidth / 2, canvasLeft, Math.Max(canvasLeft, canvasRight - LabelBoxWidth));
            HorizontalAlignment alignment = boxX > x - LabelBoxWidth / 2
                ? HorizontalAlignment.Left
                : boxX < x - LabelBoxWidth / 2 ? HorizontalAlignment.Right : HorizontalAlignment.Center;
            canvas.DrawString(label, boxX, plotBottom + 6, LabelBoxWidth, 14, alignment, VerticalAlignment.Top);
        }
    }
}
