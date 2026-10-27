using Moq;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Features.ManageJournal;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class JournalMoodLoaderTests
{
    [Fact]
    public async Task LoadAsync_ShowsChart_WhenTwoMoodsExist()
    {
        Mock<IUserProgressService> progress = new();
        DateTime now = DateTime.UtcNow;
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodEntryId = 2, MoodLevel = 4, RecordedAt = now },
                new() { MoodEntryId = 1, MoodLevel = 3, RecordedAt = now.AddDays(-1) }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        JournalMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.True(snapshot.HasTrendChart);
        Assert.Equal(2, snapshot.ChartPoints.Count);
        Assert.NotEmpty(snapshot.ChartSubtitle);
        Assert.Equal(2, snapshot.TimelineEntries.Count);
        Assert.Equal(2, snapshot.TimelineGroups.Count);
        Assert.True(snapshot.Stats.HasStats);
        Assert.Equal(2, snapshot.Stats.CheckInCount);
        Assert.Contains("4", snapshot.Stats.BestWorstLabel, StringComparison.Ordinal);
        Assert.Contains("3", snapshot.Stats.BestWorstLabel, StringComparison.Ordinal);
    }

    // "Best 4 · worst 4" after a single check-in told nothing and looked like a glitch.
    [Fact]
    public async Task The_best_and_worst_mood_show_only_when_they_differ()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetMoodsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO> { new() { MoodEntryId = 1, MoodLevel = 4, RecordedAt = DateTime.UtcNow } });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodSnapshot snapshot = await new JournalMoodLoader(progress.Object, new KeywordCrisisDetector()).LoadAsync();

        Assert.True(snapshot.Stats.HasStats);
        Assert.Equal(string.Empty, snapshot.Stats.BestWorstLabel);
    }

    // Coming back to the journal after a crisis note must not turn the help-only card back into suggestions.
    [Theory]
    [InlineData("всем будет лучше без меня", true)]
    [InlineData("Felt calmer", false)]
    public async Task The_snapshot_says_whether_the_open_note_reads_like_a_crisis(string note, bool expected)
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetMoodsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO> { new() { MoodEntryId = 1, MoodLevel = 2, Note = note, RecordedAt = DateTime.Now } });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        JournalCheckInSlot slot = MoodCheckInSlotPolicy.IsMorningLocalHour(DateTime.Now.Hour) ? JournalCheckInSlot.Morning : JournalCheckInSlot.Evening;

        JournalMoodSnapshot snapshot = await new JournalMoodLoader(progress.Object, new KeywordCrisisDetector())
            .LoadAsync(editorDay: DateOnly.FromDateTime(DateTime.Today), editorSlot: slot);

        Assert.Equal(note, snapshot.EditorNote);
        Assert.Equal(expected, snapshot.EditorNoteLooksLikeCrisis);
    }

    // Not a streak: two check-ins on one day are one day, and a gap between days does not undo the days before it.
    [Fact]
    public async Task The_period_counts_days_with_a_check_in_not_days_in_a_row()
    {
        // Midday, so the minute before stays on the same day whenever the test runs.
        DateTime now = DateTime.Today.AddHours(12);
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetMoodsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodEntryId = 3, MoodLevel = 4, RecordedAt = now.ToUniversalTime() },
                new() { MoodEntryId = 2, MoodLevel = 3, RecordedAt = now.AddMinutes(-1).ToUniversalTime() },
                new() { MoodEntryId = 1, MoodLevel = 2, RecordedAt = now.AddDays(-3).ToUniversalTime() }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodSnapshot snapshot = await new JournalMoodLoader(progress.Object, new KeywordCrisisDetector()).LoadAsync(rangeDays: 7);

        Assert.Equal(AppStrings.JournalCheckInDays(2, 7), snapshot.Stats.CheckInDaysDisplay);
    }

    [Fact]
    public async Task LoadAsync_IncludesEntriesWithoutNotes_AndGroupsByDay()
    {
        Mock<IUserProgressService> progress = new();
        DateTime now = DateTime.UtcNow;
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new()
                {
                    MoodEntryId = 3,
                    MoodLevel = 4,
                    Note = "  Felt calmer  ",
                    RecordedAt = now
                },
                new()
                {
                    MoodEntryId = 2,
                    MoodLevel = 3,
                    Note = "   ",
                    RecordedAt = now.AddDays(-1)
                },
                new()
                {
                    MoodEntryId = 1,
                    MoodLevel = 2,
                    Note = null,
                    RecordedAt = now.AddDays(-2)
                }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        JournalMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.Equal(3, snapshot.TimelineEntries.Count);
        Assert.True(snapshot.TimelineEntries[0].HasNote);
        Assert.Equal("Felt calmer", snapshot.TimelineEntries[0].NoteText);
        Assert.False(snapshot.TimelineEntries[1].HasNote);
        Assert.Equal(3, snapshot.TimelineGroups.Count);
        Assert.Equal(7, snapshot.WeekDays.Count);
    }


    [Fact]
    public async Task LoadAsync_ChartShowsOnlyTheChosenRange_EvenWhenTheCalendarFetchedMore()
    {
        Mock<IUserProgressService> progress = new();
        DateTime now = DateTime.UtcNow;
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodEntryId = 4, MoodLevel = 4, RecordedAt = now },
                new() { MoodEntryId = 3, MoodLevel = 3, RecordedAt = now.AddDays(-3) },
                new() { MoodEntryId = 2, MoodLevel = 2, RecordedAt = now.AddDays(-12) },
                new() { MoodEntryId = 1, MoodLevel = 5, RecordedAt = now.AddDays(-25) }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        DateOnly earlier = DateOnly.FromDateTime(DateTime.Today.AddDays(-25));
        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        JournalMoodSnapshot snapshot = await loader.LoadAsync(
            rangeDays: 7,
            monthCursor: new DateOnly(earlier.Year, earlier.Month, 1),
            calendarScale: JournalCalendarScale.Month);

        Assert.Equal(2, snapshot.ChartPoints.Count);
    }
    [Fact]
    public async Task LoadAsync_Accepts90DayRange()
    {
        Mock<IUserProgressService> progress = new();
        DateTime? capturedFrom = null;
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Callback<DateTime?, DateTime?, int, CancellationToken>((from, _, _, _) => capturedFrom = from)
            .ReturnsAsync([]);
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        await loader.LoadAsync(rangeDays: 90);

        Assert.NotNull(capturedFrom);
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly expectedStart = today.AddDays(-89);
        DateTime expectedFromUtc = expectedStart.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        Assert.Equal(expectedFromUtc, capturedFrom);
    }

    [Fact]
    public async Task LoadAsync_LoadsEditorForSelectedDay()
    {
        Mock<IUserProgressService> progress = new();
        DateOnly day = DateOnly.FromDateTime(DateTime.Today.AddDays(-2));
        DateTime recorded = day.ToDateTime(new TimeOnly(15, 0)).ToUniversalTime();
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new()
                {
                    MoodEntryId = 11,
                    MoodLevel = 2,
                    Note = "past",
                    RecordedAt = recorded
                }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        JournalMoodSnapshot snapshot = await loader.LoadAsync(editorDay: day, editorSlot: JournalCheckInSlot.Evening);

        Assert.Equal(11, snapshot.EditorEntryId);
        Assert.Equal(day, snapshot.EditorDay);
        Assert.Equal(2, snapshot.SelectedMoodLevel);
        Assert.Equal("past", snapshot.EditorNote);
    }

    [Fact]
    public void FilterGroupsByNoteSearch_KeepsMatchingNotesOnly()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        var groups = new[]
        {
            new PsychologyApp.Presentation.Entities.Journal.JournalTimelineDayGroup(
                today,
                "Today",
                [
                    new PsychologyApp.Presentation.Entities.Profile.MoodNoteItem(
                        1, today, "d", "t", "Felt calmer", true, 4, "🙂 4/5", "🙂", true),
                    new PsychologyApp.Presentation.Entities.Profile.MoodNoteItem(
                        2, today, "d", "t", "Без заметки", false, 3, "😐 3/5", "😐", true)
                ])
        };

        var filtered = JournalMoodLoader.FilterGroupsByNoteSearch(groups, "calmer");

        Assert.Single(filtered);
        Assert.Single(filtered[0].Entries);
        Assert.Equal("Felt calmer", filtered[0].Entries[0].NoteText);
    }

    [Fact]
    public async Task SaveMoodAsync_Updates_WhenEntryIdProvided()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.UpdateMoodEntryAsync(9, 5, "updated", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        await loader.SaveMoodAsync(
            5,
            "updated",
            9,
            DateOnly.FromDateTime(DateTime.Today),
            JournalCheckInSlot.Morning);

        progress.Verify(p => p.UpdateMoodEntryAsync(9, 5, "updated", It.IsAny<CancellationToken>()), Times.Once);
        progress.Verify(
            p => p.RecordMoodAsync(
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveMoodAsync_Inserts_WhenNoEntryId()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.RecordMoodAsync(
                4,
                "note",
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        await loader.SaveMoodAsync(
            4,
            "note",
            null,
            DateOnly.FromDateTime(DateTime.Today),
            JournalCheckInSlot.Morning);

        progress.Verify(
            p => p.RecordMoodAsync(4, "note", It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("Не хочу жить", true)]
    [InlineData("Устала, но вечер был спокойный", false)]
    public async Task A_note_that_reads_like_a_crisis_is_saved_and_asks_for_the_help_page(string note, bool crisis)
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.RecordMoodAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);
        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());

        bool offerHelp = await loader.SaveMoodAsync(2, note, null, DateOnly.FromDateTime(DateTime.Today), JournalCheckInSlot.Evening);

        Assert.Equal(crisis, offerHelp);
        progress.Verify(p => p.RecordMoodAsync(2, note, It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoadAsync_WeekScale_DoesNotBuildYearCells()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodEntryId = 1, MoodLevel = 3, RecordedAt = DateTime.UtcNow }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        JournalMoodSnapshot snapshot = await loader.LoadAsync(calendarScale: JournalCalendarScale.Week);

        Assert.Equal(JournalCalendarScale.Week, snapshot.CalendarScale);
        Assert.NotEmpty(snapshot.WeekDays);
        Assert.Empty(snapshot.MonthCells);
        Assert.Empty(snapshot.YearCells);
    }

    [Fact]
    public async Task LoadAsync_YearScale_BuildsYearCells()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetMoodsAsync(
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodEntryId = 1, MoodLevel = 4, RecordedAt = DateTime.UtcNow }
            });
        progress
            .Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        JournalMoodLoader loader = new(progress.Object, new KeywordCrisisDetector());
        JournalMoodSnapshot snapshot = await loader.LoadAsync(calendarScale: JournalCalendarScale.Year);

        Assert.Equal(JournalCalendarScale.Year, snapshot.CalendarScale);
        Assert.Empty(snapshot.WeekDays);
        Assert.Empty(snapshot.MonthCells);
        Assert.True(snapshot.YearCells.Count > 300);
    }
}
