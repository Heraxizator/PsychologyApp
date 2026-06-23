namespace PsychologyApp.Presentation.Shared.Common;

public static class UiAnimations
{
    public const uint MicroDuration = 260;
    public const uint MediumDuration = 320;
    public const uint FastDuration = 120;
    public const uint ExitRevealDuration = 200;
    public const uint ListRevealDuration = 200;
    public const uint InputFocusDuration = 150;
    public const uint StaggerDelay = 40;
    public const int StaggerCap = 10;
    public const int PremiumRevealMaxIndex = 2;
    public const int LiteRevealMaxIndex = 8;
    public const int MaxConcurrentListReveals = 12;
    public const uint PressDuration = 100;
    public const uint ReleaseDuration = 200;
    public const double PressScale = 0.97;
    public const double PressScalePrimary = 0.97;
    public const double PressScaleSecondary = 0.98;
    public const double PressOpacity = 0.94;
    public const double PressTranslationY = 1;
    public const double PulseScaleTo = 1.04;
    public const double ShakeOffset = 4;
    public const double FocusScale = 1.01;
    public const double SlideOffset = 11;
    public const double LiteSlideOffset = 7;
    public const double TabReappearSlideOffset = 4;
    public const double RevealScaleFrom = 0.97;
    public const double CrossfadeScaleFrom = 0.98;

    public static Easing StandardEasing { get; } = Easing.CubicOut;
    public static Easing EnterEasing { get; } = Easing.CubicOut;
    public static Easing EnterTransformEasing { get; } = Easing.SinOut;
    public static Easing EnterOpacityEasing { get; } = Easing.CubicOut;
    public static Easing ExitEasing { get; } = Easing.CubicIn;
    public static Easing ReleaseEasing { get; } = Easing.SpringOut;

    public static bool CanAnimate(VisualElement? view) =>
        view is not null && view.Handler is not null && view.Window is not null;

    public static bool ShouldAnimate(VisualElement? view) =>
        CanAnimate(view) && !ReduceMotion.IsEnabled;

    public static void PrepareForReveal(VisualElement? view)
    {
        if (view is null)
        {
            return;
        }

        view.Opacity = 0;
        view.TranslationY = 0;
        view.Scale = 1;
    }

    public static void PrepareForPremiumReveal(VisualElement? view, double y = SlideOffset)
    {
        if (view is null)
        {
            return;
        }

        view.Opacity = 0;
        view.Scale = RevealScaleFrom;
        view.TranslationY = y;
    }

    public static void ResetVisualState(VisualElement? view)
    {
        if (view is null)
        {
            return;
        }

        view.Opacity = 1;
        view.TranslationY = 0;
        view.Scale = 1;
    }

    public static async Task SafeRevealAsync(
        VisualElement? view,
        double y = SlideOffset,
        uint duration = MicroDuration,
        bool allowHidden = false,
        int delayMs = 0,
        CancellationToken cancellationToken = default) =>
        await SafeRevealPremiumAsync(view, y, duration, allowHidden, delayMs, cancellationToken);

    public static async Task SafeRevealPremiumAsync(
        VisualElement? view,
        double y = SlideOffset,
        uint duration = MicroDuration,
        bool allowHidden = false,
        int delayMs = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (delayMs > 0)
            {
                await Task.Delay(delayMs, cancellationToken);
            }

            await RevealPremiumAsync(view, y, duration, allowHidden, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafeRevealPremiumAsync skipped: {ex.GetType().Name}: {ex.Message}");
            ResetVisualState(view);
        }
    }

    public static async Task SafeFadeInAsync(
        VisualElement? view,
        double fromOpacity = 0,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await FadeInAsync(view, fromOpacity, duration, allowHidden, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafeFadeInAsync skipped: {ex.GetType().Name}: {ex.Message}");
            ResetVisualState(view);
        }
    }

