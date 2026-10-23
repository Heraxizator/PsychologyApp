using System.Text.Json;
using PsychologyApp.Application.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>
/// The score range draws the axis of a test's history chart. It must be the range the published questions can actually
/// produce: "56" for a test of forty yes/no questions left every result in the lower part of the chart.
/// </summary>
public sealed class QuestionnaireScoreRangeTests
{
    internal static string RawRoot()
    {
        string? directory = AppContext.BaseDirectory;
        while (directory is not null)
        {
            string candidate = Path.Combine(directory, "PsychologyApp.Presentation", "Resources", "Raw", "tests");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Path.GetDirectoryName(directory);
        }

        throw new DirectoryNotFoundException("The questionnaire assets were not found above " + AppContext.BaseDirectory);
    }

    private static JsonElement Load(string file)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(RawRoot(), file)));
        return doc.RootElement.Clone();
    }

    /// <summary>The lowest and highest sum the questions allow, whichever of the two file layouts the test uses.</summary>
    private static (string AnalyzerId, int Min, int Max) Reachable(JsonElement test)
    {
        string analyzerId = test.GetProperty("analyzerId").GetString()!;
        JsonElement questions = test.GetProperty("questions");
        if (test.TryGetProperty("balls", out JsonElement balls))
        {
            int[] values = [.. balls.EnumerateArray().Select(ball => ball.GetInt32())];
            int count = questions.GetArrayLength();
            return (analyzerId, count * values.Min(), count * values.Max());
        }

        int min = 0;
        int max = 0;
        foreach (JsonElement question in questions.EnumerateArray())
        {
            int[] values = [.. question.GetProperty("answers").EnumerateArray().Select(answer => answer.GetProperty("ball").GetInt32())];
            min += values.Min();
            max += values.Max();
        }

        return (analyzerId, min, max);
    }

    /// <summary>Each question's points, answer by answer, so a reverse-scored item shows up as a descending row.</summary>
    private static IEnumerable<(string AnalyzerId, string Key)> Keys(JsonElement root)
    {
        IEnumerable<JsonElement> tests = root.ValueKind == JsonValueKind.Array ? root.EnumerateArray() : [root];
        foreach (JsonElement test in tests)
        {
            string analyzerId = test.GetProperty("analyzerId").GetString()!;
            string key = test.TryGetProperty("balls", out JsonElement balls)
                ? string.Join(",", balls.EnumerateArray().Select(ball => ball.GetInt32()))
                : string.Join(" / ", test.GetProperty("questions").EnumerateArray().Select(question =>
                    string.Join(",", question.GetProperty("answers").EnumerateArray().Select(answer => answer.GetProperty("ball").GetInt32()))));
            yield return (analyzerId, key);
        }
    }

    private static readonly string[] Scored = ["questionnaires", "beck", "pss10", "rses"];

    // Russian is the original (no suffix); the other languages are compared with the English file they were translated from.
    private static readonly string[] Translations = ["", ".fr", ".de", ".es", ".pt", ".zh"];

    public static TheoryData<string, string> TranslatedFiles
    {
        get
        {
            TheoryData<string, string> data = [];
            foreach (string name in Scored)
            {
                foreach (string suffix in Translations)
                {
                    data.Add(name, name + suffix + ".json");
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(TranslatedFiles))]
    public void A_test_is_scored_the_same_in_every_language(string name, string file)
    {
        // The Russian Rosenberg item 2 had drifted into "not much to be proud of", reverse-scored, while the English one
        // kept "a number of good qualities": the same answers gave a different self-esteem score in each language.
        Assert.Equal(Keys(Load(name + ".en.json")), Keys(Load(file)));
    }

    public static TheoryData<string> Files =>
        [.. Scored.SelectMany(name => Translations.Append(".en").Select(suffix => name + suffix + ".json"))];

    [Theory]
    [MemberData(nameof(Files))]
    public void The_chart_range_is_what_the_questions_can_add_up_to(string file)
    {
        JsonElement root = Load(file);
        IEnumerable<JsonElement> tests = root.ValueKind == JsonValueKind.Array ? root.EnumerateArray() : [root];
        foreach ((string analyzerId, int min, int max) in tests.Select(Reachable))
        {
            ScoreRange? range = TestScoreInterpreter.GetScoreRange(analyzerId);
            Assert.True(range is not null, $"{analyzerId} has no score range.");
            Assert.Equal((analyzerId, min, max), (analyzerId, range.Value.Min, range.Value.Max));
        }
    }
}
