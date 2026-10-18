using Microsoft.Maui.Controls.Shapes;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// Painted backdrops for the practice stages. They are pictures, not interface surfaces: in the dark theme the day scenes turn to dusk
/// (a muted landscape under the moon, a desk in the circle of a lamp) instead of shining at night; text on them uses the ink that suits the picture.
/// </summary>
internal static class StageScenery
{
    /// <summary>Where the sky meets the land, as a share of the canvas height.</summary>
    public const double HorizonShare = 0.46;

    /// <summary>The colour under the landscape while its pieces are laid out.</summary>
    public static Brush LandscapeBase(bool dark) => dark ? Gradient("#1E2846", "#1E3026") : Gradient("#8EC0EA", "#9CC283");

    /// <summary>
    /// A calm day: sky fading to a warm horizon, a soft sun, and a meadow running toward the viewer.
    /// In the dark theme the same place at dusk: an indigo sky with a last warm band, a dim meadow and a pale moon, so it does not dazzle.
    /// </summary>
    public static View[] Landscape(double height, bool dark = false)
    {
        double horizon = height * HorizonShare;
        Border sky = new()
        {
            HeightRequest = horizon,
            StrokeThickness = 0,
            VerticalOptions = LayoutOptions.Start,
            Background = dark ? Gradient("#151D38", "#2A3560", "#5E4E6E") : Gradient("#8EC0EA", "#C9E0F2", "#F6E7CF")
        };
        Border land = new()
        {
            HeightRequest = height - horizon,
            StrokeThickness = 0,
            VerticalOptions = LayoutOptions.End,
            Background = dark ? Gradient("#2F4234", "#25362A", "#1C2A21") : Gradient("#CFE3C0", "#B5D29E", "#9CC283")
        };
        Border haze = new()
        {
            HeightRequest = 18,
            StrokeThickness = 0,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, horizon - 9, 0, 0),
            Background = dark ? Gradient("#00E7B98A", "#40E7B98A", "#00E7B98A") : Gradient("#00FFFFFF", "#AAFFFFFF", "#00FFFFFF")
        };
        Border sun = dark ? Glow(Color.FromArgb("#B8BED8"), 84) : Glow(Color.FromArgb("#FFF4C9"), 120);
        sun.HorizontalOptions = LayoutOptions.End;
        sun.VerticalOptions = LayoutOptions.Start;
        sun.Margin = new Thickness(0, 10, 18, 0);
        return [sky, land, haze, sun];
    }

    /// <summary>A deep night with a few faint stars: the white light of the practice shows against it.</summary>
    public static (Brush Background, View[] Stars) Night(double width = 320, double height = 290)
    {
        Random seeded = new(7);
        List<View> stars = [];
        for (int i = 0; i < 22; i++)
        {
            double size = seeded.NextDouble() < 0.2 ? 3 : 2;
            stars.Add(new Ellipse
            {
                WidthRequest = size,
                HeightRequest = size,
                Fill = new SolidColorBrush(Colors.White),
                Opacity = 0.25 + seeded.NextDouble() * 0.45,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(seeded.NextDouble() * width, seeded.NextDouble() * height * 0.9, 0, 0)
            });
        }

        return (Gradient("#0C1230", "#1A2550", "#2B3A6E"), [.. stars]);
    }

    /// <summary>A quiet evening in deep teal for the minute of pause.</summary>
    public static Brush Twilight() => Gradient("#0E2A33", "#174750", "#22616A");

    /// <summary>A warm wooden desk for the sheet of paper; in the dark theme a dark desk in the circle of a lamp.</summary>
    public static Brush Desk(bool dark = false) => new RadialGradientBrush(
        dark
            ? [new GradientStop(Color.FromArgb("#5A4A38"), 0f), new GradientStop(Color.FromArgb("#2B231B"), 1f)]
            : [new GradientStop(Color.FromArgb("#F1E6D2"), 0f), new GradientStop(Color.FromArgb("#D9C39F"), 1f)],
        new Point(0.5, 0.35),
        0.9);

    public static LinearGradientBrush Gradient(params string[] hexStops)
    {
        GradientStopCollection stops = [];
        for (int i = 0; i < hexStops.Length; i++)
        {
            stops.Add(new GradientStop(Color.FromArgb(hexStops[i]), i / (float)Math.Max(1, hexStops.Length - 1)));
        }

        return new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
    }

    private static Border Glow(Color colour, double size) => new()
    {
        WidthRequest = size,
        HeightRequest = size,
        StrokeThickness = 0,
        StrokeShape = new RoundRectangle { CornerRadius = size / 2 },
        Background = new RadialGradientBrush(
            [new GradientStop(colour, 0f), new GradientStop(colour.WithAlpha(0.5f), 0.35f), new GradientStop(colour.WithAlpha(0f), 1f)],
            new Point(0.5, 0.5),
            0.5)
    };
}
