using PsychologyApp.Application.Tests;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class TestScoreChartRangeResolver
{
    public static (int DomainMin, int DomainMax) ResolveDomain(string? analyzerId, IReadOnlyList<int> values)
    {
        ScoreRange? domain = TestScoreInterpreter.GetScoreRange(analyzerId);
        if (domain is ScoreRange range)
        {
            return (range.Min, range.Max);
        }

        return ResolveFallbackDomain(values);
    }

    private static (int Min, int Max) ResolveFallbackDomain(IReadOnlyList<int> values)
    {
        if (values.Count == 0)
        {
            return (0, 4);
        }

        int dataMin = values.Min();
        int dataMax = values.Max();
        int pad = Math.Max(1, (dataMax - dataMin) / 4);
        return (Math.Max(0, dataMin - pad), dataMax + pad);
    }
}
