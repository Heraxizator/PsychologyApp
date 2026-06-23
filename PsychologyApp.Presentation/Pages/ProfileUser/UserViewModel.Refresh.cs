using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageProfile;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Pages.ProfileUser;

public partial class UserViewModel
{
    private async Task RefreshCoreAsync(int generation, bool forceQuotesReload)
    {
        try
        {
            await UiThread.RunAsync(InitTechniques);

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            UserProfileRefreshSnapshot? snapshot = await _profileRefreshCoordinator.LoadDashboardAsync(
                _profileStatsLoader,
                _practiceHistoryLoader,
                _profileQuotesLoader,
                generation,
                () => Volatile.Read(ref _initGeneration),
                forceQuotesReload,
                cancellationToken);

            if (snapshot is null)
            {
                return;
            }

            TechniquesCompletedCount = snapshot.Stats.TechniquesCompletedCount;
            TestsCompletedCount = snapshot.Stats.TestsCompletedCount;
            StreakCount = snapshot.Stats.StreakCount;
            LastPracticeDisplay = snapshot.Stats.LastPracticeDisplay;
            OnPropertyChanged(nameof(LastPracticeDisplay));
            OnPropertyChanged(nameof(HasLastPractice));

            await UiThread.RunAsync(() =>
            {
                PracticeHistory = new ObservableCollection<PracticeHistoryItem>(snapshot.History);
                OnPropertyChanged(nameof(PracticeHistory));
                OnPropertyChanged(nameof(HasPracticeHistory));
                OnPropertyChanged(nameof(ShowPracticeHistoryEmpty));
            });

            if (snapshot.ShouldLoadQuotes)
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

        foreach (TechniqueItem item in _featuredTechniquesBuilder.Build(_navigationService))
        {
            Techniques.Add(item);
        }
    }
}
