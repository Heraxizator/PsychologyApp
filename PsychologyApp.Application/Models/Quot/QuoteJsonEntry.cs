namespace PsychologyApp.Application.Models.Quot;

public sealed class QuoteJsonEntry
{
    public string? Author { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Theme { get; set; }
}
