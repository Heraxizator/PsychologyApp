using System.Globalization;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Topics that keep coming back across chats ("the third talk about the boss this month"). A topic is the person talked about when
/// there is one, otherwise the area of life; each chat counts once, by the date it first came up. Kept in the chat memory as
/// <c>topic:&lt;key&gt;</c> with the dates (UTC, ISO), only on the device, and cleared with "forget".
/// </summary>
public static class RecurringTopics
{
    public const string MemoryPrefix = "topic:";

    /// <summary>Chats older than this do not count: a topic from months ago is not "coming back".</summary>
    public static readonly TimeSpan Window = TimeSpan.FromDays(30);

    /// <summary>With this many earlier chats on a topic, the current one is named as a repeat: the third talk is a pattern, the second may be chance.</summary>
    public const int EarlierChatsForPattern = 2;

    private const int MaxDates = 8;

    /// <summary>A person is more specific than an area of life ("the boss" rather than "work"); null when the chat has neither.</summary>
    public static string? Key(CompanionTheme? theme, CompanionPerson? person) =>
        person is { } who ? "person:" + who : theme is { } area ? "theme:" + area : null;

    public static string MemoryKey(string topicKey) => MemoryPrefix + topicKey;

    /// <summary>The memory value with one more chat on the topic, keeping the latest dates only.</summary>
    public static string Record(string? stored, DateTime nowUtc) =>
        string.Join(',', Dates(stored).Append(nowUtc).TakeLast(MaxDates).Select(date => date.ToString("O", CultureInfo.InvariantCulture)));

    /// <summary>For every remembered topic, how many chats in the window it came up in.</summary>
    public static IReadOnlyDictionary<string, int> EarlierChats(IReadOnlyDictionary<string, string> memory, DateTime nowUtc)
    {
        Dictionary<string, int> counts = new(StringComparer.Ordinal);
        foreach ((string key, string value) in memory)
        {
            if (key.StartsWith(MemoryPrefix, StringComparison.Ordinal))
            {
                int recent = Dates(value).Count(date => nowUtc - date <= Window);
                if (recent > 0)
                {
                    counts[key[MemoryPrefix.Length..]] = recent;
                }
            }
        }

        return counts;
    }

    /// <summary>How the topic is named in a sentence: the person or the area of life.</summary>
    public static string? Label(string topicKey, bool english) => topicKey.Split(':') switch
    {
        ["person", var who] when Enum.TryParse(who, out CompanionPerson person) && Enum.IsDefined(person) => CompanionActContent.PersonName(person, english),
        ["theme", var area] when Enum.TryParse(area, out CompanionTheme theme) && Enum.IsDefined(theme) => CompanionContent.ThemeName(theme, english),
        _ => null
    };

    // A damaged value loses only the dates it cannot read: the memory is the person's own, never a reason to fail a turn.
    private static IEnumerable<DateTime> Dates(string? stored) =>
        (stored ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(text => DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime date) ? date.ToUniversalTime() : (DateTime?)null)
            .OfType<DateTime>();
}
