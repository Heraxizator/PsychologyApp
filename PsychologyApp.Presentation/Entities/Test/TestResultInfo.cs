using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Entities.Test;

public sealed class TestResultInfo
{
    public int Score { get; init; }
    public string Interpretation { get; init; } = string.Empty;
    public string? InterpretationDetail { get; init; }
    public string? AnalyzerId { get; init; }
    public TechniqueId? RecommendedTechnique { get; init; }
    public string? TestId { get; init; }
}
