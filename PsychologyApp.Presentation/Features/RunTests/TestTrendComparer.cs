using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class TestTrendComparer
{
    public static TestTrendKind CompareScores(int? current, int? previous) =>
        CompareScores(current, previous, ScoreDirection.LowerIsBetter);

    public static TestTrendKind CompareScores(int? current, int? previous, ScoreDirection direction) =>
        TestTrendEvaluator.CompareScores(current, previous, direction);

    public static string ToLabel(TestTrendKind kind) => kind switch
    {
        TestTrendKind.Improved => AppStrings.TestResultImproved,
        TestTrendKind.Worse => AppStrings.TestResultWorse,
        TestTrendKind.Same => AppStrings.TestResultSame,
        _ => string.Empty
    };
}
