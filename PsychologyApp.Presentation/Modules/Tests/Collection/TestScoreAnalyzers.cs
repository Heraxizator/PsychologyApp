namespace PsychologyApp.Presentation.Modules.Tests.Collection;

public static class TestScoreAnalyzers
{
    public static Func<int, string>? Resolve(string analyzerId) => analyzerId switch
    {
        "heck_hess" => HeckHess,
        "haer" => Haer,
        "pochebut" => Pochebut,
        "beck" => Beck,
        _ => null
    };

    private static string Beck(int ball)
    {
        if (ball <= 9)
        {
            return "0-9 - нет депрессивных симптомов";
        }

        if (ball <= 15)
        {
            return "10-15 - лёгкая депрессия";
        }

        if (ball <= 19)
        {
            return "16-19 - умеренная депрессия";
        }

        return ball <= 29
            ? "20-29 - выраженная депрессия (средней тяжести)"
            : "30-63 – тяжелая депрессия";
    }

    private static string HeckHess(int ball) =>
        ball <= 24
            ? "0-24 - невысокий уровень невротизации"
            : "25-40 - высокий уровень невротизации";

    private static string Haer(int ball) =>
        ball <= 29
            ? "0-28 - невысокий уровень психопатии"
            : "29-40 - высокий уровень психопатии";

    private static string Pochebut(int ball)
    {
        if (ball <= 29)
        {
            return "0-10 - низкий уровень агрессивности";
        }

        if (ball <= 24)
        {
            return "11-24 - средний уровень агрессивности";
        }

        return "25-40 - высокий уровень агрессивности";
    }
}
