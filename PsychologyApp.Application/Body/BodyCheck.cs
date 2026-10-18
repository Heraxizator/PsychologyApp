using System.Text.Json.Serialization;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Body;

// Stored by name, not by number, so adding a value later never turns old entries into different ones; a name this version does not
// know reads as undefined (see LenientEnumConverter) instead of failing the log or a backup.
[JsonConverter(typeof(LenientEnumConverter<BodyArea>))]
public enum BodyArea
{
    Head,
    Face,
    Throat,
    Shoulders,
    Chest,
    Stomach,
    Back,
    Arms,
    Legs,
    WholeBody
}

[JsonConverter(typeof(LenientEnumConverter<BodySensation>))]
public enum BodySensation
{
    Tight,
    Heavy,
    Heat,
    Cold,
    Lump,
    Trembling,
    Empty,
    Fluttering,
    Tingling,
    Pain
}

[JsonConverter(typeof(LenientEnumConverter<BodyFeeling>))]
public enum BodyFeeling
{
    Anxiety,
    Fear,
    Anger,
    Hurt,
    Sadness,
    Shame,
    Guilt,
    Exhaustion,
    Joy,
    Unknown
}

/// <param name="Intensity">0..10, or null when the person did not rate it.</param>
/// <param name="Feelings">What the person recognised in the sensation; may be empty when they did not know.</param>
/// <param name="Note">The person's own words: kept only in the encrypted database and the backup.</param>
public sealed record BodyEntry(
    string Id,
    DateTime CreatedUtc,
    BodyArea Area,
    BodySensation Sensation,
    int? Intensity,
    IReadOnlyList<BodyFeeling> Feelings,
    string? Note)
{
    /// <summary>Both the place and the sensation are ones this version knows: only such entries are shown and counted.</summary>
    public bool IsReadable() => Enum.IsDefined(Area) && Enum.IsDefined(Sensation);
}

/// <summary>All body check-ins, newest last. One small document, read and written whole.</summary>
public sealed record BodyLog
{
    public IReadOnlyList<BodyEntry> Entries { get; init; } = [];

    /// <summary>
    /// The entries this version can show. Entries from a newer version stay in the log, so going back to that version loses nothing.
    /// </summary>
    public IReadOnlyList<BodyEntry> Readable() => Entries.Where(entry => entry.IsReadable()).ToList();
}

/// <summary>
/// What a sensation may be telling. Not a diagnosis and not a table of causes: the feelings are guesses for the person to check, ranked
/// by where people most often feel them (bodily maps of emotions, Nummenmaa et al., PNAS 2014) and by the kind of sensation.
/// </summary>
public static class BodyRules
{
    /// <summary>At most this many guesses are offered; the person can pick any other feeling.</summary>
    public const int MaxGuesses = 3;

    private static readonly IReadOnlyDictionary<BodyArea, (BodyFeeling Feeling, int Weight)[]> ByArea = new Dictionary<BodyArea, (BodyFeeling, int)[]>
    {
        [BodyArea.Head] = [(BodyFeeling.Anxiety, 2), (BodyFeeling.Exhaustion, 2), (BodyFeeling.Anger, 1)],
        [BodyArea.Face] = [(BodyFeeling.Shame, 3), (BodyFeeling.Anger, 2), (BodyFeeling.Anxiety, 1)],
        [BodyArea.Throat] = [(BodyFeeling.Sadness, 3), (BodyFeeling.Hurt, 3), (BodyFeeling.Anxiety, 2)],
        [BodyArea.Shoulders] = [(BodyFeeling.Exhaustion, 3), (BodyFeeling.Anxiety, 2), (BodyFeeling.Anger, 1)],
        [BodyArea.Chest] = [(BodyFeeling.Anxiety, 3), (BodyFeeling.Sadness, 2), (BodyFeeling.Fear, 2), (BodyFeeling.Hurt, 1)],
        [BodyArea.Stomach] = [(BodyFeeling.Anxiety, 3), (BodyFeeling.Fear, 3), (BodyFeeling.Joy, 1)],
        [BodyArea.Back] = [(BodyFeeling.Exhaustion, 3), (BodyFeeling.Anxiety, 1)],
        [BodyArea.Arms] = [(BodyFeeling.Anger, 3), (BodyFeeling.Fear, 1), (BodyFeeling.Exhaustion, 1)],
        [BodyArea.Legs] = [(BodyFeeling.Fear, 2), (BodyFeeling.Anxiety, 2), (BodyFeeling.Exhaustion, 2)],
        [BodyArea.WholeBody] = [(BodyFeeling.Exhaustion, 2), (BodyFeeling.Fear, 2), (BodyFeeling.Anxiety, 2), (BodyFeeling.Sadness, 1)]
    };

