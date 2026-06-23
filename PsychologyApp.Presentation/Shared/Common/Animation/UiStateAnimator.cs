namespace PsychologyApp.Presentation.Shared.Common;

public static class UiStateAnimator
{
    private const double VisibilitySlideOffset = 8;

    public static async Task AnimateVisibilityAsync(
        VisualElement? element,
        bool visible,
        CancellationToken cancellationToken = default)
    {
        if (element is null)
        {
            return;
        }

        if (visible)
        {
            element.IsVisible = true;
            await UiAnimations.SafeRevealPremiumAsync(
                element,
                VisibilitySlideOffset,
                UiAnimations.MediumDuration,
                allowHidden: true,
                cancellationToken: cancellationToken);
            return;
        }

        if (!element.IsVisible || !UiAnimations.CanAnimate(element))
        {
            element.IsVisible = false;
            UiAnimations.ResetVisualState(element);
            return;
        }

        await element.FadeToAsync(0, UiAnimations.ExitRevealDuration, UiAnimations.ExitEasing);
        element.IsVisible = false;
        UiAnimations.ResetVisualState(element);
    }

    public static Task CrossfadeSectionsAsync(
        VisualElement? hide,
        VisualElement? show,
        CancellationToken cancellationToken = default) =>
        UiAnimations.CrossfadeAsync(hide, show, UiAnimations.MediumDuration, allowHidden: true, cancellationToken: cancellationToken);
}
