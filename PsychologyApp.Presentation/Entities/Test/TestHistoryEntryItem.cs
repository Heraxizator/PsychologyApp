namespace PsychologyApp.Presentation.Entities.Test;

public sealed class TestHistoryEntryItem
{
    public string DateText { get; init; } = string.Empty;
    public string SummaryText { get; init; } = string.Empty;
    public string ScoreText { get; init; } = string.Empty;
    public string TrendText { get; init; } = string.Empty;
    public TestTrendKind TrendKind { get; init; } = TestTrendKind.None;
    public bool HasScore => !string.IsNullOrWhiteSpace(ScoreText);
    public bool HasTrend => TrendKind is not TestTrendKind.None;
    public bool IsImproved => TrendKind is TestTrendKind.Improved;
    public bool IsWorse => TrendKind is TestTrendKind.Worse;
    public bool IsSame => TrendKind is TestTrendKind.Same;
}
