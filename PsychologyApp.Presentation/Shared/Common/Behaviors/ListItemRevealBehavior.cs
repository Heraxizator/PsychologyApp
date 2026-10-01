using System.Runtime.CompilerServices;
using System.Collections;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.Common.Behaviors;

public sealed class ListItemRevealBehavior : Behavior<VisualElement>
{
    private static int _activeRevealCount;

    // Items of a CollectionView that have already played their entrance: a recycled row scrolled back into view
    // must not animate again. Weak, so it never keeps an item alive.
    private static readonly ConditionalWeakTable<object, object> RevealedCollectionItems = new();
    private static readonly object Revealed = new();
    private static readonly SemaphoreSlim RevealSlot = new(UiAnimations.MaxConcurrentListReveals, UiAnimations.MaxConcurrentListReveals);

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

    internal static int ResolveRevealTier(int index) =>
        index <= UiAnimations.PremiumRevealMaxIndex ? 0
        : index <= UiAnimations.LiteRevealMaxIndex ? 1
        : 2;

    private VisualElement? _attachedView;
    private bool _hasRevealed;
    private CancellationTokenSource? _revealCts;

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
        CancelReveal();
        _attachedView = null;
        _hasRevealed = false;
        _isInsideCollectionView = false;
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

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement view)
        {
            return;
        }

        CancelReveal();
        _hasRevealed = false;

        if (view.Handler is not null)
        {
            TryReveal(view);
        }
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (_hasRevealed || sender is not VisualElement view)
        {
            return;
        }

        TryReveal(view);
    }

    private void TryReveal(VisualElement view)
    {
        int index = ResolveRevealIndex(view);

        // Throttle CollectionView: only first LiteRevealMaxIndex+1 items animate (protect scroll).
        if (IsInsideCollectionView(view)
            && (index > UiAnimations.LiteRevealMaxIndex
                || (view.BindingContext is { } item && !RevealedCollectionItems.TryAdd(item, Revealed))))
        {
            _hasRevealed = true;
            UiAnimations.ResetVisualState(view);
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

        await RevealSlot.WaitAsync(token);
        Interlocked.Increment(ref _activeRevealCount);

        try
        {
            int index = ResolveRevealIndex(view);
            int delay = UiAnimations.ComputeRevealDelay(index);
            int tier = ResolveRevealTier(index);

            switch (tier)
            {
                case 0:
                    await UiAnimations.SafeRevealPremiumAsync(
                        view,
                        delayMs: delay,
                        allowHidden: true,
                        cancellationToken: token);
                    break;
                case 1:
                    await UiAnimations.SafeRevealLiteAsync(
                        view,
                        delayMs: delay,
                        allowHidden: true,
                        cancellationToken: token);
                    break;
                default:
                    if (delay > 0)
                    {
                        await Task.Delay(delay, token);
                    }

                    await UiAnimations.SafeFadeInAsync(
                        view,
                        duration: UiAnimations.ListRevealDuration,
                        allowHidden: true,
                        cancellationToken: token);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            UiAnimations.ResetVisualState(view);
        }
        finally
        {
            Interlocked.Decrement(ref _activeRevealCount);
            RevealSlot.Release();
        }
    }

    private int ResolveRevealIndex(VisualElement view)
    {
        if (RevealIndex >= 0)
        {
            return RevealIndex;
        }

        return FindIndexInParentCollection(view);
    }

    // Only a positive answer is cached: a recycled row may be bound before it is parented.
    private bool _isInsideCollectionView;

    private bool IsInsideCollectionView(VisualElement view) =>
        _isInsideCollectionView || (_isInsideCollectionView = HasCollectionViewAncestor(view));

    private static bool HasCollectionViewAncestor(VisualElement view)
    {
        Element? parent = view.Parent;
        while (parent is not null)
        {
            if (parent is CollectionView)
            {
                return true;
            }

            parent = parent.Parent;
        }

        return false;
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

    // Runs for every recycled row while a CollectionView scrolls. Only the first StaggerCap positions change the
    // animation (later ones share the last tier and delay), so the search stops there instead of walking the whole list.
    private static int FindIndexInEnumerable(IEnumerable items, object? bindingContext)
    {
        const int cap = UiAnimations.StaggerCap;
        if (items is IList list)
        {
            int count = Math.Min(list.Count, cap);
            for (int i = 0; i < count; i++)
            {
                if (ReferenceEquals(list[i], bindingContext))
                {
                    return i;
                }
            }

            return Math.Min(list.Count, cap - 1);
        }

        int index = 0;
        foreach (object? item in items)
        {
            if (ReferenceEquals(item, bindingContext) || index >= cap - 1)
            {
                return index;
            }

            index++;
        }

        return index;
    }
}
