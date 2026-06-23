using System.Collections;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.Common.Behaviors;

public sealed class ListItemRevealBehavior : Behavior<VisualElement>
{
    private static int _activeRevealCount;
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
        if (sender is not VisualElement view || IsInsideCollectionView(view))
        {
            return;
        }

        CancelReveal();
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

    private static bool IsInsideCollectionView(VisualElement view)
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
