using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfileStatsSnapshot
{
    public string TechniquesCompletedCount { get; init; } = "0";
    public string TestsCompletedCount { get; init; } = "0";
    public string ActiveDaysCount { get; init; } = "0";
}

public sealed class ProfileStatsLoader(IUserProgressService userProgressService)
{
    // Off the UI thread: building the snapshot after each query would otherwise run there.
    public Task<ProfileStatsSnapshot> LoadAsync(CancellationToken cancellationToken = default) =>
        Task.Run(() => LoadCoreAsync(cancellationToken), cancellationToken);

    private async Task<ProfileStatsSnapshot> LoadCoreAsync(CancellationToken cancellationToken = default)
    {
        Task<long> techniquesTask = userProgressService.CountTechniqueCompletionsAsync(cancellationToken);
        Task<long> testsTask = userProgressService.CountTestResultsAsync(cancellationToken);
        Task<int> activeDaysTask = userProgressService.GetActiveDaysThisWeekAsync(cancellationToken);

        await Task.WhenAll(techniquesTask, testsTask, activeDaysTask);

        int techniques = (int)await techniquesTask;
        int tests = (int)await testsTask;
        int activeDays = await activeDaysTask;

        return new ProfileStatsSnapshot
        {
            TechniquesCompletedCount = techniques.ToString(),
            TestsCompletedCount = tests.ToString(),
            ActiveDaysCount = AppStrings.ProfileActiveDaysValue(activeDays)
        };
    }
}
