using System.Reflection;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Common;

/// <summary>
/// An <see cref="Action"/> event that does not keep its subscribers alive. UserPreferences.Changed is static, and
/// view models, technique pages and every quote card subscribed to it without ever unsubscribing, so each one stayed
/// in memory for the life of the process. Static handlers and compiler-generated closures (which nothing else would
/// keep alive) are still held strongly.
/// </summary>
public sealed class WeakActionEvent
{
    private readonly Lock _sync = new();
    private readonly List<Subscription> _subscriptions = [];

    public void Add(Action handler)
    {
        lock (_sync)
        {
            foreach (Delegate single in handler.GetInvocationList())
            {
                _subscriptions.Add(Subscription.For((Action)single));
            }
        }
    }

    public void Remove(Action handler)
    {
        lock (_sync)
        {
            foreach (Delegate single in handler.GetInvocationList())
            {
                int index = _subscriptions.FindLastIndex(s => s.Matches(single));
                if (index >= 0)
                {
                    _subscriptions.RemoveAt(index);
                }
            }
        }
    }

    public void Raise()
    {
        List<Action> handlers = [];
        lock (_sync)
        {
            for (int i = _subscriptions.Count - 1; i >= 0; i--)
            {
                if (_subscriptions[i].TryGet(out Action? handler))
                {
                    handlers.Add(handler!);
                }
                else
                {
                    _subscriptions.RemoveAt(i);
                }
            }
        }

        // Collected newest-first above; invoke in subscription order, outside the lock.
        for (int i = handlers.Count - 1; i >= 0; i--)
        {
            handlers[i]();
        }
    }

    private sealed class Subscription
    {
        private readonly Action? _strong;
        private readonly WeakReference<object>? _target;
        private readonly MethodInfo _method;

        private Subscription(Action? strong, WeakReference<object>? target, MethodInfo method)
        {
            _strong = strong;
            _target = target;
            _method = method;
        }

        public static Subscription For(Action handler) =>
            handler.Target is { } target && !target.GetType().IsDefined(typeof(CompilerGeneratedAttribute), inherit: false)
                ? new Subscription(null, new WeakReference<object>(target), handler.Method)
                : new Subscription(handler, null, handler.Method);

        public bool Matches(Delegate handler) =>
            _method == handler.Method
            && (_strong is not null
                ? Equals(_strong.Target, handler.Target)
                : _target!.TryGetTarget(out object? target) && ReferenceEquals(target, handler.Target));

        public bool TryGet(out Action? handler)
        {
            if (_strong is not null)
            {
                handler = _strong;
                return true;
            }

            if (_target!.TryGetTarget(out object? target))
            {
                handler = (Action)_method.CreateDelegate(typeof(Action), target);
                return true;
            }

            handler = null;
            return false;
        }
    }
}
