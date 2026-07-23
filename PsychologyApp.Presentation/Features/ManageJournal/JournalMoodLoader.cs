using System.Globalization;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageJournal;

public sealed record JournalMoodStats(
    int CheckInCount,
    string AverageMoodDisplay,
    string MoodStreakDisplay,
    string MoodTrendLabel,
    string BestWorstLabel,
    bool HasStats);

public sealed record JournalMoodSnapshot(
    IReadOnlyList<MoodChartPoint> ChartPoints,
    bool HasTrendChart,
    string ChartSubtitle,
    IReadOnlyList<MoodNoteItem> TimelineEntries,
    IReadOnlyList<JournalTimelineDayGroup> TimelineGroups,
    IReadOnlyList<JournalDayChip> WeekDays,
    long? EditorEntryId,
    DateOnly EditorDay,
    string? EditorNote,
    int SelectedMoodLevel,
    string EditorMoodDisplay,
    string MoodHistorySummary,
    string RangeSubtitle,
    JournalMoodStats Stats);

public sealed class JournalMoodLoader(IUserProgressService userProgressService)
{
    private const int TimelineLimit = 120;

    public async Task<JournalMoodSnapshot> LoadAsync(
        int rangeDays = 7,
        DateOnly? filterDay = null,
        DateOnly? editorDay = null,
        CancellationToken cancellationToken = default)
    {
        int clampedRange = Math.Clamp(rangeDays, 1, 90);
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly rangeStart = today.AddDays(-(clampedRange - 1));
        DateOnly resolvedEditorDay = editorDay ?? filterDay ?? today;
        DateTime fromUtc = rangeStart.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        DateTime toUtcExclusive = today.AddDays(1).ToDateTime(TimeOnly.MinValue).ToUniversalTime();

        IReadOnlyList<MoodEntryDTO> moods =
            await userProgressService.GetMoodsAsync(fromUtc, toUtcExclusive, TimelineLimit, cancellationToken);
        IReadOnlyList<MoodEntryDTO> orderedNewestFirst = moods
            .OrderByDescending(entry => entry.RecordedAt)
            .ToList();

        Dictionary<DateOnly, MoodEntryDTO> byDay = BuildByDay(orderedNewestFirst);

        List<MoodNoteItem> timeline = FilterForDay(orderedNewestFirst, filterDay)
            .Select(ToTimelineItem)
            .ToList();

        List<MoodChartPoint> points = FilterForDay(orderedNewestFirst, filterDay)
            .OrderBy(entry => entry.RecordedAt)
            .Select(entry => new MoodChartPoint(entry.RecordedAt.ToLocalTime(), entry.MoodLevel))
            .ToList();

        List<JournalTimelineDayGroup> groups = BuildTimelineGroups(timeline);
        List<JournalDayChip> weekDays = BuildWeekDays(today, byDay, filterDay ?? editorDay);

        byDay.TryGetValue(resolvedEditorDay, out MoodEntryDTO? editorEntry);
        long? editorEntryId = editorEntry?.MoodEntryId;
        string? editorNote = string.IsNullOrWhiteSpace(editorEntry?.Note) ? null : editorEntry!.Note!.Trim();
        int selectedLevel = editorEntry?.MoodLevel ?? 0;
        string editorDisplay = editorEntry is null
            ? (resolvedEditorDay == today ? string.Empty : AppStrings.JournalDayEmptyHint(resolvedEditorDay))
            : resolvedEditorDay == today
                ? AppStrings.TodayMoodLine(editorEntry.MoodLevel, 5)
                : AppStrings.JournalDayMoodLine(resolvedEditorDay, editorEntry.MoodLevel, 5);

        IEnumerable<MoodEntryDTO> summarySource = editorEntry is null
            ? orderedNewestFirst
            : orderedNewestFirst.Where(entry => entry.MoodEntryId != editorEntry.MoodEntryId);
        string[] historyEntries = summarySource
            .Take(2)
            .Select(mood => AppStrings.MoodHistoryEntry(mood.RecordedAt.ToLocalTime().ToString("d"), mood.MoodLevel, 5))
            .ToArray();

        JournalMoodStats stats = BuildStats(orderedNewestFirst, today);

        return new JournalMoodSnapshot(
            points,
            points.Count >= 2,
            AppStrings.ResolveChartSubtitle(points.Count),
            timeline,
            groups,
            weekDays,
            editorEntryId,
            resolvedEditorDay,
            editorNote,
            selectedLevel,
            editorDisplay,
            historyEntries.Length == 0 ? string.Empty : string.Join(" · ", historyEntries),
            AppStrings.WeekRangeLabel(rangeStart, today),
            stats);
    }

