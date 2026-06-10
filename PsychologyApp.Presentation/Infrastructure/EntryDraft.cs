namespace PsychologyApp.Presentation.Infrastructure;

public sealed class EntryDraft
{
    public Dictionary<string, string> Fields { get; set; } = new(StringComparer.Ordinal);
}
