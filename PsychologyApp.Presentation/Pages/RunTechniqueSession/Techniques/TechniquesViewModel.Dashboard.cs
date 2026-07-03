using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    private async Task OnTechniqueMessageAsync(TechniqueMessage message)
    {
        if (message.MessageType is not (TechniqueMessageType.Add or TechniqueMessageType.Remove or TechniqueMessageType.Change))
        {
            return;
        }

        await InitializeAsync(showLoadingOverlay: false);
    }

    private void UpdateTodayRecommendation(IReadOnlyList<TechniqueItem>? staticItems = null)
    {
        TodayRecommendationResult recommendation = _dashboardPresenter.ResolveTodayRecommendation(
            StreakDays,
            _navigationService);

        _todayTechniqueId = recommendation.TechniqueId;
        TodayReasonText = recommendation.ReasonText;
        TodayTechniqueItem = recommendation.Item;
        OnPropertyChanged(nameof(TodayReasonText));

        if (staticItems is not null)
        {
            _dashboardPresenter.ApplyCatalogDate(TodayTechniqueItem, _todayTechniqueId, staticItems, HasStreak);
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
    }
}
