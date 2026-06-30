namespace PsychologyApp.Domain.Tests;

public static class TestTrendEvaluator
{
    public static TestTrendKind CompareScores(int? current, int? previous) =>
        CompareScores(current, previous, ScoreDirection.LowerIsBetter);

    public static TestTrendKind CompareScores(int? current, int? previous, ScoreDirection direction)
    {
        if (current is null || previous is null)
        {
            return TestTrendKind.None;
        }

        if (direction == ScoreDirection.None)
        {
            return TestTrendKind.None;
        }

        int cmp = current.Value.CompareTo(previous.Value);

        if (cmp == 0)
        {
            return TestTrendKind.Same;
        }

        bool improved = direction switch
        {
            ScoreDirection.LowerIsBetter => cmp < 0,
            ScoreDirection.HigherIsBetter => cmp > 0,
            _ => false
        };

        return improved ? TestTrendKind.Improved : TestTrendKind.Worse;
    }
}
