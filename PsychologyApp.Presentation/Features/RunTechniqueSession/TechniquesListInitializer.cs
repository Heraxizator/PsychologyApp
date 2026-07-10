using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed record TechniquesInitSnapshot(
    int StreakDays,
    int AtRiskStreakDays,
    int IdleDays,
    MoodSnapshot Mood,
    WeeklyInsightSnapshot WeeklyInsight,
    string? LastTechniqueName,
    TechniqueDashboardUiState UiState,
    IReadOnlyList<TechniqueItem> StaticItems,
    bool HasMoreCustomTechniques,
    int CustomTechniquesLoadedCount);

public sealed class TechniquesListInitializer
{
    public async Task<TechniquesInitSnapshot> LoadAsync(
        ITechniqueService techniqueService,
        TechniqueListBuilder listBuilder,
        PracticeDashboardLoader dashboardLoader,
        INavigationService navigation,
        string myTechniquesLabel,
        CancellationToken cancellationToken)
    {
        int pageSize = CatalogListPolicy.CustomTechniquesPageSize;

        Task<int> streakTask = dashboardLoader.LoadStreakDaysAsync(cancellationToken);
        Task<int> atRiskTask = dashboardLoader.LoadAtRiskStreakDaysAsync(cancellationToken);
        Task<DateTime?> lastPracticeTask = dashboardLoader.LoadLastPracticeUtcAsync(cancellationToken);
        Task<MoodSnapshot> moodTask = dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);
        Task<WeeklyInsightSnapshot> weeklyInsightTask = dashboardLoader.LoadWeeklyInsightAsync(cancellationToken);
        Task<string?> lastTechniqueNameTask = dashboardLoader.LoadLastTechniqueNameAsync(cancellationToken);
        Task<IReadOnlyList<TechniqueItem>> staticItemsTask =
            listBuilder.BuildStaticItemsAsync(navigation, cancellationToken);
        Task<IEnumerable<TechniqueDTO>> customTechniquesTask =
            techniqueService.GetTechniquesPageAsync(0, pageSize + 1, cancellationToken);

        await Task.WhenAll(
            streakTask,
            atRiskTask,
            lastPracticeTask,
            moodTask,
            weeklyInsightTask,
            lastTechniqueNameTask,
            staticItemsTask,
            customTechniquesTask);

        List<TechniqueItem> staticItems = (await staticItemsTask).ToList();
        List<TechniqueDTO> customTechniquePage = (await customTechniquesTask).ToList();
        bool hasMoreCustomTechniques = customTechniquePage.Count > pageSize;
        List<TechniqueItem> customItems = listBuilder.MapCustomItems(
            customTechniquePage.Take(pageSize),
            navigation).ToList();
        TechniqueListLayout layout = listBuilder.BuildLayout(staticItems, customItems, myTechniquesLabel);
        TechniqueDashboardUiState uiState = TechniqueDashboardApplier.CreateUiState(layout);

        DateTime? lastPracticeUtc = await lastPracticeTask;
        int idleDays = StreakCalculator.CalculateIdleDays(
            lastPracticeUtc is null ? null : DateOnly.FromDateTime(lastPracticeUtc.Value.ToLocalTime()),
            DateOnly.FromDateTime(DateTime.Today));

        return new TechniquesInitSnapshot(
            await streakTask,
            await atRiskTask,
            idleDays,
            await moodTask,
            await weeklyInsightTask,
            await lastTechniqueNameTask,
            uiState,
            staticItems,
            hasMoreCustomTechniques,
            customItems.Count);
    }
}
