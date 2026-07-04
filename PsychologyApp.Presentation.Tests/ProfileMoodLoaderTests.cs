using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.ManageProfile;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ProfileMoodLoaderTests
{
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

        ProfileMoodLoader loader = new(progress.Object);
        ProfileMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.True(snapshot.HasTrendChart);
        Assert.Equal(2, snapshot.ChartPoints.Count);
        Assert.NotEmpty(snapshot.ChartSubtitle);
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

        ProfileMoodLoader loader = new(progress.Object);
        ProfileMoodSnapshot snapshot = await loader.LoadAsync();

        Assert.False(snapshot.HasTrendChart);
        Assert.Single(snapshot.ChartPoints);
    }
}
