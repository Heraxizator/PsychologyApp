using System.Text.Json;
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
    private sealed record Item(string Lang, string Label, string Text);

    private static Item[] LoadAll()
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "Data", "real");
        if (!Directory.Exists(dir))
        {
            return [];
        }

        return [.. Directory.GetFiles(dir, "*.json").SelectMany(f =>
            JsonSerializer.Deserialize<Item[]>(File.ReadAllText(f), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [])];
    }

    [Fact]
    public void Every_real_crisis_message_raises_the_helpline_card()
    {
        Item[] crisis = [.. LoadAll().Where(i => i.Label == "Crisis")];
        KeywordCrisisDetector detector = new();
        string[] missed = [.. crisis.Where(i => !detector.IsCrisis(i.Text)).Select(i => i.Text)];

        output.WriteLine($"Crisis messages: {crisis.Length}, missed: {missed.Length}");
        missed.ToList().ForEach(output.WriteLine);
        Assert.Empty(missed);
    }

    [Fact]
    public void Real_message_accuracy_is_reported()
    {
        Item[] items = [.. LoadAll().Where(i => i.Label != "Crisis")];
        if (items.Length == 0)
        {
            return;
        }

        LexiconSituationAnalyzer analyzer = new();
        EmotionGuesser guesser = EmotionGuesser.Bundled;
        int lexicon = 0;
        int combined = 0;
        foreach (Item item in items)
        {
            CompanionEmotion read = analyzer.Analyze(item.Text).Emotion;
            lexicon += read.ToString() == item.Label ? 1 : 0;
            CompanionEmotion final = read != CompanionEmotion.Unknown ? read : guesser.Guess(item.Text)?.Emotion ?? CompanionEmotion.Unknown;
            bool right = final.ToString() == item.Label;
            combined += right ? 1 : 0;
            if (!right)
            {
                output.WriteLine($"[{item.Label} -> {final}] {item.Text}");
            }
        }

        output.WriteLine($"Real messages: {items.Length}; lexicon {(double)lexicon / items.Length:P0}; lexicon + guesser {(double)combined / items.Length:P0}");
    }
}
