using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    private bool _initialized;

    public bool HasInitialized => _initialized;

    public async Task EnsureInitializedAsync()
    {
        if (_initialized)
        {
            return;
        }

        await InitializeAsync(showLoadingOverlay: true);
        _initialized = true;
    }

    private async Task InitializeAsync(bool showLoadingOverlay)
    {
        await _initGate.WaitAsync();
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await InitAsync(timeoutSource.Token, showLoadingOverlay);
        }
        finally
        {
            _initGate.Release();
        }
    }

    public async Task InitAsync(CancellationToken cancellationToken, bool showLoadingOverlay = true)
    {
        try
        {
            if (showLoadingOverlay)
            {
                await UiThread.RunAsync(SetInit);
            }

            await _databaseReadySignal.WaitAsync(cancellationToken);

            TechniquesInitSnapshot snapshot = await _listInitializer.LoadAsync(
                _techniqueService,
                _techniqueListBuilder,
                _dashboardLoader,
                _navigationService,
                MyTechniquesLabel,
                cancellationToken);

            TodayRecommendationResult recommendation = await _dashboardPresenter.ResolveTodayRecommendationAsync(
                snapshot.StreakDays,
                _navigationService,
                cancellationToken);
            await _dashboardPresenter.ApplyCatalogDateAsync(
                recommendation.Item,
                recommendation.TechniqueId,
                snapshot.StaticItems,
                snapshot.StreakDays > 0,
                cancellationToken);
            bool hasDraft = await _dashboardLoader.HasSessionDraftAsync(recommendation.TechniqueId, cancellationToken);

            await UiThread.RunAsync(() =>
            {
                StreakDays = snapshot.StreakDays;
                AtRiskStreakDays = snapshot.AtRiskStreakDays;
                IdleDays = snapshot.IdleDays;
                LastTechniqueName = snapshot.LastTechniqueName;
                HasTodayDraft = hasDraft;
                _todayTechniqueId = recommendation.TechniqueId;
                TodayReasonText = recommendation.ReasonText;
                TodayTechniqueItem = recommendation.Item;
                OnPropertyChanged(nameof(TodayReasonText));
                OnPropertyChanged(nameof(TodayPrimaryReason));
                OnPropertyChanged(nameof(TodayTechniqueItem));
                OnPropertyChanged(nameof(TodayActionText));
                ApplyUiState(snapshot.UiState);
                ApplyCustomTechniquesPagingState(snapshot);
                SetDone();
            });
        }
        catch (OperationCanceledException)
        {
            await UiThread.RunAsync(CancelProgress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Practice tab initialization failed.");
            await UiThread.RunAsync(SetFail);
            _toastService.ShortToast(AppStrings.PracticeInitError);
        }
    }

    private void ReloadLocalizedContent() =>
        ReloadLocalizedContentAsync().FireAndForget();

    private async Task ReloadLocalizedContentAsync()
    {
        if (!_initialized)
        {
            return;
        }

        await _initGate.WaitAsync();
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await InitAsync(timeoutSource.Token, showLoadingOverlay: false);
        }
        finally
        {
            _initGate.Release();
        }
    }
}
