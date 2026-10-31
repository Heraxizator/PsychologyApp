using System.Text.Json;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;
using Xunit.Abstractions;

namespace PsychologyApp.Application.Tests.Conversation;

/// <summary>
/// Evaluation on real messages, when there are any. Put consented, anonymised messages as JSON arrays
/// <c>[{"lang":"ru","label":"Anxiety","text":"..."}]</c> into <c>Conversation/Data/real/</c> (git-ignored; see docs/real-data-evaluation.md).
/// Labels are emotion names, "Unknown" for small talk, "Crisis" for a message that must raise the helpline card.
/// Without such files the tests do nothing, so the build does not depend on private data.
/// </summary>
public class RealMessagesTests(ITestOutputHelper output)
{
    private sealed record Item(string Lang, string Label, string Text, string? Intent = null);

    private static Item[] LoadAll()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Conversation", "Data", "real"));
        if (!Directory.Exists(dir))
        {
            return [];
        }

        return [.. Directory.GetFiles(dir, "*.json").OrderBy(f => f, StringComparer.Ordinal).SelectMany(f =>
            JsonSerializer.Deserialize<Item[]>(File.ReadAllText(f), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [])];
    }

    [Fact]
    public void Every_real_crisis_message_raises_the_helpline_card()
    {
        Item[] crisis = [.. LoadAll().Where(i => i.Label == "Crisis")];
        if (crisis.Length == 0)
        {
            output.WriteLine("No consented real crisis cases found; evaluation skipped.");
            return;
        }

        KeywordCrisisDetector detector = new();
        int[] missed = [.. crisis.Select((item, index) => (item, index))
            .Where(pair => !detector.IsCrisis(pair.item.Text))
            .Select(pair => pair.index + 1)];

        output.WriteLine($"Consented crisis cases: {crisis.Length}, missed: {missed.Length}; case numbers: {string.Join(", ", missed)}.");
        Assert.Empty(missed);
    }

    [Fact]
    public void Real_message_accuracy_is_reported()
    {
        Item[] items = [.. LoadAll().Where(i => i.Label != "Crisis")];
        if (items.Length == 0)
        {
            output.WriteLine("No consented real emotion cases found; evaluation skipped.");
            return;
        }

        LexiconSituationAnalyzer analyzer = new();
        EmotionGuesser guesser = EmotionGuesser.Bundled;
        int lexicon = 0;
        int combined = 0;
        for (int index = 0; index < items.Length; index++)
        {
            Item item = items[index];
            CompanionEmotion read = analyzer.Analyze(item.Text).Emotion;
            lexicon += read.ToString() == item.Label ? 1 : 0;
            CompanionEmotion final = read != CompanionEmotion.Unknown ? read : guesser.Guess(item.Text)?.Emotion ?? CompanionEmotion.Unknown;
            bool right = final.ToString() == item.Label;
            combined += right ? 1 : 0;
            if (!right)
            {
                output.WriteLine($"Emotion mismatch at case {index + 1}: expected {item.Label}, got {final}.");
            }
        }

        output.WriteLine($"Real messages: {items.Length}; lexicon {(double)lexicon / items.Length:P0}; lexicon + guesser {(double)combined / items.Length:P0}");
    }

    [Fact]
    public void Real_intent_accuracy_is_checked_without_logging_message_text()
    {
        Item[] items = [.. LoadAll().Where(i => !string.IsNullOrWhiteSpace(i.Intent))];
        if (items.Length == 0)
        {
            output.WriteLine("No consented real intent cases found; evaluation skipped.");
            return;
        }

        LexiconSituationAnalyzer analyzer = new();
        List<(int Index, string Expected, string Actual)> misses = [];
        Dictionary<string, (int Total, int Correct)> byIntent = new(StringComparer.Ordinal);
        for (int index = 0; index < items.Length; index++)
        {
            Item item = items[index];
            string expected = item.Intent!;
            bool hasFeeling = analyzer.Analyze(item.Text).Emotion != CompanionEmotion.Unknown;
            string actual = UtteranceClassifier.Classify(item.Text, hasFeeling).ToString();
            byIntent.TryGetValue(expected, out (int Total, int Correct) counts);
            byIntent[expected] = (counts.Total + 1, counts.Correct + (actual == expected ? 1 : 0));
            if (actual != expected)
            {
                misses.Add((index + 1, expected, actual));
            }
        }

        foreach ((string intent, (int total, int correct)) in byIntent.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            output.WriteLine($"Intent {intent}: {correct}/{total} correct.");
        }

        output.WriteLine($"Real intent cases: {items.Length}; misses: {misses.Count}.");
        foreach ((int index, string expected, string actual) in misses)
        {
            output.WriteLine($"Intent mismatch at case {index}: expected {expected}, got {actual}.");
        }

        Assert.Empty(misses);
    }
}