    private static readonly IReadOnlyDictionary<BodySensation, (BodyFeeling Feeling, int Weight)[]> BySensation = new Dictionary<BodySensation, (BodyFeeling, int)[]>
    {
        [BodySensation.Tight] = [(BodyFeeling.Anxiety, 2), (BodyFeeling.Anger, 1), (BodyFeeling.Fear, 1)],
        [BodySensation.Heavy] = [(BodyFeeling.Sadness, 3), (BodyFeeling.Exhaustion, 3), (BodyFeeling.Guilt, 1)],
        [BodySensation.Heat] = [(BodyFeeling.Anger, 3), (BodyFeeling.Shame, 2)],
        [BodySensation.Cold] = [(BodyFeeling.Fear, 3), (BodyFeeling.Sadness, 1)],
        [BodySensation.Lump] = [(BodyFeeling.Sadness, 2), (BodyFeeling.Hurt, 2), (BodyFeeling.Anxiety, 1)],
        [BodySensation.Trembling] = [(BodyFeeling.Fear, 3), (BodyFeeling.Anger, 1), (BodyFeeling.Anxiety, 1)],
        [BodySensation.Empty] = [(BodyFeeling.Sadness, 3), (BodyFeeling.Exhaustion, 1)],
        [BodySensation.Fluttering] = [(BodyFeeling.Anxiety, 2), (BodyFeeling.Joy, 2)],
        [BodySensation.Tingling] = [(BodyFeeling.Anxiety, 2), (BodyFeeling.Anger, 1)],
        [BodySensation.Pain] = [(BodyFeeling.Exhaustion, 1), (BodyFeeling.Anxiety, 1), (BodyFeeling.Hurt, 1)]
    };

    /// <summary>The feelings most often felt this way in this place, best first. Ties keep the order of <see cref="BodyFeeling"/>.</summary>
    public static IReadOnlyList<BodyFeeling> Guesses(BodyArea area, BodySensation sensation)
    {
        Dictionary<BodyFeeling, int> score = [];
        foreach ((BodyFeeling feeling, int weight) in ByArea[area].Concat(BySensation[sensation]))
        {
            score[feeling] = score.GetValueOrDefault(feeling) + weight;
        }

        return score
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key)
            .Take(MaxGuesses)
            .Select(pair => pair.Key)
            .ToList();
    }

    /// <summary>
    /// Pain, or any sensation at 8 and above, gets the doctor first: a body signal can be an illness, and reading it as a feeling must
    /// never delay that check.
    /// </summary>
    public static bool AsksForDoctor(BodySensation sensation, int? intensity) => sensation == BodySensation.Pain || intensity >= 8;

    /// <summary>Practices that work with what was found: body ones for a sensation alone, a kinder or calmer one for the feeling.</summary>
    public static IReadOnlyList<TechniqueId> Practices(IReadOnlyCollection<BodyFeeling> feelings)
    {
        List<TechniqueId> practices = [TechniqueId.BodyScan];
        foreach (BodyFeeling feeling in feelings)
        {
            practices.AddRange(feeling switch
            {
                BodyFeeling.Anxiety or BodyFeeling.Fear => [TechniqueId.Breathing, TechniqueId.Grounding],
                BodyFeeling.Anger => [TechniqueId.Breathing, TechniqueId.Observer],
                BodyFeeling.Hurt or BodyFeeling.Sadness or BodyFeeling.Shame or BodyFeeling.Guilt => [TechniqueId.SelfCompassion, TechniqueId.ThoughtRecord],
                BodyFeeling.Exhaustion => [TechniqueId.SmallStep],
                _ => []
            });
        }

        return practices.Distinct().Take(3).ToList();
    }
}

public enum PartOfDay
{
    Morning,
    Day,
    Evening,
    Night
}

/// <param name="Count">How many check-ins had this place and sensation.</param>
/// <param name="Feeling">The feeling the person recognised most often in it, or null when none was chosen.</param>
/// <param name="MostlyAt">The part of the day most of them fell on, or null when they are spread out.</param>
public sealed record BodyPattern(BodyArea Area, BodySensation Sensation, int Count, BodyFeeling? Feeling, PartOfDay? MostlyAt);

/// <summary>The person's own regularities, found in their check-ins rather than in a ready table.</summary>
public static class BodyPatterns
{
    /// <summary>A pair seen fewer times is not a pattern yet.</summary>
    public const int MinRepeats = 2;

    public const int MaxPatterns = 3;

    /// <summary>A part of the day counts when at least this share of the check-ins fell on it.</summary>
    private const double MostlyShare = 0.6;

    public static IReadOnlyList<BodyPattern> Find(IReadOnlyList<BodyEntry> entries, TimeZoneInfo zone) =>
        entries
            .Where(entry => entry.IsReadable())
            .GroupBy(entry => (entry.Area, entry.Sensation))
            .Where(group => group.Count() >= MinRepeats)
            .Select(group => new BodyPattern(
                group.Key.Area,
                group.Key.Sensation,
                group.Count(),
                TopFeeling(group),
                MostlyAt(group.Select(entry => PartOf(entry.CreatedUtc, zone)).ToList())))
            .OrderByDescending(pattern => pattern.Count)
            .ThenBy(pattern => pattern.Area)
            .ThenBy(pattern => pattern.Sensation)
            .Take(MaxPatterns)
            .ToList();

    public static PartOfDay PartOf(DateTime utc, TimeZoneInfo zone) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone).Hour switch
        {
            >= 5 and < 12 => PartOfDay.Morning,
            >= 12 and < 17 => PartOfDay.Day,
            >= 17 and < 23 => PartOfDay.Evening,
            _ => PartOfDay.Night
        };

    private static BodyFeeling? TopFeeling(IEnumerable<BodyEntry> entries) =>
        entries
            .SelectMany(entry => entry.Feelings)
            .Where(feeling => feeling != BodyFeeling.Unknown && Enum.IsDefined(feeling))
            .GroupBy(feeling => feeling)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            .Select(group => (BodyFeeling?)group.Key)
            .FirstOrDefault();

    private static PartOfDay? MostlyAt(IReadOnlyList<PartOfDay> parts) =>
        parts
            .GroupBy(part => part)
            .Where(group => group.Count() >= parts.Count * MostlyShare)
            .Select(group => (PartOfDay?)group.Key)
            .FirstOrDefault();
}
