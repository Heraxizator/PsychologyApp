using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation.Modules.Tests.Collection;

public static class TestScoreAnalyzers
{
    public static Func<int, string>? Resolve(string analyzerId) => analyzerId switch
    {
        "heck_hess" => AppStrings.HeckHessScore,
        "haer" => AppStrings.HaerScore,
        "pochebut" => AppStrings.PochebutScore,
        "beck" => AppStrings.BeckScore,
        _ => null
    };
}
