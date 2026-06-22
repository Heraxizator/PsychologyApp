namespace PsychologyApp.Presentation.Models.Quotes;

public sealed class QuoteJsonEntry
{
    public string? Author { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Theme { get; set; }
}
