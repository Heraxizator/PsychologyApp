using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed record MoodRecordResult(int StreakDays, MoodSnapshot MoodSnapshot);

public sealed class TechniquesDashboardPresenter(
    PracticeDashboardLoader dashboardLoader,
    TodayRecommendationResolver todayRecommendationResolver,
    IToastService toastService)
{
    public Task<TodayRecommendationResult> ResolveTodayRecommendationAsync(
        int streakDays,
        INavigationService navigationService,
        CancellationToken cancellationToken = default) =>
        dashboardLoader.ResolveTodayRecommendationAsync(streakDays, navigationService, cancellationToken);

    public Task ApplyCatalogDateAsync(
        TechniqueItem? todayTechniqueItem,
        TechniqueId todayTechniqueId,
        IReadOnlyList<TechniqueItem> staticItems,
        bool hasStreak,
        CancellationToken cancellationToken = default)
    {
        if (todayTechniqueItem is null)
        {
            return Task.CompletedTask;
        }

        return todayRecommendationResolver.ApplyCatalogDateAsync(
            todayTechniqueItem,
            todayTechniqueId,
            staticItems,
            hasStreak,
            cancellationToken);
    }

    public Task<MoodSnapshot> LoadMoodSnapshotAsync(CancellationToken cancellationToken = default) =>
        dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);

    public async Task<MoodRecordResult> RecordMoodAsync(int moodLevel, CancellationToken cancellationToken = default)
    {
        await dashboardLoader.RecordMoodAsync(moodLevel, cancellationToken);
        int streakDays = await dashboardLoader.LoadStreakDaysAsync(cancellationToken);
        MoodSnapshot moodSnapshot = await dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);
        toastService.ShortToast(AppStrings.TodayMoodSaved, AppToastKind.Success);
        return new MoodRecordResult(streakDays, moodSnapshot);
    }
}
