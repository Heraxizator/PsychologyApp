using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageProfile.Index;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    private async Task RefreshCoreAsync(int generation, bool forceQuotesReload)
    {
        bool showLoading = !_initialized || IsFail;
        if (showLoading)
        {
            SetInit();
        }

        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            IReadOnlyList<TechniqueItem> featuredTechniques =
                await _profileScreenCoordinator.BuildFeaturedTechniquesAsync(
                    _featuredTechniquesBuilder,
                    _navigationService,
                    cancellationToken);

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
                if (showLoading && generation == Volatile.Read(ref _initGeneration))
                {
                    SetDone();
                }

                return;
            }

            ProfileMoodSnapshot moodSnapshot = await _profileMoodLoader.LoadAsync(cancellationToken);
            MoodChartPoints = moodSnapshot.ChartPoints;
            MoodChartSubtitle = moodSnapshot.ChartSubtitle;
            HasMoodTrendChart = moodSnapshot.HasTrendChart;
            MoodNotes = moodSnapshot.RecentNotes;

            TechniquesCompletedCount = result.Stats.TechniquesCompletedCount;
            TestsCompletedCount = result.Stats.TestsCompletedCount;
            StreakCount = result.Stats.StreakCount;
            LastPracticeDisplay = result.Stats.LastPracticeDisplay;
            OnPropertyChanged(nameof(LastPracticeDisplay));
            OnPropertyChanged(nameof(HasLastPractice));

            await UiThread.RunAsync(() =>
            {
                Techniques.Clear();
                foreach (TechniqueItem item in featuredTechniques)
                {
                    Techniques.Add(item);
                }

                PracticeHistory = new ObservableCollection<PracticeHistoryItem>(
                    result.History.Select(item => ProfilePracticeHistoryTapFactory.WithTapCommand(item, _navigationService)));
                OnPropertyChanged(nameof(PracticeHistory));
                OnPropertyChanged(nameof(HasPracticeHistory));
                OnPropertyChanged(nameof(ShowPracticeHistoryEmpty));
            });

            if (result.ShouldLoadQuotes)
            {
                await LoadQuotesAsync(generation, cancellationToken);
            }

            if (generation == Volatile.Read(ref _initGeneration))
            {
                SetDone();
            }
        }
        catch (Exception e)
        {
            // Quotes failures are handled inside LoadQuotesAsync via SetQuotesFailed.
            // Dashboard/history failures must not masquerade as a quotes-only error.
            if (generation == Volatile.Read(ref _initGeneration))
            {
                await UiThread.RunAsync(SetFail);
            }

            _logger.LogError(e, "UserViewModel refresh failed.");
        }
    }

    private Task ReloadQuotesAsync() => RefreshAsync(forceQuotesReload: true);

    private void OnFavoritesChanged() =>
        RefreshAsync(forceQuotesReload: true).FireAndForget();

    private void InitTechniques() => InitTechniquesAsync().FireAndForget();

    private async Task InitTechniquesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TechniqueItem> items = await _profileScreenCoordinator.BuildFeaturedTechniquesAsync(
            _featuredTechniquesBuilder,
            _navigationService,
            cancellationToken);

        await UiThread.RunAsync(() =>
        {
            Techniques.Clear();
            foreach (TechniqueItem item in items)
            {
                Techniques.Add(item);
            }
        });
    }
}
