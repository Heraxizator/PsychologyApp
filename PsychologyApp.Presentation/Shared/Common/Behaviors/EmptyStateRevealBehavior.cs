using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.Common.Behaviors;

public sealed class EmptyStateRevealBehavior : Behavior<VisualElement>
{
    private VisualElement? _attachedView;
    private bool _hasRevealed;
    private CancellationTokenSource? _revealCts;

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
        _hasRevealed = false;
        base.OnDetachingFrom(bindable);
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

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_hasRevealed || sender is not VisualElement view)
        {
            return;
        }

        RevealAsync(view).FireAndForget();
    }

    private async Task RevealAsync(VisualElement view)
    {
        if (_hasRevealed || !UiAnimations.ShouldAnimate(view))
        {
            return;
        }

        _hasRevealed = true;
        CancelReveal();
        _revealCts = new CancellationTokenSource();
        CancellationToken token = _revealCts.Token;

        try
        {
            await UiAnimations.SafeRevealPremiumAsync(
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
