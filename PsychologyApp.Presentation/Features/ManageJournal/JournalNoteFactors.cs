using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageJournal;

/// <summary>
/// Factor lines in journal notes use stable prefixes so toggles can add/remove without duplicates.
/// </summary>
public static class JournalNoteFactors
{
    public const string SleepKey = "sleep";
    public const string PeopleKey = "people";
    public const string PracticeKey = "practice";

    public static string GetPrefix(string key) => key switch
    {
        SleepKey => AppStrings.JournalFactorSleep,
        PeopleKey => AppStrings.JournalFactorPeople,
        PracticeKey => AppStrings.JournalFactorPractice,
        _ => string.Empty
    };

    public static string GetLabel(string key) => key switch
    {
        SleepKey => AppStrings.JournalFactorSleepLabel,
        PeopleKey => AppStrings.JournalFactorPeopleLabel,
        PracticeKey => AppStrings.JournalFactorPracticeLabel,
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
        if (HasFactor(note, SleepKey))
        {
            labels.Add(GetLabel(SleepKey));
        }

        if (HasFactor(note, PeopleKey))
        {
            labels.Add(GetLabel(PeopleKey));
        }

        if (HasFactor(note, PracticeKey))
        {
            labels.Add(GetLabel(PracticeKey));
        }

        return labels;
    }
}
