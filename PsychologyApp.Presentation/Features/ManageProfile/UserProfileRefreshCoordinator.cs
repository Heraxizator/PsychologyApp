using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageProfile;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed record UserProfileRefreshSnapshot(
    ProfileStatsSnapshot Stats,
    IReadOnlyList<PracticeHistoryItem> History,
    bool ShouldLoadQuotes);

public sealed class UserProfileRefreshCoordinator
{
    public async Task<UserProfileRefreshSnapshot?> LoadDashboardAsync(
        ProfileStatsLoader statsLoader,
        ProfilePracticeHistoryLoader historyLoader,
        ProfileQuotesLoader quotesLoader,
        int generation,
        Func<int> getCurrentGeneration,
        bool forceQuotesReload,
        CancellationToken cancellationToken)
    {
        Task<ProfileStatsSnapshot> statsTask = statsLoader.LoadAsync(cancellationToken);
        Task<IReadOnlyList<PracticeHistoryItem>> historyTask = historyLoader.LoadAsync(10, cancellationToken);

        await Task.WhenAll(statsTask, historyTask);

        if (generation != getCurrentGeneration())
        {
            return null;
        }

        bool shouldLoadQuotes = UserQuotesRefreshPolicy.ShouldReload(quotesLoader.LoadedOnce, forceQuotesReload);
        return new UserProfileRefreshSnapshot(await statsTask, await historyTask, shouldLoadQuotes);
    }
}
