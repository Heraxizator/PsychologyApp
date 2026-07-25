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
    IReadOnlyList<JournalMonthCell> MonthCells,
    DateOnly MonthCursor,
    string MonthTitle,
    IReadOnlyList<JournalYearCell> YearCells,
    int YearCursor,
    string YearTitle,
    JournalCalendarScale CalendarScale,
    long? EditorEntryId,
    DateOnly EditorDay,
    JournalCheckInSlot EditorSlot,
    bool HasMorningEntry,
    bool HasEveningEntry,
    string? EditorNote,
    int SelectedMoodLevel,
    string EditorMoodDisplay,
    string MoodHistorySummary,
    string RangeSubtitle,
    JournalMoodStats Stats,
    string PracticeMoodInsight,
    string OnThisDayLastYearText,
    DateOnly WeekStripEnd,
    IReadOnlyList<JournalActivityInsight> ActivityInsights);

public sealed class JournalMoodLoader(IUserProgressService userProgressService)
{
    private const int TimelineLimit = 500;
    private const int MaxWeekLookbackDays = 84;
    private const int MaxMonthLookbackMonths = 12;
    private const int MaxYearLookbackYears = 2;
    private const int MorningHourCutoff = 15;

    public async Task<JournalMoodSnapshot> LoadAsync(
        int rangeDays = 7,
        DateOnly? filterDay = null,
        DateOnly? editorDay = null,
        DateOnly? weekStripEnd = null,
        DateOnly? monthCursor = null,
        int? yearCursor = null,
        JournalCheckInSlot editorSlot = JournalCheckInSlot.Morning,
        JournalCalendarScale calendarScale = JournalCalendarScale.Week,
        CancellationToken cancellationToken = default)
    {
        int clampedRange = Math.Clamp(rangeDays, 1, 90);
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly rangeStart = today.AddDays(-(clampedRange - 1));
        DateOnly resolvedEditorDay = editorDay ?? filterDay ?? today;
        DateOnly stripEnd = ClampStripEnd(weekStripEnd ?? today, today);
        DateOnly stripStart = stripEnd.AddDays(-6);
        DateOnly resolvedMonth = ClampMonth(monthCursor ?? new DateOnly(today.Year, today.Month, 1), today);
        DateOnly monthStart = new(resolvedMonth.Year, resolvedMonth.Month, 1);
        int resolvedYear = ClampYear(yearCursor ?? today.Year, today);
        DateOnly yearStart = new(resolvedYear, 1, 1);
        DateOnly lastYearSameDay = resolvedEditorDay.AddYears(-1);
        DateOnly fetchStart = stripStart < rangeStart ? stripStart : rangeStart;
        if (calendarScale == JournalCalendarScale.Month && monthStart < fetchStart)
        {
            fetchStart = monthStart;
        }

        if (calendarScale == JournalCalendarScale.Year && yearStart < fetchStart)
        {
            fetchStart = yearStart;
        }

        if ((editorDay is not null || filterDay is not null) && lastYearSameDay < fetchStart)
        {
            fetchStart = lastYearSameDay;
        }

        DateTime fromUtc = fetchStart.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        DateTime toUtcExclusive = today.AddDays(1).ToDateTime(TimeOnly.MinValue).ToUniversalTime();

        IReadOnlyList<MoodEntryDTO> moods =
            await userProgressService.GetMoodsAsync(fromUtc, toUtcExclusive, TimelineLimit, cancellationToken);
        IReadOnlyList<MoodEntryDTO> orderedNewestFirst = moods
            .OrderByDescending(entry => entry.RecordedAt)
            .ToList();

        Dictionary<DateOnly, MoodEntryDTO> byDay = BuildByDay(orderedNewestFirst);
        Dictionary<DateOnly, List<MoodEntryDTO>> dayEntries = BuildDayEntries(orderedNewestFirst);

        List<MoodNoteItem> timeline = FilterForDay(orderedNewestFirst, filterDay)
            .Select(ToTimelineItem)
            .ToList();

        List<MoodChartPoint> points = FilterForDay(orderedNewestFirst, filterDay)
            .OrderBy(entry => entry.RecordedAt)
            .Select(entry => new MoodChartPoint(entry.RecordedAt.ToLocalTime(), entry.MoodLevel))
            .ToList();

        List<JournalTimelineDayGroup> groups = BuildTimelineGroups(timeline);
        List<JournalDayChip> weekDays = calendarScale == JournalCalendarScale.Week
            ? BuildWeekDays(stripEnd, byDay, filterDay ?? editorDay)
            : [];
        List<JournalMonthCell> monthCells = calendarScale == JournalCalendarScale.Month
            ? BuildMonthCells(monthStart, today, byDay, filterDay ?? editorDay)
            : [];
        List<JournalYearCell> yearCells = calendarScale == JournalCalendarScale.Year
            ? BuildYearCells(resolvedYear, today, byDay, filterDay ?? editorDay)
            : [];

        dayEntries.TryGetValue(resolvedEditorDay, out List<MoodEntryDTO>? editorDayList);
        editorDayList ??= [];
        (MoodEntryDTO? morningEntry, MoodEntryDTO? eveningEntry) = SplitDaySlots(editorDayList);
        JournalCheckInSlot resolvedSlot = ResolveEditorSlot(editorSlot, morningEntry, eveningEntry);
        MoodEntryDTO? editorEntry = resolvedSlot == JournalCheckInSlot.Morning ? morningEntry : eveningEntry;

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

        IReadOnlyList<MoodEntryDTO> statsSource = orderedNewestFirst
            .Where(entry =>
            {
                DateOnly day = DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime());
                return day >= rangeStart && day <= today;
            })
            .ToList();
        JournalMoodStats stats = BuildStats(statsSource, today);
        IReadOnlyList<JournalActivityInsight> activityInsights = JournalNoteFactors.Analyze(
            statsSource.Select(entry => (entry.Note, entry.MoodLevel)));

