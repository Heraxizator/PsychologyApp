using Microsoft.Maui.Devices;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Infrastructure;

public static class VisualElementPressFeedback
{
    private static readonly HashSet<VisualElement> AnimatingViews = [];
    private static readonly HashSet<VisualElement> AttachedViews = [];
    private static readonly ConditionalWeakTable<VisualElement, PressFeedbackOptions> Options = new();

    public static void Attach(VisualElement? view, PressFeedbackOptions? options = null)
    {
        if (view is not View target)
        {
            return;
        }

        if (options is not null)
        {
            Options.AddOrUpdate(target, options);
        }

        if (!AttachedViews.Add(target))
        {
            return;
        }

        PointerGestureRecognizer pointer = new();
        pointer.PointerPressed += async (_, _) => await AnimatePressAsync(target);
        pointer.PointerReleased += async (_, _) => await AnimateReleaseAsync(target);
        pointer.PointerExited += async (_, _) => await AnimateReleaseAsync(target);

        target.GestureRecognizers.Add(pointer);
    }

    public static void AttachToTemplateRoot(ContentView contentView, PressFeedbackOptions? options = null) =>
        TemplatePressFeedback.Attach(contentView, options);

    private static PressFeedbackOptions GetOptions(VisualElement target) =>
        Options.TryGetValue(target, out PressFeedbackOptions? options)
            ? options
            : new PressFeedbackOptions();

    private static async Task AnimatePressAsync(VisualElement target)
    {
        if (!UiAnimations.ShouldAnimate(target) || !AnimatingViews.Add(target))
        {
            return;
        }

        PressFeedbackOptions opts = GetOptions(target);
        double scale = opts.PressScale();

        try
        {
            await Task.WhenAll(
                target.ScaleToAsync(scale, UiAnimations.PressDuration, UiAnimations.EnterEasing),
                target.FadeToAsync(UiAnimations.PressOpacity, UiAnimations.PressDuration, UiAnimations.EnterEasing),
                target.TranslateToAsync(0, UiAnimations.PressTranslationY, UiAnimations.PressDuration, UiAnimations.EnterEasing));
        }
        finally
        {
            AnimatingViews.Remove(target);
        }
    }

    private static async Task AnimateReleaseAsync(VisualElement target)
    {
        PressFeedbackOptions opts = GetOptions(target);

        if (!UiAnimations.ShouldAnimate(target) || !AnimatingViews.Add(target))
        {
            ResetInstant(target);
            if (opts.HapticOnRelease)
            {
                TryPerformHaptic();
            }

            return;
        }

        try
        {
            await Task.WhenAll(
                target.ScaleToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing),
                target.FadeToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing),
                target.TranslateToAsync(0, 0, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing));
        }
        finally
        {
            AnimatingViews.Remove(target);
            UiAnimations.ResetVisualState(target);

            if (opts.HapticOnRelease)
            {
                TryPerformHaptic();
            }
        }
    }

    private static void TryPerformHaptic()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Haptic feedback skipped: {ex.GetType().Name}: {ex.Message}");
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
