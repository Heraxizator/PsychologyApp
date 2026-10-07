using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Features.ManageJournal;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Charts;

/// <summary>The month and year calendars carry the mood level of each day, which the screen turns into its colour.</summary>
public class JournalCalendarMoodTests
{
    private static readonly DateOnly Today = new(2026, 10, 20);

    [Fact]
    public void MonthDaysWithANoteCarryTheirMoodLevelAndTheOthersCarryNone()
    {
        Dictionary<DateOnly, int> moods = new() { [new DateOnly(2026, 10, 3)] = 2, [new DateOnly(2026, 10, 9)] = 5 };

        List<JournalMonthCell> cells = JournalCalendarBuilder.BuildMonthCells(new DateOnly(2026, 10, 1), Today, moods, null);

        Assert.Equal(2, cells.Single(c => c.Date == new DateOnly(2026, 10, 3)).MoodLevel);
        Assert.Equal(5, cells.Single(c => c.Date == new DateOnly(2026, 10, 9)).MoodLevel);
        Assert.Null(cells.Single(c => c.Date == new DateOnly(2026, 10, 4)).MoodLevel);
        Assert.All(cells.Where(c => c.IsPlaceholder), c => Assert.Null(c.MoodLevel));
    }

    [Fact]
    public void YearDaysCarryTheirMoodLevelToo()
    {
        Dictionary<DateOnly, int> moods = new() { [new DateOnly(2026, 2, 14)] = 4 };

        List<JournalYearCell> cells = JournalCalendarBuilder.BuildYearCells(2026, Today, moods, null);

        Assert.Equal(4, cells.Single(c => c.Date == new DateOnly(2026, 2, 14)).MoodLevel);
    }
}
