namespace PsychologyApp.Presentation.Infrastructure;

public static class VisualElementPressFeedback
{
    private const uint PressDuration = 80;
    private const uint ReleaseDuration = 120;
    private const double PressOpacity = 0.88;
    private const double PressScale = 0.96;
    private const double PressTranslationY = 1;

    private static readonly HashSet<VisualElement> AnimatingViews = [];

    public static void Attach(VisualElement? view)
    {
        if (view is not View target)
        {
            return;
        }

        PointerGestureRecognizer pointer = new();
        pointer.PointerPressed += async (_, _) => await AnimatePressAsync(target);
        pointer.PointerReleased += async (_, _) => await AnimateReleaseAsync(target);
        pointer.PointerExited += async (_, _) => await AnimateReleaseAsync(target);

        target.GestureRecognizers.Add(pointer);
    }

    public static void AttachToTemplateRoot(ContentView contentView, string borderStyleKey = "ListCardItemStyle") =>
        TemplatePressFeedback.Attach(contentView);

    private static async Task AnimatePressAsync(VisualElement target)
    {
        if (!UiAnimations.CanAnimate(target) || !AnimatingViews.Add(target))
        {
            return;
        }

        try
        {
            await Task.WhenAll(
                target.ScaleToAsync(PressScale, PressDuration, UiAnimations.EnterEasing),
                target.FadeToAsync(PressOpacity, PressDuration, UiAnimations.EnterEasing),
                target.TranslateToAsync(0, PressTranslationY, PressDuration, UiAnimations.EnterEasing));
        }
        finally
        {
            AnimatingViews.Remove(target);
        }
    }

    private static async Task AnimateReleaseAsync(VisualElement target)
    {
        if (!UiAnimations.CanAnimate(target) || !AnimatingViews.Add(target))
        {
            ResetInstant(target);
            return;
        }

        try
        {
            await Task.WhenAll(
                target.ScaleToAsync(1, ReleaseDuration, UiAnimations.ReleaseEasing),
                target.FadeToAsync(1, ReleaseDuration, UiAnimations.ReleaseEasing),
                target.TranslateToAsync(0, 0, ReleaseDuration, UiAnimations.ReleaseEasing));
        }
        finally
        {
            AnimatingViews.Remove(target);
            UiAnimations.ResetVisualState(target);
        }
    }

    private static void ResetInstant(VisualElement target)
    {
        if (Math.Abs(target.Scale - 1) > 0.01
            || Math.Abs(target.Opacity - 1) > 0.01
            || Math.Abs(target.TranslationY) > 0.01)
        {
            UiAnimations.ResetVisualState(target);
        }
    }
}
