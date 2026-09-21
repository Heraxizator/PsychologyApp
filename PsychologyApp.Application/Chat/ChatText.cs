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
                    chars[i] = char.ToUpperInvariant(c);
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

    private static bool NextIsSpaceOrEnd(char[] chars, int index) =>
        index + 1 >= chars.Length || char.IsWhiteSpace(chars[index + 1]);
}
