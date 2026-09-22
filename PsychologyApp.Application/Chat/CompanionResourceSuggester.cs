using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>What else, besides a technique, might genuinely fit right now.</summary>
public enum CompanionResourceKind
{
    None,
    /// <summary>A short quote, shown right in the chat — no navigation needed.</summary>
    Quote,
    /// <summary>The body/psychosomatic explorer: fits feelings that show up physically.</summary>
    Somatic,
    /// <summary>A short self-assessment test.</summary>
    Test,
    /// <summary>Calming audio / prayers, offered alongside a calming practice.</summary>
    Prayer
}

/// <summary>
/// Decides whether the companion's practice offer should also point at something else in the app: a test, the body
/// explorer, a quote, or calming audio. Offered at most once per chat (<see cref="CompanionState.OfferedResource"/>),
/// so a long conversation is not showered with extra suggestions every time a practice is offered.
/// </summary>
public static class CompanionResourceSuggester
{
    /// <summary>Which kind of extra resource fits this moment, if any. <paramref name="calming"/> mirrors the same flag
    /// <c>Offer</c> uses for a body-first practice: high tension or an explicit request to calm down first.</summary>
    public static CompanionResourceKind Suggest(CompanionEmotion emotion, bool calming)
    {
        if (calming)
        {
            return CompanionResourceKind.Prayer;
        }

        return emotion switch
        {
            CompanionEmotion.Panic or CompanionEmotion.Anxiety => CompanionResourceKind.Somatic,
            CompanionEmotion.Overthinking or CompanionEmotion.Exhaustion => CompanionResourceKind.Test,
            CompanionEmotion.Sadness or CompanionEmotion.Loneliness or CompanionEmotion.Guilt
                or CompanionEmotion.Resentment or CompanionEmotion.Anger or CompanionEmotion.Procrastination => CompanionResourceKind.Quote,
            _ => CompanionResourceKind.None
        };
    }

    // Quote themes are the same in every language file; only the text and author change with it.
    private static readonly Dictionary<CompanionEmotion, string[]> QuoteThemes = new()
    {
        [CompanionEmotion.Sadness] = ["hope", "healing"],
        [CompanionEmotion.Loneliness] = ["love", "relationships", "hope"],
        [CompanionEmotion.Guilt] = ["self-love", "acceptance"],
        [CompanionEmotion.Resentment] = ["empathy", "acceptance"],
        [CompanionEmotion.Anger] = ["acceptance", "self-awareness"],
        [CompanionEmotion.Procrastination] = ["motivation", "habits", "growth"]
    };

    /// <summary>Picks one quote matching the feeling, falling back to a general one. Null only when the catalog itself is empty.</summary>
    public static QuotSeed? PickQuote(IReadOnlyList<QuotSeed> quotes, CompanionEmotion emotion, Random random)
    {
        if (quotes.Count == 0)
        {
            return null;
        }

        string[] themes = QuoteThemes.TryGetValue(emotion, out string[]? mapped) ? mapped : ["general"];
        List<QuotSeed> matching = quotes.Where(q => themes.Contains(q.Theme, StringComparer.OrdinalIgnoreCase)).ToList();
        if (matching.Count == 0)
        {
            matching = quotes.Where(q => string.Equals(q.Theme, "general", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (matching.Count == 0)
        {
            matching = quotes.ToList();
        }

        return matching[random.Next(matching.Count)];
    }
}
