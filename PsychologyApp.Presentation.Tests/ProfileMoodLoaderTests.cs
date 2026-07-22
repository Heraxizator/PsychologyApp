using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Shared.Services.Progress;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ProfileMoodLoaderTests
{
    private static ProfileMoodLoader CreateLoader(Mock<IUserProgressService> progress)
    {
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetMostRecentTestResultAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestResultDTO?)null);
        return new ProfileMoodLoader(progress.Object, new WeeklyInsightLoader(progress.Object));
    }

    [Fact]
    public async Task LoadAsync_ShowsChart_WhenTwoMoodsExist()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetRecentMoodsAsync(30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodLevel = 4, RecordedAt = new DateTime(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc) },
                new() { MoodLevel = 3, RecordedAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc) }
            });

        ProfileMoodLoader loader = CreateLoader(progress);
        ProfileMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.True(snapshot.HasTrendChart);
        Assert.Equal(2, snapshot.ChartPoints.Count);
        Assert.NotEmpty(snapshot.ChartSubtitle);
        Assert.Empty(snapshot.RecentNotes);
    }

    [Fact]
    public async Task LoadAsync_HidesChart_WhenSingleMoodExists()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetRecentMoodsAsync(30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new() { MoodLevel = 4, RecordedAt = new DateTime(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc) }
            });

        ProfileMoodLoader loader = CreateLoader(progress);
        ProfileMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.False(snapshot.HasTrendChart);
        Assert.Single(snapshot.ChartPoints);
    }

    [Fact]
    public async Task LoadAsync_ReturnsNonEmptyNotes_Only()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetRecentMoodsAsync(30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MoodEntryDTO>
            {
                new()
                {
                    MoodLevel = 4,
                    Note = "  Felt calmer  ",
                    RecordedAt = new DateTime(2026, 1, 3, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    MoodLevel = 3,
                    Note = "   ",
                    RecordedAt = new DateTime(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    MoodLevel = 2,
                    Note = null,
                    RecordedAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    MoodLevel = 5,
                    Note = "Good day",
                    RecordedAt = new DateTime(2025, 12, 31, 12, 0, 0, DateTimeKind.Utc)
                }
            });

        ProfileMoodLoader loader = CreateLoader(progress);
        ProfileMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.Equal(2, snapshot.RecentNotes.Count);
        Assert.Equal("Felt calmer", snapshot.RecentNotes[0].NoteText);
        Assert.Equal("Good day", snapshot.RecentNotes[1].NoteText);
    }
}
