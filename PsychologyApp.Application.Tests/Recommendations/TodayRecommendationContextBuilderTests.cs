using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Recommendations;

public sealed class TodayRecommendationContextBuilderTests
{
    [Fact]
    public async Task BuildAsync_AveragesSudsDropPerTechniqueWithEnoughSessions()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetMostRecentTestResultAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestResultDTO?)null);
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<MoodEntryDTO>)[]);
        progress.Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyDictionary<string, DateTime>)new Dictionary<string, DateTime>());
        progress.Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlySet<string>)new HashSet<string>());
        progress.Setup(p => p.GetRecentSessionResultsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<SessionResultDTO>)
            [
                new SessionResultDTO { ItemKey = TechniqueId.Breathing.ToString(), PreIntensity = 8, PostIntensity = 3 },
                new SessionResultDTO { ItemKey = TechniqueId.Breathing.ToString(), PreIntensity = 6, PostIntensity = 4 },
                new SessionResultDTO { ItemKey = TechniqueId.Grounding.ToString(), PreIntensity = 7, PostIntensity = 6 },
                // Only one session: not enough to trust the average yet.
                new SessionResultDTO { ItemKey = TechniqueId.SmallStep.ToString(), PreIntensity = 9, PostIntensity = 1 },
                // Missing a reading: excluded from the average entirely.
                new SessionResultDTO { ItemKey = TechniqueId.Breathing.ToString(), PreIntensity = 5, PostIntensity = null },
            ]);

        TodayRecommendationContext context = await TodayRecommendationContextBuilder.BuildAsync(
            progress.Object,
            OnboardingConcernKeys.Explore);

        Assert.NotNull(context.TechniqueEffectiveness);
        Assert.Equal(3.5, context.TechniqueEffectiveness![TechniqueId.Breathing.ToString()]);
        Assert.False(context.TechniqueEffectiveness.ContainsKey(TechniqueId.Grounding.ToString()));
        Assert.False(context.TechniqueEffectiveness.ContainsKey(TechniqueId.SmallStep.ToString()));
    }
}
