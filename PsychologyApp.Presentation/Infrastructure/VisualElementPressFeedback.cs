namespace PsychologyApp.Presentation.Infrastructure;

public static class VisualElementPressFeedback
{
    private const uint PressDuration = 80;
    private const uint ReleaseDuration = 120;

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

    public static void AttachToTemplateRoot(ContentView contentView, string borderStyleKey = "ListCardItemStyle")
    {
        contentView.Loaded += (_, _) =>
        {
            VisualElement? target = FindPressTarget(contentView);
            if (target is not null)
            {
                Attach(target);
            }
        };
    }

    private static async Task AnimatePressAsync(VisualElement target)
    {
        if (!UiAnimations.CanAnimate(target) || !AnimatingViews.Add(target))
        {
            return;
        }

        try
        {
            await target.ScaleToAsync(UiAnimations.PressScale, PressDuration, UiAnimations.EnterEasing);
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
            if (Math.Abs(target.Scale - 1) > 0.01)
            {
                target.Scale = 1;
            }

            return;
        }

        try
        {
            await target.ScaleToAsync(1, ReleaseDuration, UiAnimations.ReleaseEasing);
        }
        finally
        {
            AnimatingViews.Remove(target);
            UiAnimations.ResetVisualState(target);
        }
    }

    private static VisualElement? FindPressTarget(VisualElement root)
    {
        if (root is Border border)
        {
            return border;
        }

        foreach (VisualElement child in root.GetVisualTreeDescendants().OfType<VisualElement>())
        {
            if (child is Border)
            {
                return child;
            }
        }

        return root;
    }
}
