using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.UI.Drawing;

namespace PsychologyApp.Presentation.Widgets.Profile;

public sealed class MoodTrendDrawable : IDrawable
{
    private const int MoodDomainMin = 1;
    private const int MoodDomainMax = 5;

    public IReadOnlyList<MoodChartPoint> Points { get; set; } = [];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (Points.Count == 0)
        {
            return;
        }

        IReadOnlyList<TrendChartPoint> trendPoints = Points
            .Select(point => new TrendChartPoint(point.RecordedAtLocal, point.MoodLevel))
            .ToList();

        TrendLineChartRenderer.Draw(
            canvas,
            dirtyRect,
            new TrendLineChartOptions(
                trendPoints,
                MoodDomainMin,
                MoodDomainMax,
                AppStrings.ChartDateLabel,
                static score => score.ToString()));
    }
}
