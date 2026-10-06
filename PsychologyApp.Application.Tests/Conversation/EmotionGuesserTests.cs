using System.Text.Json;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;
using Xunit.Abstractions;

namespace PsychologyApp.Application.Tests.Conversation;

public class EmotionGuesserTests(ITestOutputHelper output)
{
    private sealed record Item(string Lang, string Label, string Text);

    private static readonly string[] Sets = ["nlu-dataset.json", "nlu-heldout.json", "nlu-fresh.json", "nlu-fresh2.json"];

    private static Item[] Load(string file) => JsonSerializer.Deserialize<Item[]>(
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", file)),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

    private static EmotionGuesser TrainedWithout(string heldOut)
    {
        EmotionGuesser guesser = new();
        guesser.Train(Sets.Where(s => s != heldOut).SelectMany(Load)
            .Where(i => Enum.TryParse(i.Label, out CompanionEmotion e) && e != CompanionEmotion.Unknown)
            .Select(i => (Enum.Parse<CompanionEmotion>(i.Label), i.Text)));
        return guesser;
    }

    /// <summary>The honest number: trained on three sets, asked about the fourth, which it has never seen.</summary>
    [Fact]
    public void ALeftOutSetIsGuessedBetterThanTheLexiconAloneAndTheGuessesAreUsuallyRight()
    {
        LexiconSituationAnalyzer lexicon = new();
        int offered = 0, right = 0, lexiconMissed = 0, lexiconHits = 0, combinedHits = 0, total = 0, chitChatOffered = 0;
        foreach (string heldOut in Sets)
        {
            EmotionGuesser guesser = TrainedWithout(heldOut);
            foreach (Item item in Load(heldOut))
            {
                total++;
                string lex = lexicon.Analyze(item.Text).Emotion.ToString();
                EmotionGuess? guess = guesser.Guess(item.Text);
                lexiconHits += lex == item.Label ? 1 : 0;
                string pick = lex != "Unknown" ? lex : guess?.Emotion.ToString() ?? "Unknown";
                combinedHits += pick == item.Label ? 1 : 0;
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
        }

        output.WriteLine($"lexicon {lexiconHits}/{total}; lexicon then guesser {combinedHits}/{total}; proposed {offered} of {lexiconMissed} misses, right {right}, chit-chat wrongly proposed {chitChatOffered}");
        Assert.True(combinedHits > lexiconHits, "The guesser must add something.");
        Assert.True(offered >= lexiconMissed / 2, $"Proposed only {offered} of {lexiconMissed}.");
        Assert.True((double)right / offered >= 0.55, $"Precision of the proposals fell to {(double)right / offered:P0}.");
        Assert.True(chitChatOffered <= 3, $"{chitChatOffered} chit-chat messages were given a feeling.");
    }

    [Fact]
    public void TheBundledGuesserKnowsEveryFeelingAndStaysSilentOnSmallTalk()
    {
        EmotionGuesser guesser = EmotionGuesser.Bundled;

        Assert.Null(guesser.Guess(string.Empty));
        Assert.Null(guesser.Guess("hello"));
        Assert.Null(guesser.Guess("Какие у тебя есть упражнения"));
        Assert.Equal(CompanionEmotion.Loneliness, guesser.Guess("В новом городе у меня нет ни одного друга и поговорить вечером не с кем")?.Emotion);
    }

    [Fact]
    public void AnUntrainedGuesserProposesNothing()
    {
        Assert.Null(new EmotionGuesser().Guess("I keep worrying about tomorrow and cannot relax at all"));
    }

    [Fact]
    public void TheTrainingFileIsWellFormedAndNeverEmptyForAFeeling()
    {
        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", "nlu-fresh.json"));
        EmotionGuesser guesser = EmotionGuesser.FromJson(json);

        Assert.NotNull(guesser.Rank("I keep feeling like something bad is about to happen and I cannot relax"));
    }
}
