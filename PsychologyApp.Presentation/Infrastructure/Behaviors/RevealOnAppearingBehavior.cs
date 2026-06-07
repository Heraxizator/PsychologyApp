using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation.Infrastructure.Behaviors;

public sealed class RevealOnAppearingBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty RevealOnTabAppearOnlyProperty =
        BindableProperty.Create(
            nameof(RevealOnTabAppearOnly),
            typeof(bool),
            typeof(RevealOnAppearingBehavior),
            false);

    public bool RevealOnTabAppearOnly
    {
        get => (bool)GetValue(RevealOnTabAppearOnlyProperty);
        set => SetValue(RevealOnTabAppearOnlyProperty, value);
    }

    private VisualElement? _attachedView;
    private ContentPage? _parentPage;
    private bool _hasRevealedThisSession;

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        _attachedView = bindable;
        bindable.Loaded += OnLoaded;
        bindable.ParentChanged += OnParentChanged;
        AttachParentPage(bindable);
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.Loaded -= OnLoaded;
        bindable.ParentChanged -= OnParentChanged;
        DetachParentPage();
        _attachedView = null;
        _hasRevealedThisSession = false;
        base.OnDetachingFrom(bindable);
    }

    private void OnParentChanged(object? sender, EventArgs e)
    {
        if (sender is VisualElement element)
        {
            AttachParentPage(element);
        }
    }

    private void AttachParentPage(VisualElement bindable)
    {
        DetachParentPage();
        _parentPage = FindParentPage(bindable);
        if (_parentPage is not null)
        {
            _parentPage.Appearing += OnPageAppearing;
            _parentPage.Disappearing += OnPageDisappearing;
        }
    }

    private void DetachParentPage()
    {
        if (_parentPage is null)
        {
            return;
        }

        _parentPage.Appearing -= OnPageAppearing;
        _parentPage.Disappearing -= OnPageDisappearing;
        _parentPage = null;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (RevealOnTabAppearOnly || _attachedView is null)
        {
            return;
        }

        RevealInitialAsync().FireAndForget();
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        if (_attachedView is null)
        {
            return;
        }

        if (RevealOnTabAppearOnly)
        {
            if (!_hasRevealedThisSession)
            {
                _hasRevealedThisSession = true;
                UiAnimations.SafeRevealPremiumAsync(_attachedView, allowHidden: true).FireAndForget();
            }
            else
            {
                UiAnimations.SafeFadeInAsync(_attachedView, duration: UiAnimations.FastDuration).FireAndForget();
            }

            return;
        }

        if (!_hasRevealedThisSession)
        {
            RevealInitialAsync().FireAndForget();
        }
    }

    private void OnPageDisappearing(object? sender, EventArgs e)
    {
        if (RevealOnTabAppearOnly)
        {
            _hasRevealedThisSession = false;
        }
    }

    private async Task RevealInitialAsync()
    {
        if (_hasRevealedThisSession || _attachedView is null)
        {
            return;
        }

        _hasRevealedThisSession = true;
        await UiAnimations.SafeRevealPremiumAsync(_attachedView, y: 20, allowHidden: true);
    }

    private static ContentPage? FindParentPage(Element? element)
    {
        while (element is not null)
        {
            if (element is ContentPage page)
            {
                return page;
            }

            element = element.Parent;
        }

        return null;
    }
}
