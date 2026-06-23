using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.Techniques;

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
        TodayRecommendationResult recommendation = _dashboardLoader.ResolveTodayRecommendation(
            StreakDays,
            _navigationService);

        _todayTechniqueId = recommendation.TechniqueId;
        TodayReasonText = recommendation.ReasonText;
        TodayTechniqueItem = recommendation.Item;
        OnPropertyChanged(nameof(TodayReasonText));

        if (staticItems is not null)
        {
            TodayRecommendationResolver.ApplyCatalogDate(TodayTechniqueItem, _todayTechniqueId, staticItems, HasStreak);
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
        await _dashboardLoader.RecordMoodAsync(moodLevel);
        SelectedMoodLevel = moodLevel;
        StreakDays = await _dashboardLoader.LoadStreakDaysAsync();
        ApplyMoodSnapshot(await _dashboardLoader.LoadMoodSnapshotAsync());
        _toastService.ShortToast(AppStrings.TodayMoodSaved);
    }
}
