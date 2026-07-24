using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
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

        JournalMoodLoader loader = new(progress.Object);
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

        JournalMoodLoader loader = new(progress.Object);
        JournalMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.Equal(3, snapshot.TimelineEntries.Count);
        Assert.True(snapshot.TimelineEntries[0].HasNote);
        Assert.Equal("Felt calmer", snapshot.TimelineEntries[0].NoteText);
        Assert.False(snapshot.TimelineEntries[1].HasNote);
        Assert.Equal(3, snapshot.TimelineGroups.Count);
        Assert.Equal(7, snapshot.WeekDays.Count);
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

        JournalMoodLoader loader = new(progress.Object);
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

        JournalMoodLoader loader = new(progress.Object);
        JournalMoodSnapshot snapshot = await loader.LoadAsync(editorDay: day);

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

        JournalMoodLoader loader = new(progress.Object);
        await loader.SaveMoodAsync(5, "updated", 9, DateOnly.FromDateTime(DateTime.Today));

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
            .Returns(Task.CompletedTask);

        JournalMoodLoader loader = new(progress.Object);
        await loader.SaveMoodAsync(4, "note", null, DateOnly.FromDateTime(DateTime.Today));

        progress.Verify(
            p => p.RecordMoodAsync(4, "note", It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteMoodAsync_CallsService()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.DeleteMoodEntryAsync(7, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        JournalMoodLoader loader = new(progress.Object);
        await loader.DeleteMoodAsync(7);

        progress.Verify(p => p.DeleteMoodEntryAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }
}
