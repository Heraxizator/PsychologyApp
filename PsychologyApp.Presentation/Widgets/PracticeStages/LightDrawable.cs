using Microsoft.Maui.Graphics;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// A soft round light drawn on a <see cref="GraphicsView"/>: bright at the centre, fading to nothing at the edge, optionally over a
/// "dawn" wash that fills the whole canvas. Drawn by hand because a radial brush on a rounded border does not paint reliably on Android.
/// </summary>
internal sealed class LightDrawable : IDrawable
{
    /// <summary>Radius of the light as a share of the canvas diagonal.</summary>
    public float Radius { get; set; } = 0.1f;

    /// <summary>0..1: how bright the light is.</summary>
    public float Intensity { get; set; }

    /// <summary>0..1: how much of the canvas is washed with the soft light of the end.</summary>
    public float Wash { get; set; }

    public Color Core { get; set; } = Colors.White;

    public Color WashCentre { get; set; } = Color.FromArgb("#FFFFFF");

    public Color WashEdge { get; set; } = Color.FromArgb("#E3E6F5");

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        PointF centre = dirtyRect.Center;
        float diagonal = MathF.Sqrt(dirtyRect.Width * dirtyRect.Width + dirtyRect.Height * dirtyRect.Height);

        if (Wash > 0)
        {
            RadialGradientPaint wash = new(
                [
                    new PaintGradientStop(0f, WashCentre.WithAlpha(Wash)),
                    new PaintGradientStop(1f, WashEdge.WithAlpha(Wash))
                ],
                new Point(0.5, 0.5),
                0.75);
            canvas.SetFillPaint(wash, dirtyRect);
            canvas.FillRectangle(dirtyRect);
        }

        float radius = Math.Max(1, Radius * diagonal);
        if (Intensity <= 0)
        {
            return;
        }

        RectF bounds = new(centre.X - radius, centre.Y - radius, radius * 2, radius * 2);
        RadialGradientPaint light = new(
            [
                new PaintGradientStop(0f, Core.WithAlpha(Intensity)),
                new PaintGradientStop(0.4f, Core.WithAlpha(Intensity * 0.6f)),
                new PaintGradientStop(1f, Core.WithAlpha(0f))
            ],
            new Point(0.5, 0.5),
            0.5);
        canvas.SetFillPaint(light, bounds);
        canvas.FillEllipse(bounds);
    }
}
