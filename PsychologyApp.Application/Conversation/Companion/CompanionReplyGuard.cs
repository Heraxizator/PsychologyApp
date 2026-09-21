using System.Text;
using System.Text.RegularExpressions;

namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>
/// Deterministic safety and quality net between the language model and the user. A small model can drift into diagnoses,
/// medication talk, advice, dismissive phrases, lists, the wrong register or language, or simply talk about something
/// the person never said. Anything suspicious is dropped so the caller falls back to reviewed scripted text.
/// </summary>
public static partial class CompanionReplyGuard
{
    public const int MaxSentences = 3;
    public const int MaxLength = 420;
    private const int MinLength = 25;
    private const int StemLength = 5;

    private static readonly string[] Banned =
    [
        // Medical / identity
        "диагноз", "расстройств", "у вас депресс", "у тебя депресс", "антидепрессант", "таблетк", "лекарств", "препарат", "принимайте", "принимай ", "рекомендую принять",
        "языковая модель", "как ии", "как искусственный интеллект",
        "diagnos", "disorder", "antidepress", "medication", "prescri", "dosage", "you have depression", "as an ai", "language model",
        "http", "www.",
        // Advice and dismissing phrases: the prompt forbids them, small models do them anyway
        "не бойтесь", "не бойся", "не переживай", "не волнуйтесь", "не волнуйся", "успокойтесь", "успокойся", "попробуйте", "попробуй", "советую", "рекомендую",
        "вам нужно", "тебе нужно", "вам стоит", "нужно просто", "просто отдохни", "помните, что", "помни, что", "все будет хорошо", "все пройдет", "все наладится",
        "dont worry", "do not worry", "calm down", "try to", "you should", "you need to", "just relax", "it will be fine", "everything will be",
        // The bot talking about its own feelings or experience
        "чувствую себя", "я тоже", "мне тоже", "i feel ", "me too"
    ];

    /// <summary>Informal second person: the app addresses people as "вы".</summary>
    private static readonly HashSet<string> InformalYou =
    [
        "ты", "тебе", "тебя", "тобой", "твой", "твоя", "твое", "твои", "твоих", "твоей", "твоим", "твоего"
    ];

    [GeneratedRegex(@"<think>.*?</think>", RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex ThinkBlock();

    [GeneratedRegex(@"<\|[^|>]*\|>", RegexOptions.CultureInvariant)]
    private static partial Regex SpecialToken();

    [GeneratedRegex(@"(?<=[.!?…])\s+", RegexOptions.CultureInvariant)]
    private static partial Regex SentenceSplit();

    [GeneratedRegex(@"^\s*(?:[-•*]|\d+[.)])\s+", RegexOptions.CultureInvariant)]
    private static partial Regex ListMarker();

    [GeneratedRegex(@"\p{L}{6,}(?:ешь|ишь)$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    private static partial Regex InformalVerb();

    [GeneratedRegex(@"([.!?])\1+", RegexOptions.CultureInvariant)]
    private static partial Regex RepeatedPunctuation();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex Whitespace();

    /// <param name="raw">Model output.</param>
    /// <param name="english">Expected language.</param>
    /// <param name="userText">When given, the reply must be visibly about what the person wrote (shares a word stem with it).</param>
    /// <returns>Cleaned reply, or <c>null</c> when the reply must not be shown.</returns>
    public static string? Sanitize(string? raw, bool english, string? userText = null)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        // Reasoning models emit a <think> monologue that must never reach the user; an unfinished one means the reply is unusable.
        raw = SpecialToken().Replace(ThinkBlock().Replace(raw, string.Empty), string.Empty);
        if (raw.Contains("<think", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // SentencePiece models can leak the "▁" word-boundary marker into decoded text.
        string flat = string.Join(' ', raw.Replace('▁', ' ').Split('\n', '\r').Select(line => ListMarker().Replace(line, string.Empty)))
            .Replace("*", string.Empty)
            .Replace("_", " ")
            .Replace("#", string.Empty)
            .Replace("`", string.Empty)
            .Replace(">", string.Empty);
        flat = Whitespace().Replace(RepeatedPunctuation().Replace(flat, "$1"), " ").Trim();

        StringBuilder result = new();
        int sentences = 0;
        foreach (string part in SentenceSplit().Split(flat))
        {
            string sentence = part.Trim();
            if (sentence.Length == 0 || sentence.EndsWith('?'))
            {
                continue;
            }

            if (result.Length + sentence.Length + 1 > MaxLength || sentences >= MaxSentences)
            {
                break;
            }

            if (result.Length > 0)
            {
                result.Append(' ');
            }

            result.Append(sentence);
            sentences++;
        }

        string text = result.ToString();
        if (text.Length < MinLength)
        {
            return null;
        }

        string lowered = text.ToLowerInvariant().Replace('ё', 'е');
        if (Banned.Any(term => lowered.Contains(term, StringComparison.Ordinal)))
        {
            return null;
        }

        if (!english && UsesInformalYou(lowered))
        {
            return null;
        }

        if (!LanguageMatches(text, english))
        {
            return null;
        }

        return userText is not null && !SharesTopicWith(lowered, userText) ? null : text;
    }

    private static bool UsesInformalYou(string lowered)
    {
        foreach (string word in Regex.Split(lowered, @"[^\p{L}]+"))
        {
            if (InformalYou.Contains(word) || InformalVerb().IsMatch(word))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>A reflection reuses the person's own words; a reply with no shared stem is about something else.</summary>
    private static bool SharesTopicWith(string loweredReply, string userText)
    {
        HashSet<string> replyStems = Stems(loweredReply);
        return Stems(userText.ToLowerInvariant().Replace('ё', 'е')).Overlaps(replyStems);
    }

    private static HashSet<string> Stems(string text) =>
        Regex.Split(text, @"[^\p{L}]+")
            .Where(word => word.Length >= StemLength)
            .Select(word => word[..StemLength])
            .ToHashSet(StringComparer.Ordinal);

    private static bool LanguageMatches(string text, bool english)
    {
        int letters = 0;
        int cyrillic = 0;
        foreach (char c in text)
        {
            if (!char.IsLetter(c))
            {
                continue;
            }

            letters++;
            if (c is >= 'Ѐ' and <= 'ӿ')
            {
                cyrillic++;
            }
        }

        if (letters == 0)
        {
            return false;
        }

        double share = (double)cyrillic / letters;
        return english ? share <= 0.1 : share >= 0.7;
    }
}
