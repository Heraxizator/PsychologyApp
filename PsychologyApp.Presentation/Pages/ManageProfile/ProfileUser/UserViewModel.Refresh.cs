using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageProfile.Index;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    private async Task RefreshCoreAsync(int generation, bool forceQuotesReload)
    {
        try
        {
            await UiThread.RunAsync(InitTechniques);

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            ProfileScreenRefreshResult? result = await _profileScreenCoordinator.RefreshDashboardAsync(
                _profileStatsLoader,
                _practiceHistoryLoader,
                _profileQuotesLoader,
                generation,
                () => Volatile.Read(ref _initGeneration),
                forceQuotesReload,
                cancellationToken);

            if (result is null)
            {
                return;
            }

            TechniquesCompletedCount = result.Stats.TechniquesCompletedCount;
            TestsCompletedCount = result.Stats.TestsCompletedCount;
            StreakCount = result.Stats.StreakCount;
            LastPracticeDisplay = result.Stats.LastPracticeDisplay;
            OnPropertyChanged(nameof(LastPracticeDisplay));
            OnPropertyChanged(nameof(HasLastPractice));

            await UiThread.RunAsync(() =>
            {
                PracticeHistory = result.History;
                OnPropertyChanged(nameof(PracticeHistory));
                OnPropertyChanged(nameof(HasPracticeHistory));
                OnPropertyChanged(nameof(ShowPracticeHistoryEmpty));
            });

            if (result.ShouldLoadQuotes)
            {
                await LoadQuotesAsync(generation, cancellationToken);
            }
        }
        catch (Exception e)
        {
            if (generation == Volatile.Read(ref _initGeneration))
            {
                await UiThread.RunAsync(SetQuotesFailed);
            }

            _logger.LogError(e, "UserViewModel refresh failed.");
        }
    }

    private Task ReloadQuotesAsync() => RefreshAsync(forceQuotesReload: true);

    private void OnFavoritesChanged() =>
        RefreshAsync(forceQuotesReload: true).FireAndForget();

    private void InitTechniques()
    {
        Techniques.Clear();

        foreach (TechniqueItem item in _profileScreenCoordinator.BuildFeaturedTechniques(
                     _featuredTechniquesBuilder,
                     _navigationService))
        {
            Techniques.Add(item);
        }
    }
}