    public async Task SaveMoodAsync(
        int moodLevel,
        string? note,
        long? entryId,
        DateOnly day,
        CancellationToken cancellationToken = default)
    {
        if (entryId is > 0)
        {
            await userProgressService.UpdateMoodEntryAsync(entryId.Value, moodLevel, note, cancellationToken);
            return;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateTime recordedAtUtc = day == today
            ? DateTime.UtcNow
            : day.ToDateTime(new TimeOnly(23, 59, 59)).ToUniversalTime();

        await userProgressService.RecordMoodAsync(
            moodLevel,
            note,
            recordedAtUtc,
            cancellationToken);
    }

    public Task DeleteMoodAsync(long moodEntryId, CancellationToken cancellationToken = default) =>
        userProgressService.DeleteMoodEntryAsync(moodEntryId, cancellationToken);

    public static IReadOnlyList<JournalTimelineDayGroup> FilterGroupsByNoteSearch(
        IReadOnlyList<JournalTimelineDayGroup> groups,
        string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return groups;
        }

        string needle = query.Trim();
        return groups
            .Select(group =>
            {
                List<MoodNoteItem> matched = group.Entries
                    .Where(entry => entry.HasNote &&
                                    entry.NoteText.Contains(needle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                return matched.Count == 0
                    ? null
                    : group with { Entries = matched };
            })
            .Where(group => group is not null)
            .Select(group => group!)
            .ToList();
    }

    private static List<JournalTimelineDayGroup> BuildTimelineGroups(IReadOnlyList<MoodNoteItem> timeline) =>
        timeline
            .GroupBy(entry => entry.Day)
            .OrderByDescending(group => group.Key)
            .Select(group => new JournalTimelineDayGroup(
                group.Key,
                AppStrings.JournalEditorDayTitle(group.Key),
                group.ToList()))
            .ToList();

    private static JournalMoodStats BuildStats(IReadOnlyList<MoodEntryDTO> moods, DateOnly today)
    {
        if (moods.Count == 0)
        {
            return new JournalMoodStats(
                0,
                AppStrings.MetricEmptyValue,
                AppStrings.MetricEmptyValue,
                string.Empty,
                string.Empty,
                false);
        }

        double average = moods.Average(entry => entry.MoodLevel);
        int streak = ComputeMoodStreak(moods, today);
        string trend = ResolveMoodTrend(moods.OrderBy(entry => entry.RecordedAt).ToList());
        int best = moods.Max(entry => entry.MoodLevel);
        int worst = moods.Min(entry => entry.MoodLevel);
        string bestWorst = AppStrings.JournalBestWorstPill(best, worst);

        return new JournalMoodStats(
            moods.Count,
            AppStrings.FormatAverageMood(average),
            streak > 0 ? streak.ToString() : AppStrings.MetricEmptyValue,
            trend,
            bestWorst,
            true);
    }

    private static int ComputeMoodStreak(IReadOnlyList<MoodEntryDTO> moods, DateOnly today)
    {
        HashSet<DateOnly> daysWithMood = moods
            .Select(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()))
            .ToHashSet();

        int streak = 0;
        DateOnly cursor = today;
        if (!daysWithMood.Contains(today))
        {
            cursor = today.AddDays(-1);
        }

        while (daysWithMood.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    private static string ResolveMoodTrend(IReadOnlyList<MoodEntryDTO> orderedOldestFirst)
    {
        if (orderedOldestFirst.Count == 0)
        {
            return string.Empty;
        }

        if (orderedOldestFirst.Count == 1)
        {
            return AppStrings.MoodTrendFlat;
        }

        int first = orderedOldestFirst[0].MoodLevel;
        int last = orderedOldestFirst[^1].MoodLevel;
        if (last > first)
        {
            return AppStrings.MoodTrendUp;
        }

        if (last < first)
        {
            return AppStrings.MoodTrendDown;
        }

        return AppStrings.MoodTrendFlat;
    }

    private static Dictionary<DateOnly, MoodEntryDTO> BuildByDay(IReadOnlyList<MoodEntryDTO> moods) =>
        moods
            .GroupBy(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()))
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(entry => entry.RecordedAt).First());

    private static IEnumerable<MoodEntryDTO> FilterForDay(
        IEnumerable<MoodEntryDTO> moods,
        DateOnly? filterDay)
    {
        if (filterDay is null)
        {
            return moods;
        }

        DateOnly day = filterDay.Value;
        return moods.Where(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()) == day);
    }

    private static MoodNoteItem ToTimelineItem(MoodEntryDTO entry)
    {
        DateTime local = entry.RecordedAt.ToLocalTime();
        bool hasNote = !string.IsNullOrWhiteSpace(entry.Note);
        return new MoodNoteItem(
            entry.MoodEntryId,
            DateOnly.FromDateTime(local),
            local.ToString("d"),
            local.ToString("t"),
            hasNote ? entry.Note!.Trim() : AppStrings.JournalNoNoteCaption,
            hasNote,
            entry.MoodLevel,
            AppStrings.MoodLevelPill(entry.MoodLevel),
            local.Date == DateTime.Today);
    }

    private static List<JournalDayChip> BuildWeekDays(
        DateOnly today,
        IReadOnlyDictionary<DateOnly, MoodEntryDTO> byDay,
        DateOnly? selectedDay)
    {
        List<JournalDayChip> chips = [];
        for (int offset = 6; offset >= 0; offset--)
        {
            DateOnly date = today.AddDays(-offset);
            byDay.TryGetValue(date, out MoodEntryDTO? entry);
            chips.Add(new JournalDayChip
            {
                Date = date,
                DayLabel = date.ToDateTime(TimeOnly.MinValue)
                    .ToString("ddd", CultureInfo.CurrentCulture),
                MoodGlyph = entry is null ? "·" : AppStrings.MoodEmojiFor(entry.MoodLevel),
                MoodLevel = entry?.MoodLevel,
                HasEntry = entry is not null,
                IsSelected = selectedDay == date
            });
        }

        return chips;
    }
}
