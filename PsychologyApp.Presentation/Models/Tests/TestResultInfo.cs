using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Models.Tests;

public sealed class TestResultInfo
{
    public int Score { get; init; }
    public string Interpretation { get; init; } = string.Empty;
    public TechniqueId? RecommendedTechnique { get; init; }
}
