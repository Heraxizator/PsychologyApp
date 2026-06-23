using Microsoft.Maui.Devices;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Shared.Common;

public static class VisualElementPressFeedback
{
    private static readonly ConditionalWeakTable<View, AttachmentState> Attachments = new();
    private sealed class AnimationLock;
    private static readonly ConditionalWeakTable<VisualElement, AnimationLock> AnimatingViews = new();
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

        if (Attachments.TryGetValue(target, out _))
        {
            return;
        }

        Attachments.Add(target, new AttachmentState(target));
    }

    public static void Detach(VisualElement? view)
    {
        if (view is not View target)
        {
            return;
        }

        if (Attachments.TryGetValue(target, out AttachmentState? state))
        {
            state.Detach(target);
            Attachments.Remove(target);
        }

        Options.Remove(target);
        AnimatingViews.Remove(target);
    }

    public static bool IsAttached(VisualElement? view) =>
        view is View target && Attachments.TryGetValue(target, out _);

    public static void AttachToTemplateRoot(ContentView contentView, PressFeedbackOptions? options = null) =>
        TemplatePressFeedback.Attach(contentView, options);

    private static PressFeedbackOptions GetOptions(VisualElement target) =>
        Options.TryGetValue(target, out PressFeedbackOptions? options)
            ? options
            : new PressFeedbackOptions();

    private static bool TryBeginAnimation(VisualElement target) =>
        !AnimatingViews.TryGetValue(target, out _);

    private static void EndAnimation(VisualElement target) =>
        AnimatingViews.Remove(target);

    private static async Task WaitForAnimationSlotAsync(VisualElement target)
    {
        for (int attempt = 0; attempt < 25 && AnimatingViews.TryGetValue(target, out _); attempt++)
        {
            await Task.Delay(10);
        }
    }

    private static async Task AnimatePressAsync(VisualElement target)
    {
        if (!UiAnimations.ShouldAnimate(target) || !TryBeginAnimation(target))
        {
            return;
        }

        AnimatingViews.Add(target, new AnimationLock());
        PressFeedbackOptions opts = GetOptions(target);
        double scale = opts.PressScale();

        try
        {
            if (opts.ScaleOnly)
            {
                await target.ScaleToAsync(scale, UiAnimations.PressDuration, UiAnimations.EnterTransformEasing);
            }
            else
            {
                await Task.WhenAll(
                    target.ScaleToAsync(scale, UiAnimations.PressDuration, UiAnimations.EnterTransformEasing),
                    target.FadeToAsync(UiAnimations.PressOpacity, UiAnimations.PressDuration, UiAnimations.EnterOpacityEasing),
                    target.TranslateToAsync(0, UiAnimations.PressTranslationY, UiAnimations.PressDuration, UiAnimations.EnterTransformEasing));
            }
        }
        finally
        {
            EndAnimation(target);
        }
    }

    private static async Task AnimateReleaseAsync(VisualElement target)
    {
        PressFeedbackOptions opts = GetOptions(target);

        if (!UiAnimations.ShouldAnimate(target))
        {
            ResetInstant(target);
            if (opts.HapticOnRelease)
            {
                TryPerformHaptic();
            }

            return;
        }

        await WaitForAnimationSlotAsync(target);

        if (!TryBeginAnimation(target))
        {
            ResetInstant(target);
            if (opts.HapticOnRelease)
            {
                TryPerformHaptic();
            }

            return;
        }

        AnimatingViews.Add(target, new AnimationLock());

        try
        {
            if (opts.ScaleOnly)
            {
                await target.ScaleToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing);
            }
            else
            {
                await Task.WhenAll(
                    target.ScaleToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing),
                    target.FadeToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing),
                    target.TranslateToAsync(0, 0, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing));
            }
        }
        finally
        {
            EndAnimation(target);
            UiAnimations.ResetVisualState(target);

            if (opts.HapticOnRelease)
            {
                TryPerformHaptic();
            }
        }
    }

    private static async Task PlayTapFeedbackAsync(VisualElement target)
    {
        await AnimatePressAsync(target);
        await AnimateReleaseAsync(target);
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

    private sealed class AttachmentState
    {
        private readonly View _target;
        private readonly PointerGestureRecognizer _pointer;
        private readonly List<TapGestureRecognizer> _subscribedTaps = [];
        private bool _pointerGestureActive;

        public AttachmentState(View target)
        {
            _target = target;
            _pointer = new PointerGestureRecognizer();
            _pointer.PointerPressed += OnPointerPressed;
            _pointer.PointerReleased += OnPointerReleased;
            _pointer.PointerExited += OnPointerExited;
            target.GestureRecognizers.Add(_pointer);

            foreach (TapGestureRecognizer tap in target.GestureRecognizers.OfType<TapGestureRecognizer>())
            {
                tap.Tapped += OnTapped;
                _subscribedTaps.Add(tap);
            }
        }

        private void OnPointerPressed(object? sender, PointerEventArgs e)
        {
            if (sender is not View target)
            {
                return;
            }

            _pointerGestureActive = true;
            AnimatePressAsync(target).FireAndForget();
        }

        private void OnPointerReleased(object? sender, PointerEventArgs e)
        {
            if (sender is not View target)
            {
                return;
            }

            _pointerGestureActive = false;
            AnimateReleaseAsync(target).FireAndForget();
        }

        private void OnPointerExited(object? sender, PointerEventArgs e)
        {
            if (sender is not View target)
            {
                return;
            }

            _pointerGestureActive = false;
            AnimateReleaseAsync(target).FireAndForget();
        }

        private void OnTapped(object? sender, TappedEventArgs e)
        {
            if (_pointerGestureActive)
            {
                return;
            }

            PlayTapFeedbackAsync(_target).FireAndForget();
        }

        public void Detach(View target)
        {
            _pointer.PointerPressed -= OnPointerPressed;
            _pointer.PointerReleased -= OnPointerReleased;
            _pointer.PointerExited -= OnPointerExited;
            target.GestureRecognizers.Remove(_pointer);

            foreach (TapGestureRecognizer tap in _subscribedTaps)
            {
                tap.Tapped -= OnTapped;
            }

            _subscribedTaps.Clear();
        }
    }
}
