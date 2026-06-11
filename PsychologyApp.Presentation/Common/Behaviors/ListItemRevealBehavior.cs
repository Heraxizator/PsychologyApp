using System.Collections;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Common.Behaviors;

public sealed class ListItemRevealBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty RevealIndexProperty =
        BindableProperty.Create(
            nameof(RevealIndex),
            typeof(int),
            typeof(ListItemRevealBehavior),
            -1);

    public int RevealIndex
    {
        get => (int)GetValue(RevealIndexProperty);
        set => SetValue(RevealIndexProperty, value);
    }

    private VisualElement? _attachedView;
    private object? _lastBindingContext;
    private bool _hasRevealed;

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        _attachedView = bindable;
        bindable.Loaded += OnLoaded;
        bindable.BindingContextChanged += OnBindingContextChanged;
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.Loaded -= OnLoaded;
        bindable.BindingContextChanged -= OnBindingContextChanged;
        _attachedView = null;
        _lastBindingContext = null;
        _hasRevealed = false;
        base.OnDetachingFrom(bindable);
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement view)
        {
            return;
        }

        if (ReferenceEquals(view.BindingContext, _lastBindingContext))
        {
            return;
        }

        _lastBindingContext = view.BindingContext;
        _hasRevealed = false;

        if (view.Handler is not null)
        {
            RevealAsync(view).FireAndForget();
        }
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
        if (_hasRevealed)
        {
            return;
        }

        _hasRevealed = true;
        _lastBindingContext = view.BindingContext;
        int index = ResolveRevealIndex(view);
        int delay = UiAnimations.ComputeRevealDelay(index);
        await UiAnimations.SafeRevealPremiumAsync(view, delayMs: delay, allowHidden: true);
    }

    private int ResolveRevealIndex(VisualElement view)
    {
        if (RevealIndex >= 0)
        {
            return RevealIndex;
        }

        return FindIndexInParentCollection(view);
    }

    private static int FindIndexInParentCollection(VisualElement view)
    {
        Element? parent = view.Parent;
        while (parent is not null)
        {
            if (parent is CollectionView collectionView)
            {
                return FindIndexInItemsSource(collectionView.ItemsSource, view.BindingContext);
            }

            if (parent is Layout layoutWithItems)
            {
                object? itemsSource = BindableLayout.GetItemsSource(layoutWithItems);
                if (itemsSource is IEnumerable items)
                {
                    return FindIndexInEnumerable(items, view.BindingContext);
                }
            }

            if (parent is Layout layout)
            {
                int index = 0;
                foreach (IView child in layout.Children)
                {
                    if (ReferenceEquals(child, view))
                    {
                        return index;
                    }

                    index++;
                }
            }

            parent = parent.Parent;
        }

        return 0;
    }

    private static int FindIndexInItemsSource(object? itemsSource, object? bindingContext)
    {
        if (itemsSource is IEnumerable items)
        {
            return FindIndexInEnumerable(items, bindingContext);
        }

        return 0;
    }

    private static int FindIndexInEnumerable(IEnumerable items, object? bindingContext)
    {
        int index = 0;
        foreach (object? item in items)
        {
            if (ReferenceEquals(item, bindingContext))
            {
                return index;
            }

            index++;
        }

        return Math.Min(index, UiAnimations.StaggerCap - 1);
    }
}
