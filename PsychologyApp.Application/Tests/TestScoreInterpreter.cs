namespace PsychologyApp.Application.Tests;

public readonly record struct ScoreRange(int Min, int Max);

public static class TestScoreInterpreter
{
    public static ScoreRange? GetScoreRange(string? analyzerId) => analyzerId switch
    {
        "beck" => new ScoreRange(0, 63),
        "heck_hess" => new ScoreRange(0, 56),
        "haer" => new ScoreRange(0, 60),
        "pochebut" => new ScoreRange(0, 40),
        "gad7" => new ScoreRange(0, 21),
        "k10" => new ScoreRange(0, 50),
        "who5" => new ScoreRange(0, 25),
        "phq9" => new ScoreRange(0, 27),
        "isi" => new ScoreRange(0, 28),
        "ess" => new ScoreRange(0, 24),
        "phq15" => new ScoreRange(0, 30),
        "scoff" => new ScoreRange(0, 5),
        "swls" => new ScoreRange(5, 35),
        "pss10" => new ScoreRange(0, 40),
        "phq2" => new ScoreRange(0, 6),
        "gad2" => new ScoreRange(0, 6),
        "hads_a" => new ScoreRange(0, 21),
        "hads_d" => new ScoreRange(0, 21),
        "rses" => new ScoreRange(0, 30),
        _ => null
    };
    public static bool IsKnownAnalyzer(string analyzerId) => analyzerId switch
    {
        "heck_hess" or "haer" or "pochebut" or "beck" or "gad7" or "k10" or "who5"
            or "phq9" or "isi" or "ess" or "phq15" or "scoff" or "swls"
            or "pss10" or "phq2" or "gad2" or "hads_a" or "hads_d" or "rses" => true,
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
        "pss10" => score switch
        {
            <= 13 => 0,
            <= 26 => 1,
            _ => 2
        },
        "phq2" => score <= 2 ? 0 : 1,
        "gad2" => score <= 2 ? 0 : 1,
        "hads_a" => score switch
        {
            <= 7 => 0,
            <= 10 => 1,
            _ => 2
        },
        "hads_d" => score switch
        {
            <= 7 => 0,
            <= 10 => 1,
            _ => 2
        },
        "rses" => score switch
        {
            <= 14 => 0,
            <= 25 => 1,
            _ => 2
        },
        _ => -1
    };
}
