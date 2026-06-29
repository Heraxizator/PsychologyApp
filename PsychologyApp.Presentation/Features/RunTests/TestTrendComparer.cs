using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class TestTrendComparer
{
    public static TestTrendKind CompareScores(int? current, int? previous)
    {
        if (current is null || previous is null)
        {
            return TestTrendKind.None;
        }

        if (current < previous)
        {
            return TestTrendKind.Improved;
        }

        if (current > previous)
        {
            return TestTrendKind.Worse;
        }

        return TestTrendKind.Same;
    }

    public static string ToLabel(TestTrendKind kind) => kind switch
    {
        TestTrendKind.Improved => AppStrings.TestResultImproved,
        TestTrendKind.Worse => AppStrings.TestResultWorse,
        TestTrendKind.Same => AppStrings.TestResultSame,
        _ => string.Empty
    };
}
