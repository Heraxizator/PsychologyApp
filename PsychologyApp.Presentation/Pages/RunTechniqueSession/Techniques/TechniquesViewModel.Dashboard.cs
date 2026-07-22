using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    private async Task RefreshDashboardOnAppearAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            await _databaseReadySignal.WaitAsync(cancellationToken);

            Task<int> streakTask = _dashboardLoader.LoadStreakDaysAsync(cancellationToken);
            Task<int> atRiskTask = _dashboardLoader.LoadAtRiskStreakDaysAsync(cancellationToken);
            Task<DateTime?> lastPracticeTask = _dashboardLoader.LoadLastPracticeUtcAsync(cancellationToken);
            Task<MoodSnapshot> moodTask = _dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);
            Task<WeeklyInsightSnapshot> weeklyInsightTask = _dashboardLoader.LoadWeeklyInsightAsync(cancellationToken);
            Task<string?> lastTechniqueNameTask = _dashboardLoader.LoadLastTechniqueNameAsync(cancellationToken);
            Task<IReadOnlyList<TechniqueItem>> staticItemsTask =
                _techniqueListBuilder.BuildStaticItemsAsync(_navigationService, cancellationToken);

            await Task.WhenAll(
                streakTask,
                atRiskTask,
                lastPracticeTask,
                moodTask,
                weeklyInsightTask,
                lastTechniqueNameTask,
                staticItemsTask);

            int streakDays = await streakTask;
            int atRiskDays = await atRiskTask;
            DateTime? lastPracticeUtc = await lastPracticeTask;
            MoodSnapshot mood = await moodTask;
            WeeklyInsightSnapshot weeklyInsight = await weeklyInsightTask;
            string? lastTechniqueName = await lastTechniqueNameTask;
            IReadOnlyList<TechniqueItem> staticItems = await staticItemsTask;

            TodayRecommendationResult recommendation = await _dashboardPresenter.ResolveTodayRecommendationAsync(
                streakDays,
                _navigationService,
                cancellationToken);
            await _dashboardPresenter.ApplyCatalogDateAsync(
                recommendation.Item,
                recommendation.TechniqueId,
                staticItems,
                streakDays > 0,
                cancellationToken);

            bool hasDraft = await _dashboardLoader.HasSessionDraftAsync(recommendation.TechniqueId, cancellationToken);
            int idleDays = StreakCalculator.CalculateIdleDays(
                lastPracticeUtc is null ? null : DateOnly.FromDateTime(lastPracticeUtc.Value.ToLocalTime()),
                DateOnly.FromDateTime(DateTime.Today));

            TherapyProgramStateDTO? program = null;
            RiskAssessmentDTO? latestRisk = null;
            try
            {
                await _clinicalCareService.AdjustProgramFromScorecardAsync(cancellationToken);
                program = await _clinicalCareService.GetActiveProgramAsync(cancellationToken);
                latestRisk = await _clinicalCareService.GetLatestRiskAssessmentAsync(cancellationToken);
            }
            catch (Exception clinicalEx)
            {
                _logger.LogDebug(clinicalEx, "Clinical care dashboard enrichment skipped.");
            }

            await UiThread.RunAsync(() =>
            {
                StreakDays = streakDays;
                AtRiskStreakDays = atRiskDays;
                IdleDays = idleDays;
                LastTechniqueName = lastTechniqueName;
                HasTodayDraft = hasDraft;
                ApplyMoodSnapshot(mood);
                WeeklyInsightText = weeklyInsight.DisplayText;
                TherapyProgramBanner = program is { IsActive: true }
                    ? FormatProgramBanner(program)
                    : string.Empty;
                ClinicalRiskBanner = latestRisk is null
                    ? string.Empty
                    : FormatRiskBanner(latestRisk.RiskLevel);
                _todayTechniqueId = recommendation.TechniqueId;
                TodayReasonText = recommendation.ReasonText;
                TodayTechniqueItem = recommendation.Item;
                OnPropertyChanged(nameof(TodayReasonText));
                OnPropertyChanged(nameof(TodayTechniqueItem));
                OnPropertyChanged(nameof(TodayActionText));
            });
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Practice tab dashboard refresh failed.");
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task ApplyTechniqueMessageAsync(TechniqueMessage message)
    {
        if (message.MessageType is not (TechniqueMessageType.Add or TechniqueMessageType.Remove or TechniqueMessageType.Change))
        {
            return;
        }

        if (!_initialized)
        {
            return;
        }

        await _initGate.WaitAsync();
        try
        {
            switch (message.MessageType)
            {
                case TechniqueMessageType.Add:
                    await ApplyTechniqueAddedAsync(message.Technique);
                    break;
                case TechniqueMessageType.Remove:
                    await ApplyTechniqueRemovedAsync(message.Technique.TechniqueId);
                    break;
                case TechniqueMessageType.Change:
                    await ApplyTechniqueChangedAsync(message.Technique);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to apply technique list message surgically; falling back to full reload.");
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await InitAsync(timeoutSource.Token, showLoadingOverlay: false);
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task ApplyTechniqueAddedAsync(TechniqueDTO dto)
    {
        TechniqueItem item = _techniqueListBuilder
            .MapCustomItems([dto], _navigationService)
            .Single();

        await UiThread.RunAsync(() =>
        {
            if (!IsTechniquesGrouped || _customTechniquesGroup is null)
            {
                IReadOnlyList<TechniqueItem> staticItems = ExtractCurrentStaticItems();
                TechniqueListLayout layout = _techniqueListBuilder.BuildLayout(
                    staticItems,
                    [item],
                    MyTechniquesLabel);
                ApplyLayout(layout, hasMore: _hasMoreCustomTechniques, offset: 1);
                return;
            }

            if (_customTechniquesGroup.Any(existing => existing.Id == item.Id))
            {
                return;
            }

            _customTechniquesGroup.Insert(0, item);
            _customTechniquesOffset++;
        });
    }

    private async Task ApplyTechniqueRemovedAsync(long techniqueId)
    {
        await UiThread.RunAsync(() =>
        {
            TechniqueGroup? group = _customTechniquesGroup;
            if (group is null)
            {
                return;
            }

            TechniqueItem? existing = group.FirstOrDefault(i => i.Id == techniqueId);
            if (existing is null)
            {
                return;
            }

            group.Remove(existing);
            _customTechniquesOffset = Math.Max(0, _customTechniquesOffset - 1);

            if (group.Count == 0)
            {
                IReadOnlyList<TechniqueItem> staticItems = ExtractCurrentStaticItems();
                TechniqueListLayout layout = _techniqueListBuilder.BuildLayout(
                    staticItems,
                    [],
                    MyTechniquesLabel);
                ApplyLayout(layout, hasMore: false, offset: 0);
            }
        });
    }

    private async Task ApplyTechniqueChangedAsync(TechniqueDTO dto)
    {
        TechniqueItem mapped = _techniqueListBuilder
            .MapCustomItems([dto], _navigationService)
            .Single();

        await UiThread.RunAsync(() =>
        {
            TechniqueGroup? group = _customTechniquesGroup;
            if (group is null)
            {
                return;
            }

            for (int index = 0; index < group.Count; index++)
            {
                if (group[index].Id != dto.TechniqueId)
                {
                    continue;
                }

                group[index] = mapped;
                return;
            }
        });
    }

    private void UpdateTodayRecommendation(IReadOnlyList<TechniqueItem>? staticItems = null) =>
        UpdateTodayRecommendationCoreAsync(staticItems).FireAndForget();

    private async Task UpdateTodayRecommendationCoreAsync(IReadOnlyList<TechniqueItem>? staticItems = null)
    {
        TodayRecommendationResult recommendation = await _dashboardPresenter.ResolveTodayRecommendationAsync(
            StreakDays,
            _navigationService);

        bool hasDraft = await _dashboardLoader.HasSessionDraftAsync(recommendation.TechniqueId);

        _todayTechniqueId = recommendation.TechniqueId;
        TodayReasonText = recommendation.ReasonText;
        TodayTechniqueItem = recommendation.Item;
        HasTodayDraft = hasDraft;
        OnPropertyChanged(nameof(TodayReasonText));
        OnPropertyChanged(nameof(TodayActionText));

        if (staticItems is not null)
        {
            await _dashboardPresenter.ApplyCatalogDateAsync(
                TodayTechniqueItem,
                _todayTechniqueId,
                staticItems,
                HasStreak);
            OnPropertyChanged(nameof(TodayTechniqueItem));
        }
    }

    private void ApplyMoodSnapshot(MoodSnapshot snapshot)
    {
        TodayMoodDisplay = snapshot.TodayMoodDisplay;
        SelectedMoodLevel = snapshot.SelectedMoodLevel;
        MoodHistorySummary = snapshot.MoodHistorySummary;
        OnPropertyChanged(nameof(TodayMoodDisplay));
        OnPropertyChanged(nameof(HasTodayMood));
        OnPropertyChanged(nameof(MoodHistorySummary));
        OnPropertyChanged(nameof(HasMoodHistorySummary));
    }

    private async Task RecordMoodAsync(int moodLevel)
    {
        MoodRecordResult result = await _dashboardPresenter.RecordMoodAsync(moodLevel);
        SelectedMoodLevel = moodLevel;
        StreakDays = result.StreakDays;
        ApplyMoodSnapshot(result.MoodSnapshot);
        UpdateTodayRecommendation();
    }

    private IReadOnlyList<TechniqueItem> ExtractCurrentStaticItems()
    {
        if (IsTechniquesGrouped && TechniqueGroups.Count > 0)
        {
            return TechniqueGroups[0].ToList();
        }

        return CatalogTechniques.ToList();
    }

    private void ApplyLayout(TechniqueListLayout layout, bool hasMore, int offset)
    {
        TechniqueDashboardUiState ui = TechniqueDashboardApplier.CreateUiState(layout);
        ApplyUiState(ui);
        _hasMoreCustomTechniques = hasMore;
        _customTechniquesOffset = offset;
        _customTechniquesGroup = IsTechniquesGrouped && TechniqueGroups.Count > 1
            ? TechniqueGroups[^1]
            : null;
    }
}
