using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Infrastructure.Behaviors;

public sealed class StaggerChildrenBehavior : Behavior<Layout>
{
    private Layout? _layout;
    private bool _hasRevealed;

    protected override void OnAttachedTo(Layout bindable)
    {
        base.OnAttachedTo(bindable);
        _layout = bindable;
        bindable.Loaded += OnLoaded;
    }

    protected override void OnDetachingFrom(Layout bindable)
    {
        bindable.Loaded -= OnLoaded;
        _layout = null;
        _hasRevealed = false;
        base.OnDetachingFrom(bindable);
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_hasRevealed || _layout is null)
        {
            return;
        }

        _hasRevealed = true;
        UiAnimations.RevealChildrenStaggeredAsync(_layout, allowHidden: true).FireAndForget();
    }
}
