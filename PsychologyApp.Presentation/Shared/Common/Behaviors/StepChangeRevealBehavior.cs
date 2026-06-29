using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Shared.Common.Behaviors;

public sealed class StepChangeRevealBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(
            nameof(Step),
            typeof(int),
            typeof(StepChangeRevealBehavior),
            0,
            propertyChanged: OnStepChanged);

    private VisualElement? _attachedView;
    private int _lastStep = -1;
    private CancellationTokenSource? _revealCts;

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        _attachedView = bindable;
        bindable.Loaded += OnLoaded;
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.Loaded -= OnLoaded;
        CancelReveal();
        _attachedView = null;
        _lastStep = -1;
        base.OnDetachingFrom(bindable);
    }

    private static void OnStepChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StepChangeRevealBehavior behavior)
        {
            behavior.OnStepUpdated();
        }
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_lastStep < 0 && _attachedView is not null)
        {
            _lastStep = Step;
            RevealAsync(_attachedView, isInitial: true).FireAndForget();
        }
    }

    private void OnStepUpdated()
    {
        if (_attachedView is null || _lastStep == Step)
        {
            return;
        }

        _lastStep = Step;
        RevealAsync(_attachedView, isInitial: false).FireAndForget();
    }

    private void CancelReveal()
    {
        if (_revealCts is null)
        {
            return;
        }

        _revealCts.Cancel();
        _revealCts.Dispose();
        _revealCts = null;
    }

    private async Task RevealAsync(VisualElement view, bool isInitial)
    {
        if (!UiAnimations.ShouldAnimate(view))
        {
            return;
        }

        CancelReveal();
        _revealCts = new CancellationTokenSource();
        CancellationToken token = _revealCts.Token;

        try
        {
            UiAnimations.ResetVisualState(view);

            if (isInitial)
            {
                await UiAnimations.SafeRevealPremiumAsync(
                    view,
                    y: UiAnimations.SlideOffset,
                    allowHidden: true,
                    cancellationToken: token);
                return;
            }

            await UiAnimations.SafeRevealLiteAsync(
                view,
                y: UiAnimations.SlideOffset / 2,
                allowHidden: true,
                cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            UiAnimations.ResetVisualState(view);
        }
    }
}
