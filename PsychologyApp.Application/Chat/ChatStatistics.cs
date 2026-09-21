using System.Globalization;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>Keys of what the companion remembers between chats (see <see cref="Abstractions.Persistence.IChatRepository.GetMemoryAsync"/>).</summary>
public static class ChatMemoryKeys
{
    public const string Name = "name";
    public const string TriedPrefix = "tried:";
    public const string HelpedPrefix = "helped:";

    public static string Tried(TechniqueId id) => TriedPrefix + id;

    public static string Helped(TechniqueId id) => HelpedPrefix + id;

    /// <summary>The practice with the most "it helped" marks; the first by name on a tie so the choice is stable.</summary>
    public static TechniqueId? MostHelped(IReadOnlyDictionary<string, string> memory) =>
        Counters(memory, HelpedPrefix)
            .Where(c => c.Count > 0)
            .OrderByDescending(c => c.Count)
            .ThenBy(c => c.Technique.ToString(), StringComparer.Ordinal)
            .Select(c => (TechniqueId?)c.Technique)
            .FirstOrDefault();

    internal static IEnumerable<(TechniqueId Technique, int Count)> Counters(IReadOnlyDictionary<string, string> memory, string prefix)
    {
        foreach ((string key, string value) in memory)
        {
            if (key.StartsWith(prefix, StringComparison.Ordinal)
                && Enum.TryParse(key[prefix.Length..], out TechniqueId id)
                && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count))
            {
                yield return (id, count);
            }
        }
    }
}

public sealed record PracticeStat(TechniqueId Technique, int Tried, int Helped);

/// <param name="Share">0..1: part of the chats with a recognised feeling.</param>
public sealed record EmotionStat(CompanionEmotion Emotion, int Chats, double Share);

public sealed record TensionPoint(DateTime AtUtc, int Before, int After);

/// <summary>How far the relationship with the companion has come, from the number of messages the person wrote.</summary>
/// <param name="Progress">0..1 towards the next level; 1 at the last one.</param>
public sealed record TrustLevel(int Index, double Progress, int MessagesToNext);

/// <summary>Everything the companion's profile screen shows. Computed from stored chats and memory, never stored itself.</summary>
public sealed record CompanionProfile(
    string? UserName,
    int Chats,
    int UserMessages,
    int Days,
    int StreakDays,
    DateTime? SinceUtc,
    int PracticesTried,
    int PracticesHelped,
    IReadOnlyList<PracticeStat> Practices,
    IReadOnlyList<EmotionStat> Emotions,
    IReadOnlyList<TensionPoint> Tension,
    int TensionMeasured,
    int TensionImproved,
    double? AverageDrop,
    TrustLevel Trust)
{
    public bool HasHistory => Chats > 0;

    public static CompanionProfile Empty { get; } = ChatStatistics.Compute([], new Dictionary<string, string>(), DateTime.UtcNow, TimeZoneInfo.Utc);
}

public static class ChatStatistics
{
    public const int MaxTensionPoints = 8;
    public const int MaxEmotions = 4;

    /// <summary>Messages needed to reach each trust level after the first (level 0 is "just met").</summary>
    private static readonly int[] TrustThresholds = [0, 5, 25, 80];

    public static CompanionProfile Compute(
        IReadOnlyList<ChatSessionDTO> sessions,
        IReadOnlyDictionary<string, string> memory,
        DateTime nowUtc,
        TimeZoneInfo zone)
    {
        List<ChatSessionDTO> chats = sessions.Where(s => s.HasConversation()).ToList();
        int userMessages = chats.Sum(s => s.UserMessageCount);

        HashSet<DateOnly> activeDays = [];
        foreach (ChatSessionDTO chat in chats)
        {
            activeDays.Add(LocalDay(chat.CreatedAt, zone));
            activeDays.Add(LocalDay(chat.UpdatedAt, zone));
        }

        List<PracticeStat> practices = ChatMemoryKeys.Counters(memory, ChatMemoryKeys.TriedPrefix).Select(c => c.Technique)
            .Union(ChatMemoryKeys.Counters(memory, ChatMemoryKeys.HelpedPrefix).Select(c => c.Technique))
            .Select(id => new PracticeStat(id, Count(memory, ChatMemoryKeys.Tried(id)), Count(memory, ChatMemoryKeys.Helped(id))))
            .OrderByDescending(p => p.Helped)
            .ThenByDescending(p => p.Tried)
            .ThenBy(p => p.Technique.ToString(), StringComparer.Ordinal)
            .ToList();

        List<EmotionStat> emotions = Emotions(chats);

        List<ChatSessionDTO> measured = chats
            .Where(s => s.FirstIntensity is not null && s.LastIntensity is not null)
            .OrderBy(s => s.CreatedAt)
            .ToList();
        List<TensionPoint> tension = measured
            .TakeLast(MaxTensionPoints)
            .Select(s => new TensionPoint(s.CreatedAt, s.FirstIntensity!.Value, s.LastIntensity!.Value))
            .ToList();
        int improved = measured.Count(s => s.LastIntensity < s.FirstIntensity);
        double? drop = measured.Count == 0 ? null : measured.Average(s => s.FirstIntensity!.Value - s.LastIntensity!.Value);

        memory.TryGetValue(ChatMemoryKeys.Name, out string? name);

        return new CompanionProfile(
            string.IsNullOrWhiteSpace(name) ? null : name,
            chats.Count,
            userMessages,
            activeDays.Count,
            Streak(activeDays, LocalDay(nowUtc, zone)),
            chats.Count == 0 ? null : chats.Min(s => s.CreatedAt),
            practices.Sum(p => p.Tried),
            practices.Sum(p => p.Helped),
            practices,
            emotions,
            tension,
            measured.Count,
            improved,
            drop,
            Trust(userMessages));
    }

    public static TrustLevel Trust(int userMessages)
    {
        int level = 0;
        for (int i = 1; i < TrustThresholds.Length; i++)
        {
            if (userMessages >= TrustThresholds[i])
            {
                level = i;
            }
        }

        if (level == TrustThresholds.Length - 1)
        {
            return new TrustLevel(level, 1, 0);
        }

        int from = TrustThresholds[level];
        int to = TrustThresholds[level + 1];
        return new TrustLevel(level, (double)(userMessages - from) / (to - from), to - userMessages);
    }

    private static List<EmotionStat> Emotions(List<ChatSessionDTO> chats)
    {
        List<CompanionEmotion> known = chats
            .Select(s => Enum.TryParse(s.Emotion, out CompanionEmotion e) ? e : CompanionEmotion.Unknown)
            .Where(e => e != CompanionEmotion.Unknown)
            .ToList();
        return known
            .GroupBy(e => e)
            .Select(g => new EmotionStat(g.Key, g.Count(), (double)g.Count() / known.Count))
            .OrderByDescending(e => e.Chats)
            .ThenBy(e => e.Emotion)
            .Take(MaxEmotions)
            .ToList();
    }

    /// <summary>Consecutive days with a conversation, counted back from today, or from yesterday when today has none yet.</summary>
    private static int Streak(HashSet<DateOnly> days, DateOnly today)
    {
        DateOnly cursor = days.Contains(today) ? today : today.AddDays(-1);
        int streak = 0;
        while (days.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    private static DateOnly LocalDay(DateTime utc, TimeZoneInfo zone) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone));

    private static int Count(IReadOnlyDictionary<string, string> memory, string key) =>
        memory.TryGetValue(key, out string? value) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n) ? n : 0;
}
