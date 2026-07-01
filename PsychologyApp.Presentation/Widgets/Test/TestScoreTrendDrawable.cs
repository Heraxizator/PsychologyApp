using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.Test;

public sealed class TestScoreTrendDrawable : IDrawable
{
    public IReadOnlyList<TestScoreChartPoint> Points { get; set; } = [];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Points.Count < 2)
        {
            return;
        }

        float padding = 12f;
        float width = Math.Max(1f, dirtyRect.Width - padding * 2);
        float height = Math.Max(1f, dirtyRect.Height - padding * 2);

        int minScore = Points.Min(point => point.Score);
        int maxScore = Points.Max(point => point.Score);
        if (minScore == maxScore)
        {
            maxScore = minScore + 1;
        }

        float scoreRange = maxScore - minScore;
        PointF[] mapped = new PointF[Points.Count];
        for (int i = 0; i < Points.Count; i++)
        {
            float x = padding + width * i / Math.Max(1, Points.Count - 1);
            float normalized = (Points[i].Score - minScore) / scoreRange;
            float y = padding + height - normalized * height;
            mapped[i] = new PointF(x, y);
        }

        canvas.StrokeColor = Colors.Gray.WithAlpha(0.35f);
        canvas.StrokeSize = 1;
        canvas.DrawLine(padding, padding + height, padding + width, padding + height);
        canvas.DrawLine(padding, padding, padding, padding + height);

        Color strokeColor = Colors.SteelBlue;
        if (Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue("Primary", out object? primary) == true &&
            primary is Color color)
        {
            strokeColor = color;
        }

        canvas.StrokeColor = strokeColor;
        canvas.StrokeSize = 2;
        for (int i = 1; i < mapped.Length; i++)
        {
            canvas.DrawLine(mapped[i - 1].X, mapped[i - 1].Y, mapped[i].X, mapped[i].Y);
        }

        canvas.FillColor = strokeColor;
        foreach (PointF point in mapped)
        {
            canvas.FillCircle(point.X, point.Y, 4);
        }

        canvas.FontSize = 10;
        canvas.FontColor = Colors.Gray;
        canvas.DrawString(
            AppStrings.TestHistoryScore(minScore),
            padding,
            dirtyRect.Bottom - padding,
            dirtyRect.Width / 2,
            14,
            HorizontalAlignment.Left,
            VerticalAlignment.Top);
        canvas.DrawString(
            AppStrings.TestHistoryScore(maxScore),
            padding,
            padding,
            dirtyRect.Width / 2,
            14,
            HorizontalAlignment.Left,
            VerticalAlignment.Top);
    }
}
