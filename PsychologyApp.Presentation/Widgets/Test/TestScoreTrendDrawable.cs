using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.UI.Drawing;

namespace PsychologyApp.Presentation.Widgets.Test;

public sealed class TestScoreTrendDrawable : IDrawable
{
    public IReadOnlyList<TestScoreChartPoint> Points { get; set; } = [];
    public int DomainMin { get; set; }
    public int DomainMax { get; set; } = 10;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Points.Count == 0)
        {
            return;
        }

        IReadOnlyList<TrendChartPoint> trendPoints = Points
            .Select(point => new TrendChartPoint(point.CompletedAt, point.Score))
            .ToList();

        TrendLineChartRenderer.Draw(
            canvas,
            dirtyRect,
            new TrendLineChartOptions(
                trendPoints,
                DomainMin,
                DomainMax,
                AppStrings.ChartDateLabel,
                static score => score.ToString()));
    }
}
