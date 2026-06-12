namespace PsychologyApp.Presentation.Models.Tests;

public sealed class TestDefinition
{
    public required string TestId { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string Description { get; init; }
    public required string Comment { get; init; }
    public required IReadOnlyList<string> Algorithm { get; init; }
    public required TestKind Kind { get; init; }
    public string? AnalyzerId { get; init; }
    public IReadOnlyList<Question>? Questions { get; init; }
    public bool SingleAnswer { get; init; }
    public LuscherMode? LuscherMode { get; init; }
    public int? EstimatedMinutes { get; init; }
    public int? QuestionCount { get; init; }
    public string? Construct { get; init; }
}
