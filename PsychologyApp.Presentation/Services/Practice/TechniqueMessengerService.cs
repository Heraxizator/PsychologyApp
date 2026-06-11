namespace PsychologyApp.Presentation.Services.Practice;

public sealed class TechniqueMessengerService : ITechniqueMessenger
{
    private readonly WeakReferenceManager<TechniqueMessage> _manager = new();

    public void Subscribe(object subscriber, Action<TechniqueMessage> handler) =>
        _manager.Subscribe(subscriber, handler);

    public void Unsubscribe(object subscriber) =>
        _manager.Unsubscribe(subscriber);

    public void Send(TechniqueMessage message) =>
        _manager.Send(message);
}
