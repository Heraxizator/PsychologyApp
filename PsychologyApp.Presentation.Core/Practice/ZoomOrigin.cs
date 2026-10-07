namespace PsychologyApp.Presentation.Common;

/// <summary>
/// Where a screen should grow from when it opens from a tapped card: the card's rectangle becomes a starting scale and offset for the
/// whole screen, which then eases to full size. Kept free of the UI toolkit so the geometry can be tested.
/// </summary>
public static class ZoomOrigin
{
    public const double MinScale = 0.3;
    public const double MaxAgeMilliseconds = 1500;

    /// <summary>The scale and the offset of the screen centre so that the screen starts exactly over the card. Null when the card cannot be placed on the page.</summary>
    public static (double Scale, double TranslationX, double TranslationY)? Start(
        double cardX, double cardY, double cardWidth, double cardHeight, double pageWidth, double pageHeight)
    {
        if (pageWidth <= 0 || pageHeight <= 0 || cardWidth <= 0 || cardHeight <= 0)
        {
            return null;
        }

        double centreX = cardX + cardWidth / 2;
        double centreY = cardY + cardHeight / 2;
        if (centreX < 0 || centreX > pageWidth || centreY < 0 || centreY > pageHeight)
        {
            return null;
        }

        double scale = Math.Clamp(Math.Max(cardWidth / pageWidth, cardHeight / pageHeight), MinScale, 1);
        return (scale, centreX - pageWidth / 2, centreY - pageHeight / 2);
    }

    /// <summary>A tap older than this is not what opened the screen.</summary>
    public static bool IsFresh(DateTimeOffset tapped, DateTimeOffset now) => (now - tapped).TotalMilliseconds is >= 0 and <= MaxAgeMilliseconds;
}
