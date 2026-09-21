using System.Text.RegularExpressions;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Picks the sentence that carries the feeling so the companion can quote it back verbatim. Quoting avoids the grammar
/// mistakes of rephrasing Russian ("Я устал" -> "Вы устали") and shows the person that their own words were heard.
/// </summary>
public static partial class QuoteExtractor
{
    private const int MinLength = 8;

    [GeneratedRegex(@"(?<=[.!?…])\s+|\n+", RegexOptions.CultureInvariant)]
    private static partial Regex SentenceSplit();

    public static string? Pick(string text, ISituationAnalyzer analyzer, CompanionEmotion emotion, int maxLength = 110)
    {
        List<string> sentences = SentenceSplit().Split(text)
            .Select(s => s.Trim().TrimEnd('.', '!', '?', '…', ' '))
            .Where(s => s.Length >= MinLength)
            .ToList();
        if (sentences.Count == 0)
        {
            return null;
        }

        string? chosen = null;
        if (emotion != CompanionEmotion.Unknown)
        {
            chosen = sentences.FirstOrDefault(s => analyzer.Analyze(s).Emotion == emotion);
        }

        chosen ??= sentences.FirstOrDefault(s => analyzer.Analyze(s).Emotion != CompanionEmotion.Unknown);
        chosen ??= sentences.OrderByDescending(s => s.Length).First();

        return Truncate(chosen, maxLength);
    }

    private static string Truncate(string sentence, int maxLength)
    {
        if (sentence.Length <= maxLength)
        {
            return sentence;
        }

        int cut = sentence.LastIndexOf(' ', maxLength);
        return (cut > maxLength / 2 ? sentence[..cut] : sentence[..maxLength]).TrimEnd(',', ';', ':', ' ') + "…";
    }
}
