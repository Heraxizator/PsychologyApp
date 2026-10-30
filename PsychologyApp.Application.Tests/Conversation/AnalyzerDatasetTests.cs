using System.Text.Json;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;
using Xunit.Abstractions;

namespace PsychologyApp.Application.Tests.Conversation;

/// <summary>
/// Accuracy of the lexicon analyzer on labelled messages (Data/*.json). Two sets on purpose:
/// <c>nlu-dataset.json</c> is the development set the lexicon was tuned on (so its accuracy is inflated and only guards regressions);
/// <c>nlu-heldout.json</c> was written afterwards and never used for tuning, so it is the honest estimate of how the lexicon
/// copes with unseen phrasing. Do not add terms based on held-out misses without replacing that set with a fresh one.
/// </summary>
public class AnalyzerDatasetTests(ITestOutputHelper output)
{
    private sealed record Item(string Lang, string Label, string Text);

    private static Item[] Load(string file) => JsonSerializer.Deserialize<Item[]>(
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", file)),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

    private double Measure(string file, string name)
    {
        LexiconSituationAnalyzer analyzer = new();
        Item[] items = Load(file);
        List<string> misses = [];
        Dictionary<string, (int Total, int Correct)> byLabel = [];
        int hits = 0;

        foreach (Item item in items)
        {
            string predicted = analyzer.Analyze(item.Text).Emotion.ToString();
            (int total, int correct) = byLabel.GetValueOrDefault(item.Label);
            byLabel[item.Label] = (total + 1, correct + (predicted == item.Label ? 1 : 0));
            if (predicted == item.Label)
            {
                hits++;
            }
            else
            {
                misses.Add($"[{item.Label} -> {predicted}] {item.Text}");
            }
        }

        double accuracy = (double)hits / items.Length;
        output.WriteLine($"{name}: {hits}/{items.Length} = {accuracy:P0}");
        foreach (KeyValuePair<string, (int Total, int Correct)> pair in byLabel.OrderBy(pair => pair.Key))
        {
            output.WriteLine($"  {pair.Key}: recall {pair.Value.Correct}/{pair.Value.Total} = {(double)pair.Value.Correct / pair.Value.Total:P0}");
        }

        misses.ForEach(output.WriteLine);
        return accuracy;
    }

    [Fact]
    public void Development_set_accuracy_does_not_regress()
    {
        double accuracy = Measure("nlu-dataset.json", "Development set (tuned on)");

        Assert.True(accuracy >= 0.95, $"Development-set accuracy fell to {accuracy:P0}.");
    }

    [Fact]
    public void Held_out_accuracy_is_recorded_and_does_not_collapse()
    {
        double accuracy = Measure("nlu-heldout.json", "Held-out set (never tuned on)");

        // This is the second held-out set: the first was spent (read and tuned against, then folded into
        // nlu-dataset.json — see its trailing entries) after a round of lexicon expansion. Measured 33% on
        // this fresh one, up from an initial 24% on the first. The floor only detects breakage.
        Assert.True(accuracy >= 0.20, $"Held-out accuracy fell to {accuracy:P0}.");
    }

    [Fact]
    public void Fresh_set_accuracy_is_recorded_and_does_not_collapse()
    {
        double accuracy = Measure("nlu-fresh.json", "Fresh set (written before the third lexicon round)");

        Assert.True(accuracy >= 0.20, $"Fresh-set accuracy fell to {accuracy:P0}.");
    }

    [Fact]
    public void Final_set_accuracy_is_recorded_and_does_not_collapse()
    {
        // Written after the third lexicon round and never tuned on: the honest estimate. Do not add terms from its misses.
        double accuracy = Measure("nlu-fresh2.json", "Final set (written after the third round, never tuned on)");

        Assert.True(accuracy >= 0.20, $"Final-set accuracy fell to {accuracy:P0}.");
    }

    [Fact]
    public void Third_final_set_accuracy_is_recorded_and_does_not_collapse()
    {
        double accuracy = Measure("nlu-fresh3.json", "Third final set (written before the guesser was extended)");

        Assert.True(accuracy >= 0.20, $"Third-set accuracy fell to {accuracy:P0}.");
    }

    [Fact]
    public void Fourth_final_set_accuracy_is_recorded_and_does_not_collapse()
    {
        double accuracy = Measure("nlu-fresh4.json", "Fourth final set (never tuned on)");

        Assert.True(accuracy >= 0.20, $"Fourth-set accuracy fell to {accuracy:P0}.");
    }

    [Theory]
    [InlineData("nlu-dataset.json")]
    [InlineData("nlu-heldout.json")]
    [InlineData("nlu-fresh.json")]
    [InlineData("nlu-fresh2.json")]
    [InlineData("nlu-fresh3.json")]
    [InlineData("nlu-fresh4.json")]
    public void Datasets_are_well_formed(string file)
    {
        Item[] items = Load(file);

        Assert.True(items.Length >= 45);
        Assert.All(Enum.GetNames<CompanionEmotion>(), name => Assert.True(items.Count(i => i.Label == name) >= 2, $"{file}: {name}"));
        Assert.All(items, i => Assert.False(string.IsNullOrWhiteSpace(i.Text)));
    }
}
