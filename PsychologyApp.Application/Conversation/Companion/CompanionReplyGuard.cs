using System.Text;
using System.Text.RegularExpressions;

namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>
/// Deterministic safety net between the language model and the user. A small model can drift into diagnoses,
/// medication talk, lists or the wrong language; anything suspicious is dropped so the caller falls back to scripted text.
/// </summary>
public static partial class CompanionReplyGuard
{
    public const int MaxSentences = 3;
    public const int MaxLength = 420;

    private static readonly string[] Banned =
    [
        "диагноз", "расстройств", "у вас депресс", "у тебя депресс", "антидепрессант", "таблетк", "лекарств", "препарат", "принимайте", "принимай ", "рекомендую принять",
        "языковая модель", "как ии", "как искусственный интеллект",
        "diagnos", "disorder", "antidepress", "medication", "prescri", "dosage", "you have depression", "as an ai", "language model",
        "http", "www."
    ];

    [GeneratedRegex(@"(?<=[.!?…])\s+", RegexOptions.CultureInvariant)]
    private static partial Regex SentenceSplit();

    [GeneratedRegex(@"^\s*(?:[-•*]|\d+[.)])\s+", RegexOptions.CultureInvariant)]
    private static partial Regex ListMarker();

    /// <returns>Cleaned reply, or <c>null</c> when the reply must not be shown.</returns>
    public static string? Sanitize(string? raw, bool english)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        string flat = string.Join(' ', raw.Split('\n', '\r').Select(line => ListMarker().Replace(line, string.Empty)))
            .Replace("*", string.Empty)
            .Replace("_", " ")
            .Replace("#", string.Empty)
            .Replace("`", string.Empty)
            .Replace(">", string.Empty);

        StringBuilder result = new();
        int sentences = 0;
        foreach (string part in SentenceSplit().Split(flat.Trim()))
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
        if (text.Length == 0)
        {
            return null;
        }

        string lowered = text.ToLowerInvariant();
        if (Banned.Any(term => lowered.Contains(term, StringComparison.Ordinal)))
        {
            return null;
        }

        return LanguageMatches(text, english) ? text : null;
    }

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
