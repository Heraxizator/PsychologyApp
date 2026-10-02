namespace PsychologyApp.Application.Conversation;

public interface ICrisisDetector
{
    bool IsCrisis(string? text);
}

/// <summary>
/// Conservative phrase matcher that runs locally before any scenario logic or language model sees free text.
/// It favours false positives: a wrongly shown helpline card costs little, a missed one costs a lot.
/// Text is normalised (lower case, "ё" = "е", apostrophes removed, punctuation collapsed) so "Don't" and "dont" behave the same.
/// </summary>
public sealed class KeywordCrisisDetector : ICrisisDetector
{
    private static readonly string[] Markers =
    [
        // Russian
        "суицид", "самоубий", "покончить с собой", "покончу с собой", "убить себя", "убью себя", "покончить с жизнью", "уйти из жизни",
        "не хочу жить", "не хочется жить", "не хочу больше жить", "жить не хочу", "хочу умереть", "хочется умереть", "хочу сдохнуть", "хочу исчезнуть", "хочется исчезнуть",
        "не хочу быть живым", "не хочу быть живой", "не хочу просыпаться", "уснуть и не проснуться",
        "лучше бы я умер", "лучше умереть", "лучше бы меня не было", "жить не стоит", "смысла жить нет", "нет смысла жить", "не вижу смысла жить",
        "не вижу смысла в жизни", "жизнь не имеет смысла", "надоело жить", "устал жить", "устала жить",
        "свести счеты", "наложить на себя руки", "вскрыть вены", "повеситься", "спрыгнуть с крыши",
        "причинить себе вред", "навредить себе", "режу себя", "резать себя", "самоповрежд",
        "повешусь", "вешаться", "повесить себя", "вскрою вены", "порезать вены", "перерезать вены", "вскрыться", "вскроюсь",
        "выйти в окно", "выброситься из окна", "выпрыгнуть из окна", "спрыгнуть с моста", "прыгнуть с моста", "броситься под поезд",
        "наглотаться таблеток", "выпить все таблетки", "выпью все таблетки", "хочу покончить", "умереть хочу", "лучше сдохнуть",
        "меня не станет", "чтобы меня не стало", "лучше без меня", "лучше будет без меня", "не хочу существовать",
        "всем будет лучше без меня", "жизнь не стоит", "не стоит жить",
        "режу руки", "порезала себя", "порезал себя", "причиняю себе боль", "делаю себе больно",
        // English (apostrophes are stripped before matching)
        "kill myself", "suicide", "suicidal", "want to die", "end my life", "end it all", "take my own life", "hurt myself",
        "self harm", "selfharm", "no reason to live", "nothing to live for", "not worth living", "better off dead", "wish i was dead",
        "wish i were dead", "wish i was never born", "wish i wasnt alive", "want to disappear", "cant go on", "kill me",
        "dont want to live", "do not want to live", "dont want to be alive", "dont want to be here anymore", "no longer want to live",
        "cut myself", "cutting myself", "slit my wrists", "hang myself", "shoot myself", "overdose on purpose", "take all my pills",
        "jump off a bridge", "jump off the roof", "jump off a building", "hurting myself", "harm myself", "harming myself",
        "off myself", "unalive myself", "wish i werent alive", "wish i werent here", "want to be dead", "rather be dead",
        "no point in living", "no point living", "better without me", "everyone would be better off", "dont want to exist",
        "dont want to wake up", "never wake up"
    ];

    /// <summary>
    /// Slang that is only a signal as a whole word: as a substring "kms" would match "5 kms" and "unalive" is harmless inside
    /// longer words. Matched against the normalised tokens instead of the text.
    /// </summary>
    private static readonly HashSet<string> TokenMarkers = new(StringComparer.Ordinal) { "kms", "unalive", "unaliving", "kys" };

    public bool IsCrisis(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string normalized = Normalize(text);
        foreach (string marker in Markers)
        {
            if (normalized.Contains(marker, StringComparison.Ordinal))
            {
                return true;
            }
        }

        string[] tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return HasSlangMarker(tokens) || NegatedWishToLive(tokens);
    }

    /// <summary>
    /// Catches loosely worded variants a fixed phrase list cannot, e.g. "не хочу уже совсем жить" or "I really don't want to keep living":
    /// a negation within three words before "want", and a "live" word within four words after it.
    /// </summary>
    // "5 kms" is distance, not slang: a bare number before the token rules it out.
    private static bool HasSlangMarker(string[] tokens)
    {
        for (int i = 0; i < tokens.Length; i++)
        {
            if (TokenMarkers.Contains(tokens[i]) && !(i > 0 && tokens[i - 1].All(char.IsDigit)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool NegatedWishToLive(string[] tokens)
    {
        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] is not ("хочу" or "хочется" or "want" or "wanna"))
            {
                continue;
            }

            bool negated = Enumerable.Range(Math.Max(0, i - 3), Math.Min(3, i))
                .Any(k => tokens[k] is "не" or "dont" or "not" or "never");
            if (!negated)
            {
                continue;
            }

            bool livesSoon = Enumerable.Range(i + 1, Math.Min(4, tokens.Length - i - 1))
                .Any(k => tokens[k] is "жить" or "live" or "living" or "alive");
            if (livesSoon)
            {
                return true;
            }
        }

        return false;
    }

    private static string Normalize(string text)
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
