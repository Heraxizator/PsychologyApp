using System.Collections.Concurrent;
using System.Runtime;

namespace PsychologyApp.Presentation.Modules.Practic.Messages;

public class WeakReferenceManager<TMessage>
{
    private readonly ConcurrentDictionary<object, List<WeakReference<Action<TMessage>>>> _subscribers = new();

    public void Subscribe(object subscriber, Action<TMessage> handler)
    {
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var weakHandler = new WeakReference<Action<TMessage>>(handler);
        
        _subscribers.AddOrUpdate(
            subscriber,
            new List<WeakReference<Action<TMessage>>> { weakHandler },
            (key, existingList) =>
            {
                lock (existingList)
                {
                    existingList.Add(weakHandler);
                }
                return existingList;
            });
    }

    public void Unsubscribe(object subscriber)
    {
        if (subscriber == null)
            return;

        _subscribers.TryRemove(subscriber, out _);
    }

    public void Send(TMessage message)
    {
        if (message == null)
            return;

        var keysToRemove = new List<object>();

        foreach (var kvp in _subscribers)
        {
            var subscriber = kvp.Key;
            var handlers = kvp.Value;
            var handlersToRemove = new List<WeakReference<Action<TMessage>>>();

            lock (handlers)
            {
                foreach (var weakHandler in handlers)
                {
                    if (weakHandler.TryGetTarget(out var handler))
                    {
                        try
                        {
                            handler(message);
                        }
                        catch
                        {
                            // Игнорируем ошибки в обработчиках, чтобы не прерывать отправку другим подписчикам
                        }
                    }
                    else
                    {
                        handlersToRemove.Add(weakHandler);
                    }
                }

                foreach (var handlerToRemove in handlersToRemove)
                {
                    handlers.Remove(handlerToRemove);
                }

                if (handlers.Count == 0)
                {
                    keysToRemove.Add(subscriber);
                }
            }
        }

        foreach (var keyToRemove in keysToRemove)
        {
            _subscribers.TryRemove(keyToRemove, out _);
        }
    }
}
