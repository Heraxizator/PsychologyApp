namespace PsychologyApp.Presentation.Common;

public sealed class EntryDraft
{
    public Dictionary<string, string> Fields { get; set; } = new(StringComparer.Ordinal);
}
