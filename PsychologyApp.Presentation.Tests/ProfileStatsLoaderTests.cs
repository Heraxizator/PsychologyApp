using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Pages.Onboarding;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ProfileStatsLoaderTests
{
    [Fact]
    public async Task LoadAsync_ReturnsFormattedCounts()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.CountTechniqueCompletionsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);
        progress.Setup(p => p.CountTestResultsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(4);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));

        ProfileStatsLoader loader = new(progress.Object);
        ProfileStatsSnapshot snapshot = await loader.LoadAsync();

        Assert.Equal("3", snapshot.TechniquesCompletedCount);
        Assert.Equal("2", snapshot.TestsCompletedCount);
        Assert.Contains("4", snapshot.StreakCount);
        Assert.False(string.IsNullOrWhiteSpace(snapshot.LastPracticeDisplay));
    }
}
