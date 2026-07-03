using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Features.ManageProfile.Index;

public sealed record ProfileScreenRefreshResult(
    ProfileStatsSnapshot Stats,
    ObservableCollection<PracticeHistoryItem> History,
    bool ShouldLoadQuotes);

/// <summary>
/// Orchestrates profile screen refresh and featured technique composition.
/// </summary>
public sealed class ProfileScreenCoordinator(UserProfileRefreshCoordinator refreshCoordinator)
{
    public IReadOnlyList<TechniqueItem> BuildFeaturedTechniques(
        ProfileFeaturedTechniquesBuilder builder,
        INavigationService navigationService) =>
        builder.Build(navigationService);

    public async Task<ProfileScreenRefreshResult?> RefreshDashboardAsync(
        ProfileStatsLoader statsLoader,
        ProfilePracticeHistoryLoader historyLoader,
        ProfileQuotesLoader quotesLoader,
        int generation,
        Func<int> getCurrentGeneration,
        bool forceQuotesReload,
        CancellationToken cancellationToken)
    {
        UserProfileRefreshSnapshot? snapshot = await refreshCoordinator.LoadDashboardAsync(
            statsLoader,
            historyLoader,
            quotesLoader,
            generation,
            getCurrentGeneration,
            forceQuotesReload,
            cancellationToken);

        return snapshot is null
            ? null
            : new ProfileScreenRefreshResult(
                snapshot.Stats,
                new ObservableCollection<PracticeHistoryItem>(snapshot.History),
                snapshot.ShouldLoadQuotes);
    }
}
