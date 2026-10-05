namespace PsychologyApp.Application.Chat;

public static class ChatText
{
    private static readonly char[] SentenceEnds = ['!', '?', '…'];

    /// <summary>
    /// Trims the message and capitalises the start of every sentence, so what the person typed on a keyboard that does not
    /// capitalise ("я устала. он опять кричал") reads properly. A period counts as a sentence end only after a real word,
    /// so abbreviations such as "т. д." do not break the next word.
    /// </summary>
    public static string Capitalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        char[] chars = text.Trim().ToCharArray();
        bool atSentenceStart = true;
        int wordLength = 0;

        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];
            if (char.IsLetter(c))
            {
                if (atSentenceStart)
                {
                    if (!IsVerbatimToken(chars, i))
                    {
                        chars[i] = char.ToUpperInvariant(c);
                    }

                    atSentenceStart = false;
                }

                wordLength++;
                continue;
            }

            if (char.IsDigit(c))
            {
                atSentenceStart = false;
                wordLength++;
                continue;
            }

            if (Array.IndexOf(SentenceEnds, c) >= 0 || (c == '.' && wordLength >= 3))
            {
                atSentenceStart = NextIsSpaceOrEnd(chars, i);
            }
            else if (c == '\n')
            {
                atSentenceStart = true;
            }

            wordLength = 0;
        }

        return new string(chars);
    }

    /// <summary>An address, a link or a mixed-case name ("iPhone", "eBay") is left exactly as typed: capitalising it would change what it is.</summary>
    private static bool IsVerbatimToken(char[] chars, int start)
    {
        int end = start;
        while (end < chars.Length && !char.IsWhiteSpace(chars[end]))
        {
            end++;
        }

        string token = new(chars, start, end - start);
        return token.Contains('@')
            || token.Contains("://", StringComparison.Ordinal)
            || token.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
            || token.Skip(1).Any(char.IsUpper);
    }

    private static bool NextIsSpaceOrEnd(char[] chars, int index) =>
        index + 1 >= chars.Length || char.IsWhiteSpace(chars[index + 1]);
}
