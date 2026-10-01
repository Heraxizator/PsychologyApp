using System.Runtime.CompilerServices;
using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class WeakActionEventTests
{
    [Fact]
    public void Live_subscribers_are_called_in_order_and_can_unsubscribe()
    {
        WeakActionEvent changed = new();
        Counter first = new();
        Counter second = new();
        changed.Add(first.Increment);
        changed.Add(second.Increment);

        changed.Raise();
        changed.Remove(first.Increment);
        changed.Raise();

        Assert.Equal(1, first.Count);
        Assert.Equal(2, second.Count);
    }

    [Fact]
    public void A_subscriber_nothing_else_references_is_not_kept_alive()
    {
        WeakActionEvent changed = new();
        WeakReference subscriber = SubscribeAndForget(changed);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(subscriber.IsAlive);
        changed.Raise();
    }

    [Fact]
    public void A_closure_handler_is_kept_so_it_is_not_silently_lost()
    {
        WeakActionEvent changed = new();
        int calls = 0;
        SubscribeClosure(changed, () => calls++);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        changed.Raise();

        Assert.Equal(1, calls);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference SubscribeAndForget(WeakActionEvent changed)
    {
        Counter counter = new();
        changed.Add(counter.Increment);
        return new WeakReference(counter);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void SubscribeClosure(WeakActionEvent changed, Action onCall) =>
        changed.Add(() => onCall());

    private sealed class Counter
    {
        public int Count { get; private set; }

        public void Increment() => Count++;
    }
}
