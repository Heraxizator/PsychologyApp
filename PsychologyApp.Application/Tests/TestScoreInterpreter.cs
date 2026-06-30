namespace PsychologyApp.Application.Tests;

public static class TestScoreInterpreter
{
    public static bool IsKnownAnalyzer(string analyzerId) => analyzerId switch
    {
        "heck_hess" or "haer" or "pochebut" or "beck" or "gad7" or "k10" or "who5"
            or "phq9" or "isi" or "ess" or "phq15" or "scoff" or "swls" => true,
        _ => false
    };

    public static int GetBandIndex(string? analyzerId, int score) => analyzerId switch
    {
        "beck" => score switch
        {
            <= 9 => 0,
            <= 15 => 1,
            <= 19 => 2,
            <= 29 => 3,
            _ => 4
        },
        "heck_hess" => score <= 24 ? 0 : 1,
        "haer" => score <= 29 ? 0 : 1,
        "pochebut" => score switch
        {
            <= 10 => 0,
            <= 24 => 1,
            _ => 2
        },
        "gad7" => score switch
        {
            <= 4 => 0,
            <= 9 => 1,
            <= 14 => 2,
            _ => 3
        },
        "k10" => score switch
        {
            <= 15 => 0,
            <= 21 => 1,
            <= 29 => 2,
            _ => 3
        },
        "who5" => score switch
        {
            <= 12 => 0,
            <= 18 => 1,
            _ => 2
        },
        "phq9" => score switch
        {
            <= 4 => 0,
            <= 9 => 1,
            <= 14 => 2,
            <= 19 => 3,
            _ => 4
        },
        "isi" => score switch
        {
            <= 7 => 0,
            <= 14 => 1,
            <= 21 => 2,
            _ => 3
        },
        "ess" => score switch
        {
            <= 10 => 0,
            <= 12 => 1,
            <= 15 => 2,
            _ => 3
        },
        "phq15" => score switch
        {
            <= 4 => 0,
            <= 9 => 1,
            <= 14 => 2,
            _ => 3
        },
        "scoff" => score <= 1 ? 0 : 1,
        "swls" => score switch
        {
            <= 20 => 0,
            <= 25 => 1,
            _ => 2
        },
        _ => -1
    };
}
