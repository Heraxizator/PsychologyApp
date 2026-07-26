using System.Globalization;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Journal;

namespace PsychologyApp.Presentation.Features.ManageJournal;

public static class JournalCalendarBuilder
{
    public static List<JournalDayChip> BuildWeekDays(
        DateOnly stripEnd,
        IReadOnlyDictionary<DateOnly, int> moodLevelByDay,
        DateOnly? selectedDay,
        CultureInfo? culture = null)
    {
        CultureInfo labelCulture = culture ?? CultureInfo.CurrentCulture;
        List<JournalDayChip> chips = [];
        for (int offset = 6; offset >= 0; offset--)
        {
            DateOnly date = stripEnd.AddDays(-offset);
            bool hasEntry = moodLevelByDay.TryGetValue(date, out int moodLevel);
            chips.Add(new JournalDayChip
            {
                Date = date,
                DayLabel = date.ToDateTime(TimeOnly.MinValue).ToString("ddd", labelCulture),
                MoodGlyph = hasEntry ? AppStrings.MoodEmojiFor(moodLevel) : "·",
                MoodLevel = hasEntry ? moodLevel : null,
                HasEntry = hasEntry,
                IsSelected = selectedDay == date
            });
        }

        return chips;
    }

    public static List<JournalMonthCell> BuildMonthCells(
        DateOnly monthStart,
        DateOnly today,
        IReadOnlyDictionary<DateOnly, int> moodLevelByDay,
        DateOnly? selectedDay)
    {
        List<JournalMonthCell> cells = [];
        int leading = MondayFirstLeadingDays(monthStart);
        for (int i = 0; i < leading; i++)
        {
            cells.Add(new JournalMonthCell());
        }

        int daysInMonth = DateTime.DaysInMonth(monthStart.Year, monthStart.Month);
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateOnly date = new(monthStart.Year, monthStart.Month, day);
            bool hasEntry = moodLevelByDay.TryGetValue(date, out int moodLevel);
            bool enabled = date <= today;
            cells.Add(new JournalMonthCell
            {
                Date = date,
                DayNumber = day.ToString(CultureInfo.InvariantCulture),
                MoodGlyph = hasEntry
                    ? AppStrings.MoodEmojiFor(moodLevel)
                    : (enabled ? "·" : string.Empty),
                HasEntry = hasEntry,
                IsEnabled = enabled,
                IsSelected = selectedDay == date
            });
        }

        return cells;
    }

    public static List<JournalYearCell> BuildYearCells(
        int year,
        DateOnly today,
        IReadOnlyDictionary<DateOnly, int> moodLevelByDay,
        DateOnly? selectedDay)
    {
        DateOnly yearStart = new(year, 1, 1);
        DateOnly yearEnd = new(year, 12, 31);
        List<JournalYearCell> cells = [];
        int leading = MondayFirstLeadingDays(yearStart);
        for (int i = 0; i < leading; i++)
        {
            cells.Add(new JournalYearCell());
        }

        for (DateOnly date = yearStart; date <= yearEnd; date = date.AddDays(1))
        {
            bool hasEntry = moodLevelByDay.TryGetValue(date, out int moodLevel);
            bool enabled = date <= today;
            cells.Add(new JournalYearCell
            {
                Date = date,
                MoodGlyph = hasEntry
                    ? AppStrings.MoodEmojiFor(moodLevel)
                    : (enabled ? "·" : string.Empty),
                MoodLevel = hasEntry ? moodLevel : null,
                HasEntry = hasEntry,
                IsEnabled = enabled,
                IsSelected = selectedDay == date
            });
        }

        return cells;
    }

    /// <summary>Number of empty Monday-first cells before the first day of the period.</summary>
    public static int MondayFirstLeadingDays(DateOnly firstDay) =>
        ((int)firstDay.DayOfWeek + 6) % 7;
}
