using PsychologyApp.Application.Body;

namespace PsychologyApp.Presentation.Body;

/// <param name="IsEllipse">The head is round; the rest are rounded rectangles.</param>
public sealed record BodyMapShape(BodyArea Area, double X, double Y, double Width, double Height, bool IsEllipse = false)
{
    public bool Contains(double x, double y)
    {
        if (!IsEllipse)
        {
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        double dx = (x - (X + Width / 2)) / (Width / 2);
        double dy = (y - (Y + Height / 2)) / (Height / 2);
        return dx * dx + dy * dy <= 1;
    }
}

/// <summary>
/// The front silhouette of the Body page in a 100 × 200 box: what is drawn and what a tap hits come from the same shapes. The back and
/// the whole body cannot be pointed at from the front; they are chosen from the list under the figure.
/// </summary>
public static class BodyMapLayout
{
    public const double BoxWidth = 100;
    public const double BoxHeight = 200;

    /// <summary>In drawing order; a tap is checked from the last one back, so the face wins over the head around it.</summary>
    public static IReadOnlyList<BodyMapShape> Shapes { get; } =
    [
        new(BodyArea.Head, 37, 2, 26, 28, IsEllipse: true),
        // The lower part of the head: the middle of the head stays "head", which is where a headache is pointed at.
        new(BodyArea.Face, 42, 18, 16, 11),
        new(BodyArea.Throat, 44, 30, 12, 8),
        new(BodyArea.Arms, 12, 40, 12, 58),
        new(BodyArea.Arms, 76, 40, 12, 58),
        new(BodyArea.Shoulders, 24, 38, 52, 9),
        new(BodyArea.Chest, 28, 47, 44, 24),
        new(BodyArea.Stomach, 30, 71, 40, 25),
        new(BodyArea.Legs, 31, 97, 17, 100),
        new(BodyArea.Legs, 52, 97, 17, 100)
    ];

    /// <summary>The scale and offset that fit the box into a view, keeping its proportions and centring it.</summary>
    public static (double Scale, double OffsetX, double OffsetY) Fit(double viewWidth, double viewHeight)
    {
        double scale = Math.Min(viewWidth / BoxWidth, viewHeight / BoxHeight);
        return (scale, (viewWidth - BoxWidth * scale) / 2, (viewHeight - BoxHeight * scale) / 2);
    }

    /// <summary>The place under a tap at (x, y) in view coordinates, or null for a tap beside the figure.</summary>
    public static BodyArea? AreaAt(double x, double y, double viewWidth, double viewHeight)
    {
        (double scale, double offsetX, double offsetY) = Fit(viewWidth, viewHeight);
        if (scale <= 0)
        {
            return null;
        }

        double boxX = (x - offsetX) / scale;
        double boxY = (y - offsetY) / scale;
        for (int index = Shapes.Count - 1; index >= 0; index--)
        {
            if (Shapes[index].Contains(boxX, boxY))
            {
                return Shapes[index].Area;
            }
        }

        return null;
    }
}
