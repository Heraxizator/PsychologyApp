namespace PsychologyApp.Presentation.Modules.Practice.Messages;

public interface ITechniqueMessenger
{
    void Subscribe(object subscriber, Action<TechniqueMessage> handler);
    void Unsubscribe(object subscriber);
    void Send(TechniqueMessage message);
}
