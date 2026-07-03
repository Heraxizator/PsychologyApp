using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Toasts;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed record MoodRecordResult(int StreakDays, MoodSnapshot MoodSnapshot);

public sealed class TechniquesDashboardPresenter(
    PracticeDashboardLoader dashboardLoader,
    TodayRecommendationResolver todayRecommendationResolver,
    IToastService toastService)
{
    public TodayRecommendationResult ResolveTodayRecommendation(
        int streakDays,
        INavigationService navigationService) =>
        dashboardLoader.ResolveTodayRecommendation(streakDays, navigationService);

    public void ApplyCatalogDate(
        TechniqueItem? todayTechniqueItem,
        TechniqueId todayTechniqueId,
        IReadOnlyList<TechniqueItem> staticItems,
        bool hasStreak) =>
        todayRecommendationResolver.ApplyCatalogDate(todayTechniqueItem, todayTechniqueId, staticItems, hasStreak);

    public Task<MoodSnapshot> LoadMoodSnapshotAsync(CancellationToken cancellationToken = default) =>
        dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);

    public async Task<MoodRecordResult> RecordMoodAsync(int moodLevel, CancellationToken cancellationToken = default)
    {
        await dashboardLoader.RecordMoodAsync(moodLevel, cancellationToken);
        int streakDays = await dashboardLoader.LoadStreakDaysAsync(cancellationToken);
        MoodSnapshot moodSnapshot = await dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);
        toastService.ShortToast(AppStrings.TodayMoodSaved);
        return new MoodRecordResult(streakDays, moodSnapshot);
    }
}
