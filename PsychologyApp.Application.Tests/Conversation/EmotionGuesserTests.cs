using System.Text.Json;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;
using Xunit.Abstractions;

namespace PsychologyApp.Application.Tests.Conversation;

public class EmotionGuesserTests(ITestOutputHelper output)
{
    private sealed record Item(string Label, string Text);

    private static readonly string[] Sets = ["nlu-dataset.json", "nlu-heldout.json", "nlu-fresh.json", "nlu-fresh2.json", "nlu-fresh3.json"];

    private static Item[] Load(string file) => JsonSerializer.Deserialize<Item[]>(
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", file)),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

    private static EmotionGuesser TrainedWithout(string heldOut)
    {
        EmotionGuesser guesser = new();
        guesser.Train(Sets.Where(s => s != heldOut).SelectMany(Load).Concat(Load("nlu-train-extra.json"))
            .Where(i => Enum.TryParse(i.Label, out CompanionEmotion _))
            .Select(i => (Enum.Parse<CompanionEmotion>(i.Label), i.Text)));
        return guesser;
    }

    /// <summary>The honest number: trained on four sets and the extra messages, asked about the fifth, which it has never seen.</summary>
    [Fact]
    public void ALeftOutSetIsGuessedBetterThanTheLexiconAloneAndTheGuessesAreUsuallyRight()
    {
        LexiconSituationAnalyzer lexicon = new();
        int offered = 0, right = 0, lexiconMissed = 0, lexiconHits = 0, combinedHits = 0, total = 0, chitChatOffered = 0;
        foreach (string heldOut in Sets)
        {
            EmotionGuesser guesser = TrainedWithout(heldOut);
            int setLexicon = 0, setCombined = 0, setCount = 0;
            foreach (Item item in Load(heldOut))
            {
                total++;
                string lex = lexicon.Analyze(item.Text).Emotion.ToString();
                EmotionGuess? guess = guesser.Guess(item.Text);
                lexiconHits += lex == item.Label ? 1 : 0;
                setCount++;
                setLexicon += lex == item.Label ? 1 : 0;
                string pick = lex != "Unknown" ? lex : guess?.Emotion.ToString() ?? "Unknown";
                combinedHits += pick == item.Label ? 1 : 0;
                setCombined += pick == item.Label ? 1 : 0;
                if (lex == "Unknown" && item.Label != "Unknown")
                {
                    lexiconMissed++;
                }

                if (lex == "Unknown" && guess is not null)
                {
                    offered++;
                    right += guess.Emotion.ToString() == item.Label ? 1 : 0;
                    chitChatOffered += item.Label == "Unknown" ? 1 : 0;
                }
            }

            output.WriteLine($"  {heldOut}: lexicon {setLexicon}/{setCount} ({(double)setLexicon / setCount:P0}), lexicon then guesser {setCombined}/{setCount} ({(double)setCombined / setCount:P0})");
        }

        output.WriteLine($"lexicon {lexiconHits}/{total}; lexicon then guesser {combinedHits}/{total}; proposed {offered} of {lexiconMissed} misses, right {right}, chit-chat wrongly proposed {chitChatOffered}");
        Assert.True(combinedHits >= lexiconHits + 40, $"The guesser must add clearly: {lexiconHits} -> {combinedHits}.");
        Assert.True(offered >= lexiconMissed * 0.7, $"Proposed only {offered} of {lexiconMissed}.");
        Assert.True((double)right / offered >= 0.8, $"Precision of the proposals fell to {(double)right / offered:P0}.");
        Assert.True(chitChatOffered <= 2, $"{chitChatOffered} chit-chat messages were given a feeling.");
    }

    [Fact]
    public void TheBundledGuesserKnowsEveryFeelingAndStaysSilentOnSmallTalk()
    {
        EmotionGuesser guesser = EmotionGuesser.Bundled;

        Assert.Null(guesser.Guess(string.Empty));
        Assert.Null(guesser.Guess("hello"));
        Assert.Null(guesser.Guess("Какие у тебя есть упражнения"));
        Assert.Null(guesser.Guess("Today I cooked pasta and then watched a movie with my brother"));
        Assert.Equal(CompanionEmotion.Loneliness, guesser.Guess("В новом городе у меня нет ни одного друга и поговорить вечером не с кем")?.Emotion);
    }

    [Fact]
    public void AnUntrainedGuesserProposesNothing()
    {
        Assert.Null(new EmotionGuesser().Guess("I keep worrying about tomorrow and cannot relax at all"));
    }

    [Fact]
    public void ABlankOrSingleClassGuesserNeverThrows()
    {
        EmotionGuesser guesser = new();
        guesser.Train([(CompanionEmotion.Anxiety, "I am worried about tomorrow")]);

        Assert.Null(guesser.Guess("   "));
        Assert.Null(guesser.Guess("I am worried about tomorrow and the day after"));
    }

    /// <summary>The shipped training set is generated from the labelled sets; this fails when one was edited without rebuilding it.</summary>
    [Fact]
    public void TheShippedTrainingSetContainsEveryLabelledMessage()
    {
        using Stream stream = typeof(EmotionGuesser).Assembly.GetManifestResourceStream(
            typeof(EmotionGuesser).Assembly.GetManifestResourceNames().Single(n => n.EndsWith("emotion-training.json", StringComparison.Ordinal)))!;
        using StreamReader reader = new(stream);
        HashSet<string> shipped = JsonSerializer.Deserialize<Item[]>(reader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!
            .Select(i => i.Label + "|" + i.Text.ToLowerInvariant()).ToHashSet();

        foreach (string file in Sets.Append("nlu-train-extra.json"))
        {
            foreach (Item item in Load(file))
            {
                Assert.True(shipped.Contains(item.Label + "|" + item.Text.ToLowerInvariant()), $"{file} has a message that is not in emotion-training.json (run: node tools/build-emotion-training.js): {item.Text}");
            }
        }
    }
}