    public static async Task SafeRevealLiteAsync(
        VisualElement? view,
        double y = LiteSlideOffset,
        uint duration = ListRevealDuration,
        bool allowHidden = false,
        int delayMs = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (delayMs > 0)
            {
                await Task.Delay(delayMs, cancellationToken);
            }

            await RevealLiteAsync(view, y, duration, allowHidden, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafeRevealLiteAsync skipped: {ex.GetType().Name}: {ex.Message}");
            ResetVisualState(view);
        }
    }

    public static async Task RevealLiteAsync(
        VisualElement? view,
        double y = LiteSlideOffset,
        uint duration = ListRevealDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        if (view is null || (!allowHidden && !view.IsVisible))
        {
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        double targetY = view.TranslationY;
        view.Opacity = 0;
        view.TranslationY = targetY + y;
        view.Scale = 1;
        await AnimateRevealAsync(
            view,
            targetY,
            targetY + y,
            includeScale: false,
            duration,
            cancellationToken);
        ResetVisualState(view);
    }

    public static async Task FadeInAsync(
        VisualElement? view,
        double fromOpacity = 0,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        if (view is null || (!allowHidden && !view.IsVisible))
        {
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        view.Opacity = fromOpacity;
        await view.FadeToAsync(1, duration, EnterOpacityEasing);
        ResetVisualState(view);
    }

    public static async Task SlideInAsync(
        VisualElement? view,
        double y = SlideOffset,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        if (view is null || (!allowHidden && !view.IsVisible))
        {
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        double targetY = view.TranslationY;
        view.TranslationY = targetY + y;
        view.Opacity = 0;
        await AnimateRevealAsync(
            view,
            targetY,
            targetY + y,
            includeScale: false,
            duration,
            cancellationToken);
        ResetVisualState(view);
    }

    public static Task RevealAsync(
        VisualElement? view,
        double y = SlideOffset,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default) =>
        RevealPremiumAsync(view, y, duration, allowHidden, cancellationToken);

    public static async Task RevealPremiumAsync(
        VisualElement? view,
        double y = SlideOffset,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        if (view is null || (!allowHidden && !view.IsVisible))
        {
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        double targetY = view.TranslationY;
        PrepareForPremiumReveal(view, targetY + y);
        await AnimateRevealAsync(
            view,
            targetY,
            targetY + y,
            includeScale: true,
            duration,
            cancellationToken);
        ResetVisualState(view);
    }

    public static Task AnimateRevealAsync(
        VisualElement view,
        double targetY,
        double startY,
        bool includeScale,
        uint duration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        view.AbortAnimation("Reveal");

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                view.AbortAnimation("Reveal");
                tcs.TrySetCanceled(cancellationToken);
            });
        }

        var opacityAnimation = new Animation(v => view.Opacity = v, 0, 1, EnterOpacityEasing);
        var translateAnimation = new Animation(v => view.TranslationY = v, startY, targetY, EnterTransformEasing);

        var parent = new Animation();
        parent.Add(0, 0.85, opacityAnimation);
        parent.Add(0, 1, translateAnimation);

        if (includeScale)
        {
            var scaleAnimation = new Animation(v => view.Scale = v, RevealScaleFrom, 1, EnterTransformEasing);
            parent.Add(0, 1, scaleAnimation);
        }

        parent.Commit(
            view,
            "Reveal",
            length: duration,
            easing: Easing.Linear,
            finished: (_, cancelled) => tcs.TrySetResult(!cancelled));

        return tcs.Task;
    }

    public static async Task PressScaleAsync(
        VisualElement? view,
        double scale = PressScale,
        uint duration = MicroDuration)
    {
        if (view is null || !ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        await view.ScaleToAsync(scale, duration / 2, EnterTransformEasing);
        await view.ScaleToAsync(1, duration / 2, ReleaseEasing);
        ResetVisualState(view);
    }

    public static async Task SafePulseAsync(
        VisualElement? view,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await PulseAsync(view, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafePulseAsync skipped: {ex.GetType().Name}: {ex.Message}");
            ResetVisualState(view);
        }
    }

    public static async Task PulseAsync(
        VisualElement? view,
        CancellationToken cancellationToken = default)
    {
        if (view is null || !view.IsVisible)
        {
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await view.ScaleToAsync(PulseScaleTo, FastDuration, EnterTransformEasing);
        await view.ScaleToAsync(1, FastDuration, ReleaseEasing);
        ResetVisualState(view);
    }

    public static async Task SafeShakeAsync(
        VisualElement? view,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await ShakeAsync(view, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafeShakeAsync skipped: {ex.GetType().Name}: {ex.Message}");
            ResetVisualState(view);
        }
    }

    public static async Task ShakeAsync(
        VisualElement? view,
        CancellationToken cancellationToken = default)
    {
        if (view is null || !view.IsVisible)
        {
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        double baseX = view.TranslationX;
        await view.TranslateToAsync(baseX - ShakeOffset, view.TranslationY, FastDuration / 2, EnterTransformEasing);
        await view.TranslateToAsync(baseX + ShakeOffset, view.TranslationY, FastDuration / 2, EnterTransformEasing);
        await view.TranslateToAsync(baseX, view.TranslationY, FastDuration / 2, ReleaseEasing);
        ResetVisualState(view);
    }

    public static async Task SafeFocusRingAsync(
        Border? border,
        bool focused,
        CancellationToken cancellationToken = default) =>
        await SafeInputFocusAsync(border, focused, cancellationToken);

    public static async Task FocusRingAsync(
        Border? border,
        bool focused,
        CancellationToken cancellationToken = default) =>
        await InputFocusAsync(border, focused, cancellationToken);

    public static async Task SafeInputFocusAsync(
        Border? border,
        bool focused,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await InputFocusAsync(border, focused, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafeInputFocusAsync skipped: {ex.GetType().Name}: {ex.Message}");
            if (border is not null)
            {
                ApplyInputFocusInstant(border, focused);
            }
        }
    }

    public static async Task InputFocusAsync(
        Border? border,
        bool focused,
        CancellationToken cancellationToken = default)
    {
        if (border is null)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (!ShouldAnimate(border))
        {
            ApplyInputFocusInstant(border, focused);
            return;
        }

        Color targetStroke = focused ? GetFocusColor("Primary") : GetInputBorderColor();
        Color targetBackground = focused ? GetInputFocusBackgroundColor() : GetInputBackgroundColor();
        Color startStroke = GetBorderStrokeColor(border);
        Color startBackground = border.BackgroundColor;

        border.StrokeThickness = 1;
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                border.AbortAnimation("InputFocus");
                tcs.TrySetCanceled(cancellationToken);
            });
        }

        var animation = new Animation(
            progress =>
            {
                border.Stroke = LerpColor(startStroke, targetStroke, progress);
                border.BackgroundColor = LerpColor(startBackground, targetBackground, progress);
            },
            0,
            1);

        animation.Commit(
            border,
            "InputFocus",
            length: InputFocusDuration,
            easing: EnterOpacityEasing,
            finished: (_, cancelled) => tcs.TrySetResult(!cancelled));

        await tcs.Task;
        ApplyInputFocusInstant(border, focused);
    }

    private static void ApplyInputFocusInstant(Border border, bool focused)
    {
        border.StrokeThickness = 1;
        border.Stroke = focused ? GetFocusColor("Primary") : GetInputBorderColor();
        border.BackgroundColor = focused ? GetInputFocusBackgroundColor() : GetInputBackgroundColor();
    }

    private static Color GetBorderStrokeColor(Border border) =>
        border.Stroke is SolidColorBrush brush ? brush.Color : GetInputBorderColor();

    private static Color LerpColor(Color from, Color to, double progress) =>
        Color.FromRgba(
            from.Red + (to.Red - from.Red) * progress,
            from.Green + (to.Green - from.Green) * progress,
            from.Blue + (to.Blue - from.Blue) * progress,
            from.Alpha + (to.Alpha - from.Alpha) * progress);

    private static Color GetInputBorderColor()
    {
        string key = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark
            ? "InputBorderDark"
            : "NeutralBorder";
        return GetFocusColor(key);
    }

    private static Color GetInputBackgroundColor()
    {
        string key = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark
            ? "InputBackgroundDark"
            : "InputBackgroundLight";
        return GetFocusColor(key);
    }

    private static Color GetInputFocusBackgroundColor()
    {
        string key = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark
            ? "InputFocusBackgroundDark"
            : "InputFocusBackgroundLight";
        return GetFocusColor(key);
    }

    private static Color GetFocusColor(string key) =>
        Microsoft.Maui.Controls.Application.Current?.Resources[key] is Color color
            ? color
            : Colors.Transparent;

    public static async Task SafeHideAsync(
        VisualElement? view,
        uint duration = ExitRevealDuration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await HideAsync(view, duration, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UiAnimations.SafeHideAsync skipped: {ex.GetType().Name}: {ex.Message}");
            ResetVisualState(view);
        }
    }

    public static async Task HideAsync(
        VisualElement? view,
        uint duration = ExitRevealDuration,
        CancellationToken cancellationToken = default)
    {
        if (view is null || !view.IsVisible)
        {
            ResetVisualState(view);
            return;
        }

        if (!ShouldAnimate(view))
        {
            ResetVisualState(view);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await view.FadeToAsync(0, duration, ExitEasing);
        ResetVisualState(view);
    }

    public static async Task CrossfadeAsync(
        VisualElement? hide,
        VisualElement? show,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (show is not null && !CanAnimate(show) && hide is not null && !CanAnimate(hide))
        {
            if (show is not null)
            {
                show.IsVisible = true;
                ResetVisualState(show);
            }

            if (hide is not null)
            {
                hide.IsVisible = false;
                ResetVisualState(hide);
            }

            return;
        }

        List<Task> tasks = [];
        if (hide is not null && hide.IsVisible && CanAnimate(hide))
        {
            tasks.Add(hide.FadeToAsync(0, duration, ExitEasing));
        }

        if (show is not null)
        {
            show.IsVisible = true;
            show.Opacity = 0;
            show.Scale = CrossfadeScaleFrom;
            if (CanAnimate(show))
            {
                tasks.Add(show.FadeToAsync(1, duration, EnterOpacityEasing));
                tasks.Add(show.ScaleToAsync(1, duration, EnterTransformEasing));
            }
            else
            {
                ResetVisualState(show);
            }
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }

        if (hide is not null)
        {
            hide.IsVisible = false;
            ResetVisualState(hide);
        }

        ResetVisualState(show);
    }

    public static async Task RevealChildrenStaggeredAsync(
        Layout? layout,
        uint delay = StaggerDelay,
        uint duration = MicroDuration,
        bool allowHidden = false,
        CancellationToken cancellationToken = default)
    {
        if (layout is null)
        {
            return;
        }

        List<Task> tasks = [];
        int index = 0;

        foreach (IView child in layout.Children)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (child is VisualElement element && (allowHidden || element.IsVisible))
            {
                int delayMs = ComputeRevealDelay(index);
                tasks.Add(SafeRevealPremiumAsync(
                    element,
                    SlideOffset,
                    duration,
                    allowHidden,
                    delayMs,
                    cancellationToken));
                index++;
            }
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    public static int ComputeRevealDelay(int index, int cap = StaggerCap)
    {
        int cappedIndex = Math.Max(0, Math.Min(index, cap - 1));
        return (int)(StaggerDelay * Math.Sqrt(cappedIndex));
    }
}
