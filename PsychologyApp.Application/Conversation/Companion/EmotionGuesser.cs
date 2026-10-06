using System.Reflection;
using System.Text.Json;

namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>A feeling the classifier thinks a message expresses, and how far ahead of the runner-up it is.</summary>
public sealed record EmotionGuess(CompanionEmotion Emotion, double Margin);

/// <summary>
/// Second opinion for messages the word lists did not recognise. It is only ever *proposed* to the person ("Sounds like ...?"),
/// never assumed: a wrong guess costs one tap, an assumed one costs trust.
/// </summary>
public interface IEmotionGuesser
{
    EmotionGuess? Guess(string text);
}

/// <summary>
/// Multinomial naive Bayes over character 3-5-grams plus word stems and word pairs, trained once from the labelled messages bundled with the app
/// (<c>Data/emotion-training.json</c>). Character n-grams cope with Russian word forms and typos without a stemmer; word features add
/// the phrases that matter ("не могу" next to a verb). "No feeling named" (small talk, questions about the app) is a class of its own, so the
/// classifier can say "nothing here" instead of being forced to pick a feeling.
/// Measured by leave-one-set-out on five hand-written sets (docs/companion-understanding.md). The training texts are synthetic, so real
/// people's phrasing is the open question; that is why the result is a question, not a conclusion.
/// </summary>
public sealed class EmotionGuesser : IEmotionGuesser
{
    /// <summary>Lead over the runner-up (in natural-log units) below which the classifier stays silent. Chosen on the leave-one-set-out sweep.</summary>
    public const double MinMargin = 20;

    /// <summary>Shorter messages carry too little to tell feelings apart ("hello", "just checking in").</summary>
    public const int MinWords = 5;

    private const double Smoothing = 0.1;
    private const int WordStemLength = 6;
    private const int PairStemLength = 5;

    private static readonly char[] Separators = [' ', ',', '.', '!', '?', ';', ':', '-', '—', '"', '«', '»', '(', ')', '\n', '\t'];

    private static readonly Lazy<EmotionGuesser> BundledInstance = new(() => FromJson(ReadBundledTrainingSet()), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly Dictionary<CompanionEmotion, Dictionary<string, int>> _counts = [];
    private readonly Dictionary<CompanionEmotion, int> _totals = [];
    private readonly Dictionary<CompanionEmotion, int> _documents = [];
    private readonly HashSet<string> _vocabulary = [];
    private int _documentTotal;

    /// <summary>The classifier trained on the messages shipped with the app (built on first use, a few tens of milliseconds).</summary>
    public static EmotionGuesser Bundled => BundledInstance.Value;

    public static EmotionGuesser FromJson(string json)
    {
        List<(CompanionEmotion, string)> examples = [];
        using JsonDocument document = JsonDocument.Parse(json);
        foreach (JsonElement item in document.RootElement.EnumerateArray())
        {
            string label = item.GetProperty("label").GetString() ?? string.Empty;
            string text = item.GetProperty("text").GetString() ?? string.Empty;
            if (Enum.TryParse(label, out CompanionEmotion emotion) && text.Length > 0)
            {
                examples.Add((emotion, text));
            }
        }

        EmotionGuesser guesser = new();
        guesser.Train(examples);
        return guesser;
    }

    public void Train(IEnumerable<(CompanionEmotion Emotion, string Text)> examples)
    {
        foreach ((CompanionEmotion emotion, string text) in examples)
        {
            _documents[emotion] = _documents.GetValueOrDefault(emotion) + 1;
            _documentTotal++;
            if (!_counts.TryGetValue(emotion, out Dictionary<string, int>? counts))
            {
                counts = _counts[emotion] = [];
            }

            foreach (string feature in Features(text))
            {
                counts[feature] = counts.GetValueOrDefault(feature) + 1;
                _totals[emotion] = _totals.GetValueOrDefault(emotion) + 1;
                _vocabulary.Add(feature);
            }
        }
    }

    /// <summary>A feeling to propose, or null when the message is too short, names no feeling, or two feelings are too close to call.</summary>
    public EmotionGuess? Guess(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || WordCount(text) < MinWords)
        {
            return null;
        }

        return Rank(text) is { Emotion: not CompanionEmotion.Unknown, Margin: >= MinMargin } guess ? guess : null;
    }

    /// <summary>The best class with its margin, without the gate (it can be <see cref="CompanionEmotion.Unknown"/>); used by the evaluation tests.</summary>
    public EmotionGuess? Rank(string text)
    {
        if (_counts.Count < 2)
        {
            return null;
        }

        string[] features = [.. Features(text)];
        if (features.Length == 0)
        {
            return null;
        }

        double first = double.NegativeInfinity;
        double second = double.NegativeInfinity;
        CompanionEmotion best = CompanionEmotion.Unknown;
        foreach ((CompanionEmotion emotion, Dictionary<string, int> counts) in _counts)
        {
            double score = Math.Log((double)_documents[emotion] / _documentTotal);
            double denominator = _totals[emotion] + (Smoothing * _vocabulary.Count);
            foreach (string feature in features)
            {
                score += Math.Log((counts.GetValueOrDefault(feature) + Smoothing) / denominator);
            }

            if (score > first)
            {
                second = first;
                first = score;
                best = emotion;
            }
            else if (score > second)
            {
                second = score;
            }
        }

        return new EmotionGuess(best, first - second);
    }

    private static int WordCount(string text) => text.Split(Separators, StringSplitOptions.RemoveEmptyEntries).Length;

    private static IEnumerable<string> Features(string text)
    {
        string[] words = text.ToLowerInvariant().Split(Separators, StringSplitOptions.RemoveEmptyEntries);

        string padded = " " + string.Join(' ', words) + " ";
        for (int n = 3; n <= 5; n++)
        {
            for (int i = 0; i + n <= padded.Length; i++)
            {
                yield return "c" + padded.Substring(i, n);
            }
        }

        foreach (string word in words)
        {
            yield return "w" + Stem(word, WordStemLength);
        }

        for (int i = 0; i + 1 < words.Length; i++)
        {
            yield return "b" + Stem(words[i], PairStemLength) + "_" + Stem(words[i + 1], PairStemLength);
        }
    }

    private static string Stem(string word, int length) => word.Length > length ? word[..length] : word;

    private static string ReadBundledTrainingSet()
    {
        Assembly assembly = typeof(EmotionGuesser).Assembly;
        string name = assembly.GetManifestResourceNames().Single(n => n.EndsWith("emotion-training.json", StringComparison.Ordinal));
        using Stream stream = assembly.GetManifestResourceStream(name)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
