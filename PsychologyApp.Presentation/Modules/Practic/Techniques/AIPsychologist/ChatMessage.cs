namespace PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

public class ChatMessage
{
    public string Text { get; set; } = string.Empty;
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