        string practiceInsight = await BuildPracticeMoodInsightAsync(
            rangeStart,
            today,
            byDay,
            cancellationToken);

        string onThisDay = BuildOnThisDayLastYear(lastYearSameDay, dayEntries);

        return new JournalMoodSnapshot(
            points,
            points.Count >= 2,
            AppStrings.ResolveChartSubtitle(points.Count),
            timeline,
            groups,
            weekDays,
            monthCells,
            monthStart,
            AppStrings.JournalMonthTitle(monthStart),
            yearCells,
            resolvedYear,
            AppStrings.JournalYearTitle(resolvedYear),
            calendarScale,
            editorEntryId,
            resolvedEditorDay,
            resolvedSlot,
            morningEntry is not null,
            eveningEntry is not null,
            editorNote,
            selectedLevel,
            editorDisplay,
            historyEntries.Length == 0 ? string.Empty : string.Join(" · ", historyEntries),
            AppStrings.WeekRangeLabel(rangeStart, today),
            stats,
            practiceInsight,
            onThisDay,
            stripEnd,
            activityInsights);
    }

    public async Task SaveMoodAsync(
        int moodLevel,
        string? note,
        long? entryId,
        DateOnly day,
        JournalCheckInSlot slot,
        CancellationToken cancellationToken = default)
    {
        if (entryId is > 0)
        {
            await userProgressService.UpdateMoodEntryAsync(entryId.Value, moodLevel, note, cancellationToken);
            return;
        }

        DateTime recordedAtUtc = ResolveSlotTimestamp(day, slot);
        await userProgressService.RecordMoodAsync(
            moodLevel,
            note,
            recordedAtUtc,
            cancellationToken);
    }

    public Task DeleteMoodAsync(long moodEntryId, CancellationToken cancellationToken = default) =>
        userProgressService.DeleteMoodEntryAsync(moodEntryId, cancellationToken);

    public async Task<IReadOnlyList<MoodEntryDTO>> GetExportMoodsAsync(
        CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly from = today.AddYears(-1);
        DateTime fromUtc = from.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        DateTime toUtcExclusive = today.AddDays(1).ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        return await userProgressService.GetMoodsAsync(fromUtc, toUtcExclusive, TimelineLimit, cancellationToken);
    }

    public static DateOnly ClampStripEnd(DateOnly stripEnd, DateOnly today)
    {
        DateOnly earliest = today.AddDays(-(MaxWeekLookbackDays - 1));
        if (stripEnd > today)
        {
            return today;
        }

        return stripEnd < earliest ? earliest : stripEnd;
    }

    public static DateOnly ClampMonth(DateOnly month, DateOnly today)
    {
        DateOnly currentMonth = new(today.Year, today.Month, 1);
        DateOnly earliest = currentMonth.AddMonths(-(MaxMonthLookbackMonths - 1));
        DateOnly normalized = new(month.Year, month.Month, 1);
        if (normalized > currentMonth)
        {
            return currentMonth;
        }

        return normalized < earliest ? earliest : normalized;
    }

    public static int ClampYear(int year, DateOnly today)
    {
        int current = today.Year;
        int earliest = current - (MaxYearLookbackYears - 1);
        if (year > current)
        {
            return current;
        }

        return year < earliest ? earliest : year;
    }

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

    private async Task<string> BuildPracticeMoodInsightAsync(
        DateOnly rangeStart,
        DateOnly today,
        IReadOnlyDictionary<DateOnly, MoodEntryDTO> byDay,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CompletionDTO> completions;
        try
        {
            completions = await userProgressService.GetRecentTechniqueCompletionsAsync(50, cancellationToken)
                ?? Array.Empty<CompletionDTO>();
        }
        catch
        {
            return string.Empty;
        }

        HashSet<DateOnly> practiceDays = completions
            .Select(completion => DateOnly.FromDateTime(completion.CompletedAt.ToLocalTime()))
            .Where(day => day >= rangeStart && day <= today)
            .ToHashSet();
        if (practiceDays.Count == 0)
        {
            return string.Empty;
        }

        List<int> moodsOnPracticeDays = practiceDays
            .Where(byDay.ContainsKey)
            .Select(day => byDay[day].MoodLevel)
            .ToList();
        if (moodsOnPracticeDays.Count == 0)
        {
            return string.Empty;
        }

        List<int> moodsWithoutPractice = byDay
            .Where(pair => pair.Key >= rangeStart && pair.Key <= today && !practiceDays.Contains(pair.Key))
            .Select(pair => pair.Value.MoodLevel)
            .ToList();

        string averageOnPractice = AppStrings.FormatAverageMood(moodsOnPracticeDays.Average());
        if (moodsWithoutPractice.Count > 0)
        {
            return AppStrings.JournalPracticeMoodCompareInsight(
                practiceDays.Count,
                averageOnPractice,
                AppStrings.FormatAverageMood(moodsWithoutPractice.Average()));
        }

        return AppStrings.JournalPracticeMoodInsight(practiceDays.Count, averageOnPractice);
    }

    private static string BuildOnThisDayLastYear(
        DateOnly lastYearDay,
        IReadOnlyDictionary<DateOnly, List<MoodEntryDTO>> dayEntries)
    {
        if (!dayEntries.TryGetValue(lastYearDay, out List<MoodEntryDTO>? entries) || entries.Count == 0)
        {
            return string.Empty;
        }

        MoodEntryDTO best = entries.OrderByDescending(entry => entry.RecordedAt).First();
        string snippet = JournalNoteFactors.StripFactorLines(best.Note);
        return AppStrings.JournalOnThisDayLastYear(best.MoodLevel, snippet);
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

    private static Dictionary<DateOnly, List<MoodEntryDTO>> BuildDayEntries(IReadOnlyList<MoodEntryDTO> moods) =>
        moods
            .GroupBy(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()))
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(entry => entry.RecordedAt).ToList());

    internal static (MoodEntryDTO? Morning, MoodEntryDTO? Evening) SplitDaySlots(
        IReadOnlyList<MoodEntryDTO> dayEntries)
    {
        if (dayEntries.Count == 0)
        {
            return (null, null);
        }

        List<MoodEntryDTO> ordered = dayEntries.OrderBy(entry => entry.RecordedAt).ToList();
        if (ordered.Count == 1)
        {
            int hour = ordered[0].RecordedAt.ToLocalTime().Hour;
            return hour < MorningHourCutoff
                ? (ordered[0], null)
                : (null, ordered[0]);
        }

        return (ordered[0], ordered[^1]);
    }

    private static JournalCheckInSlot ResolveEditorSlot(
        JournalCheckInSlot requested,
        MoodEntryDTO? morning,
        MoodEntryDTO? evening)
    {
        if (requested == JournalCheckInSlot.Morning && morning is not null)
        {
            return JournalCheckInSlot.Morning;
        }

        if (requested == JournalCheckInSlot.Evening && evening is not null)
        {
            return JournalCheckInSlot.Evening;
        }

        if (requested == JournalCheckInSlot.Morning && morning is null)
        {
            return JournalCheckInSlot.Morning;
        }

        if (requested == JournalCheckInSlot.Evening && evening is null)
        {
            return JournalCheckInSlot.Evening;
        }

        if (morning is not null)
        {
            return JournalCheckInSlot.Morning;
        }

        if (evening is not null)
        {
            return JournalCheckInSlot.Evening;
        }

        return DateTime.Now.Hour < MorningHourCutoff
            ? JournalCheckInSlot.Morning
            : JournalCheckInSlot.Evening;
    }

    private static DateTime ResolveSlotTimestamp(DateOnly day, JournalCheckInSlot slot)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int hourNow = DateTime.Now.Hour;
        if (day == today)
        {
            if (slot == JournalCheckInSlot.Morning && hourNow < MorningHourCutoff)
            {
                return DateTime.UtcNow;
            }

            if (slot == JournalCheckInSlot.Evening && hourNow >= MorningHourCutoff)
            {
                return DateTime.UtcNow;
            }
        }

        TimeOnly time = slot == JournalCheckInSlot.Morning
            ? new TimeOnly(9, 0)
            : new TimeOnly(21, 0);
        return day.ToDateTime(time).ToUniversalTime();
    }

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
            AppStrings.MoodEmojiFor(entry.MoodLevel),
            local.Date == DateTime.Today);
    }

    private static List<JournalDayChip> BuildWeekDays(
        DateOnly stripEnd,
        IReadOnlyDictionary<DateOnly, MoodEntryDTO> byDay,
        DateOnly? selectedDay)
    {
        List<JournalDayChip> chips = [];
        for (int offset = 6; offset >= 0; offset--)
        {
            DateOnly date = stripEnd.AddDays(-offset);
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

    private static List<JournalMonthCell> BuildMonthCells(
        DateOnly monthStart,
        DateOnly today,
        IReadOnlyDictionary<DateOnly, MoodEntryDTO> byDay,
        DateOnly? selectedDay)
    {
        List<JournalMonthCell> cells = [];
        int leading = ((int)monthStart.DayOfWeek + 6) % 7; // Monday-first
        for (int i = 0; i < leading; i++)
        {
            cells.Add(new JournalMonthCell());
        }

        int daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateOnly date = new(monthStart.Year, monthStart.Month, day);
            byDay.TryGetValue(date, out MoodEntryDTO? entry);
            bool enabled = date <= today;
            cells.Add(new JournalMonthCell
            {
                Date = date,
                DayNumber = day.ToString(CultureInfo.InvariantCulture),
                MoodGlyph = entry is null ? (enabled ? "·" : string.Empty) : AppStrings.MoodEmojiFor(entry.MoodLevel),
                HasEntry = entry is not null,
                IsEnabled = enabled,
                IsSelected = selectedDay == date
            });
        }

        return cells;
    }

    private static List<JournalYearCell> BuildYearCells(
        int year,
        DateOnly today,
        IReadOnlyDictionary<DateOnly, MoodEntryDTO> byDay,
        DateOnly? selectedDay)
    {
        DateOnly yearStart = new(year, 1, 1);
        DateOnly yearEnd = new(year, 12, 31);
        List<JournalYearCell> cells = [];
        int leading = ((int)yearStart.DayOfWeek + 6) % 7;
        for (int i = 0; i < leading; i++)
        {
            cells.Add(new JournalYearCell());
        }

        for (DateOnly date = yearStart; date <= yearEnd; date = date.AddDays(1))
        {
            byDay.TryGetValue(date, out MoodEntryDTO? entry);
            bool enabled = date <= today;
            cells.Add(new JournalYearCell
            {
                Date = date,
                MoodGlyph = entry is null
                    ? (enabled ? "·" : string.Empty)
                    : AppStrings.MoodEmojiFor(entry.MoodLevel),
                MoodLevel = entry?.MoodLevel,
                HasEntry = entry is not null,
                IsEnabled = enabled,
                IsSelected = selectedDay == date
            });
        }

        return cells;
    }
}
