using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageJournal;

/// <summary>
/// Activity/factor lines in journal notes use stable prefixes so toggles can add/remove without duplicates.
/// </summary>
public static class JournalNoteFactors
{
    public const string SleepKey = "sleep";
    public const string PeopleKey = "people";
    public const string PracticeKey = "practice";
    public const string WalkKey = "walk";
    public const string WorkKey = "work";
    public const string SportKey = "sport";
    public const string RestKey = "rest";
    public const string StressKey = "stress";
    public const string HomeKey = "home";

    public static readonly string[] AllKeys =
    [
        SleepKey,
        PeopleKey,
        PracticeKey,
        WalkKey,
        WorkKey,
        SportKey,
        RestKey,
        StressKey,
        HomeKey
    ];

    /// <summary>Hub ritual shows a short primary set; full set remains for Overview/share.</summary>
    public static readonly string[] PrimaryKeys =
    [
        SleepKey,
        PeopleKey,
        PracticeKey,
        WalkKey,
        SportKey,
        RestKey
    ];

    public static string GetPrefix(string key) => key switch
    {
        SleepKey => AppStrings.JournalFactorSleep,
        PeopleKey => AppStrings.JournalFactorPeople,
        PracticeKey => AppStrings.JournalFactorPractice,
        WalkKey => AppStrings.JournalFactorWalk,
        WorkKey => AppStrings.JournalFactorWork,
        SportKey => AppStrings.JournalFactorSport,
        RestKey => AppStrings.JournalFactorRest,
        StressKey => AppStrings.JournalFactorStress,
        HomeKey => AppStrings.JournalFactorHome,
        _ => string.Empty
    };

    public static string GetLabel(string key) => key switch
    {
        SleepKey => AppStrings.JournalFactorSleepLabel,
        PeopleKey => AppStrings.JournalFactorPeopleLabel,
        PracticeKey => AppStrings.JournalFactorPracticeLabel,
        WalkKey => AppStrings.JournalFactorWalkLabel,
        WorkKey => AppStrings.JournalFactorWorkLabel,
        SportKey => AppStrings.JournalFactorSportLabel,
        RestKey => AppStrings.JournalFactorRestLabel,
        StressKey => AppStrings.JournalFactorStressLabel,
        HomeKey => AppStrings.JournalFactorHomeLabel,
        _ => string.Empty
    };

    public static bool HasFactor(string? note, string key)
    {
        string prefix = GetPrefix(key);
        if (string.IsNullOrEmpty(prefix) || string.IsNullOrWhiteSpace(note))
        {
            return false;
        }

        return note.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(line => line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    public static string ToggleFactor(string? note, string key)
    {
        string prefix = GetPrefix(key);
        if (string.IsNullOrEmpty(prefix))
        {
            return note?.Trim() ?? string.Empty;
        }

        List<string> lines = string.IsNullOrWhiteSpace(note)
            ? []
            : note.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

        int existingIndex = lines.FindIndex(line =>
            line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        if (existingIndex >= 0)
        {
            lines.RemoveAt(existingIndex);
        }
        else
        {
            lines.Add(prefix);
        }

        return string.Join(Environment.NewLine, lines);
    }

    public static IReadOnlyList<string> ExtractActiveLabels(string? note)
    {
        List<string> labels = [];
        foreach (string key in AllKeys)
        {
            if (HasFactor(note, key))
            {
                labels.Add(GetLabel(key));
            }
        }

        return labels;
    }

    public static string StripFactorLines(string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            return string.Empty;
        }

        string[] prefixes = AllKeys
            .Select(GetPrefix)
            .Where(prefix => !string.IsNullOrEmpty(prefix))
            .ToArray();

        return string.Join(
            Environment.NewLine,
            note.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(line => !prefixes.Any(prefix =>
                    line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))));
    }

    public static IReadOnlyList<JournalActivityInsight> Analyze(
        IEnumerable<(string? Note, int MoodLevel)> entries)
    {
        List<(string? Note, int MoodLevel)> material = entries.ToList();
        List<JournalActivityInsight> insights = [];
        foreach (string key in AllKeys)
        {
            List<int> moods = material
                .Where(entry => HasFactor(entry.Note, key))
                .Select(entry => entry.MoodLevel)
                .ToList();
            if (moods.Count == 0)
            {
                continue;
            }

            string? average = moods.Count >= 2
                ? AppStrings.FormatAverageMood(moods.Average())
                : null;
            insights.Add(new JournalActivityInsight(
                key,
                GetLabel(key),
                moods.Count,
                average));
        }

        return insights;
    }

    public static JournalFactorCounts CountInNotes(IEnumerable<string?> notes)
    {
        IReadOnlyList<JournalActivityInsight> insights = Analyze(
            notes.Select(note => (note, 0)));
        int Get(string key) => insights.FirstOrDefault(item => item.Key == key)?.Count ?? 0;
        return new JournalFactorCounts(
            Get(SleepKey),
            Get(PeopleKey),
            Get(PracticeKey),
            Get(WalkKey),
            Get(WorkKey),
            Get(SportKey),
            Get(RestKey),
            Get(StressKey),
            Get(HomeKey));
    }
}

public sealed record JournalActivityInsight(
    string Key,
    string Label,
    int Count,
    string? AverageMoodDisplay)
{
    public string DisplayPill => AverageMoodDisplay is null
        ? AppStrings.JournalFactorCountPill(Label, Count)
        : AppStrings.JournalActivityCorrelationPill(Label, Count, AverageMoodDisplay);
}

public sealed record JournalFactorCounts(
    int Sleep,
    int People,
    int Practice,
    int Walk = 0,
    int Work = 0,
    int Sport = 0,
    int Rest = 0,
    int Stress = 0,
    int Home = 0)
{
    public bool HasAny =>
        Sleep > 0 || People > 0 || Practice > 0 || Walk > 0 || Work > 0
        || Sport > 0 || Rest > 0 || Stress > 0 || Home > 0;
}
