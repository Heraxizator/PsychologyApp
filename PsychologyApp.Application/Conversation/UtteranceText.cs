namespace PsychologyApp.Application.Conversation;

/// <summary>
/// How a person's message is read before matching: lower case, ё as е, apostrophes dropped ("don't" is "dont"), every
/// other non-letter a single space. The crisis check and the situation analyzer share it, so a phrase one of them
/// matches cannot be split differently for the other.
/// </summary>
internal static class UtteranceText
{
    public static string Normalize(string text)
    {
        char[] chars = text.ToLowerInvariant()
            .Replace('ё', 'е')
            .Replace("'", string.Empty)
            .Replace("’", string.Empty)
            .Select(c => char.IsLetterOrDigit(c) ? c : ' ')
            .ToArray();
        return string.Join(' ', new string(chars).Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
