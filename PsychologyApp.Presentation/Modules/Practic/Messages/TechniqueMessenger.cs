using PsychologyApp.Presentation.Modules.Practic.Messages;

namespace PsychologyApp.Presentation.Modules.Practic.Messages;

public static class TechniqueMessenger
{
    private static readonly WeakReferenceManager<TechniqueMessage> _manager = new();

    public static void Subscribe(object subscriber, Action<TechniqueMessage> handler)
    {
        _manager.Subscribe(subscriber, handler);
    }

    public static void Unsubscribe(object subscriber)
    {
        _manager.Unsubscribe(subscriber);
    }

    public static void Send(TechniqueMessage message)
    {
        _manager.Send(message);
    }
}
