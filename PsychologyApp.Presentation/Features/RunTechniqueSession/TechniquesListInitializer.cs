using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed record TechniquesInitSnapshot(
    int StreakDays,
    MoodSnapshot Mood,
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
        Task<MoodSnapshot> moodTask = dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);
        Task<IReadOnlyList<TechniqueItem>> staticItemsTask =
            listBuilder.BuildStaticItemsAsync(navigation, cancellationToken);
        Task<IEnumerable<TechniqueDTO>> customTechniquesTask =
            techniqueService.GetTechniquesPageAsync(0, pageSize + 1, cancellationToken);

        await Task.WhenAll(streakTask, moodTask, staticItemsTask, customTechniquesTask);

        List<TechniqueItem> staticItems = (await staticItemsTask).ToList();
        List<TechniqueDTO> customTechniquePage = (await customTechniquesTask).ToList();
        bool hasMoreCustomTechniques = customTechniquePage.Count > pageSize;
        List<TechniqueItem> customItems = listBuilder.MapCustomItems(
            customTechniquePage.Take(pageSize),
            navigation).ToList();
        TechniqueListLayout layout = listBuilder.BuildLayout(staticItems, customItems, myTechniquesLabel);
        TechniqueDashboardUiState uiState = TechniqueDashboardApplier.CreateUiState(layout);

        return new TechniquesInitSnapshot(
            await streakTask,
            await moodTask,
            uiState,
            staticItems,
            hasMoreCustomTechniques,
            customItems.Count);
    }
}
