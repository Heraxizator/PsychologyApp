using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.Conversation;

/// <summary>The three pulsing dots of a messenger's "typing..." bubble. Animates only while <see cref="IsRunning"/> and honours reduced motion.</summary>
public partial class TypingBubbleView : ContentView
{
    private const uint StepMs = 220;
    private const double Dim = 0.35;

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
        _animation?.Cancel();
        _animation = null;
        foreach (VisualElement dot in new VisualElement[] { Dot1, Dot2, Dot3 })
        {
            dot.Opacity = Dim;
        }
    }

    private async Task AnimateAsync(CancellationToken cancellationToken)
    {
        VisualElement[] dots = [Dot1, Dot2, Dot3];
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (VisualElement dot in dots)
            {
                await dot.FadeTo(1, StepMs);
                await dot.FadeTo(Dim, StepMs);
            }
        }
    }
}
