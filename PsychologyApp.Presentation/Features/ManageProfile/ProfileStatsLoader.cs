using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfileStatsSnapshot
{
    public string TechniquesCompletedCount { get; init; } = "0";
    public string TestsCompletedCount { get; init; } = "0";
    public string StreakCount { get; init; } = "0";
    public string LastPracticeDisplay { get; init; } = string.Empty;
}

public sealed class ProfileStatsLoader(IUserProgressService userProgressService)
{
    public async Task<ProfileStatsSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        Task<long> techniquesTask = userProgressService.CountTechniqueCompletionsAsync(cancellationToken);
        Task<long> testsTask = userProgressService.CountTestResultsAsync(cancellationToken);
        Task<int> streakTask = userProgressService.GetStreakDaysAsync(cancellationToken);
        Task<DateTime?> lastPracticeTask = userProgressService.GetLastTechniqueCompletionDateAsync(cancellationToken);

        await Task.WhenAll(techniquesTask, testsTask, streakTask, lastPracticeTask);

        int techniques = (int)await techniquesTask;
        int tests = (int)await testsTask;
        int streak = await streakTask;
        DateTime? lastPractice = await lastPracticeTask;

        return new ProfileStatsSnapshot
        {
            TechniquesCompletedCount = techniques.ToString(),
            TestsCompletedCount = tests.ToString(),
            StreakCount = AppStrings.ProfileStreakCount(streak),
            LastPracticeDisplay = lastPractice is null
                ? string.Empty
                : AppStrings.ProfileLastPractice(lastPractice.Value.ToLocalTime().ToString("d"))
        };
    }
}
