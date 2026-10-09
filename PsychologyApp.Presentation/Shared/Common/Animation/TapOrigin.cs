using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Remembers where the last tapped card was on the page, so the screen it opens can grow out of that card. Pages that do not ask for it are
/// unaffected; a tap that is more than a moment old is ignored.
/// </summary>
public static class TapOrigin
{
    private static Rect _bounds;
    private static DateTimeOffset _tappedAt = DateTimeOffset.MinValue;

    /// <summary>A tap that can still be what opened the screen being built right now.</summary>
    public static bool HasRecentTap => ZoomOrigin.IsFresh(_tappedAt, DateTimeOffset.UtcNow);

    public static void Record(VisualElement view)
    {
        if (TryBoundsOnPage(view) is { } bounds)
        {
            _bounds = bounds;
            _tappedAt = DateTimeOffset.UtcNow;
        }
    }

    /// <summary>The start of the zoom for a page of this size, or null when no recent tap can be placed on it. Used once per tap.</summary>
    public static (double Scale, double TranslationX, double TranslationY)? TakeStartFor(double pageWidth, double pageHeight)
    {
        if (!ZoomOrigin.IsFresh(_tappedAt, DateTimeOffset.UtcNow))
        {
            return null;
        }

        _tappedAt = DateTimeOffset.MinValue;
        return ZoomOrigin.Start(_bounds.X, _bounds.Y, _bounds.Width, _bounds.Height, pageWidth, pageHeight);
    }

    /// <summary>Position of a view within its page, adding up the offsets of its parents and subtracting what a scroll view has scrolled.</summary>
    /// <summary>Where the last tap was, if it was a moment ago (not consumed: the zoom of a page uses it separately).</summary>
    public static bool TryPeek(out Rect bounds)
    {
        bounds = _bounds;
        return HasRecentTap && bounds.Width > 0;
    }

    private static Rect? TryBoundsOnPage(VisualElement view)
    {
        double x = 0;
        double y = 0;
        Element? current = view;
        while (current is VisualElement element and not Page)
        {
            x += element.X;
            y += element.Y;
            if (element.Parent is ScrollView scroll)
            {
                x -= scroll.ScrollX;
                y -= scroll.ScrollY;
            }

            current = element.Parent;
        }

        return view.Width > 0 && view.Height > 0 ? new Rect(x, y, view.Width, view.Height) : null;
    }
}
