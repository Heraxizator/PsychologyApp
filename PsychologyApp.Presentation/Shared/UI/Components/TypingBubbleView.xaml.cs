using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>The three pulsing dots of a messenger's "typing..." bubble. Animates only while <see cref="IsRunning"/> and honours reduced motion.</summary>
public partial class TypingBubbleView : ContentView
{
    private const uint StepMs = 220;
    private const double Dim = 0.35;
    private const int AnimationPasses = 2;

    public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
        nameof(IsRunning),
        typeof(bool),
        typeof(TypingBubbleView),
        false,
        propertyChanged: (bindable, _, newValue) => ((TypingBubbleView)bindable).OnRunningChanged((bool)newValue));

    private CancellationTokenSource? _animation;

    public TypingBubbleView()
    {
        InitializeComponent();
        Unloaded += (_, _) => Stop();
    }

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    private void OnRunningChanged(bool running)
    {
        Stop();
        if (!running || ReduceMotion.IsEnabled)
        {
            return;
        }

        _animation = new CancellationTokenSource();
        AnimateAsync(_animation.Token).FireAndForget();
    }

    private void Stop()
    {
        CancellationTokenSource? animation = _animation;
        _animation = null;
        animation?.Cancel();
        animation?.Dispose();
        foreach (VisualElement dot in new VisualElement[] { Dot1, Dot2, Dot3 })
        {
            dot.CancelAnimations();
            dot.Opacity = Dim;
        }
    }

    private async Task AnimateAsync(CancellationToken cancellationToken)
    {
        VisualElement[] dots = [Dot1, Dot2, Dot3];
        for (int pass = 0; pass < AnimationPasses && !cancellationToken.IsCancellationRequested; pass++)
        {
            foreach (VisualElement dot in dots)
            {
                await dot.FadeToAsync(1, StepMs);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await dot.FadeToAsync(Dim, StepMs);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
