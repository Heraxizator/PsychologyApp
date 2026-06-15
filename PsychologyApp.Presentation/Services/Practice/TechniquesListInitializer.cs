using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Practice;

namespace PsychologyApp.Presentation.Services.Practice;

public sealed record TechniquesInitSnapshot(
    int StreakDays,
    MoodSnapshot Mood,
    TechniqueDashboardUiState UiState,
    IReadOnlyList<TechniqueItem> StaticItems);

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
        Task<int> streakTask = dashboardLoader.LoadStreakDaysAsync(cancellationToken);
        Task<MoodSnapshot> moodTask = dashboardLoader.LoadMoodSnapshotAsync(cancellationToken);
        Task<IReadOnlyList<TechniqueItem>> staticItemsTask =
            listBuilder.BuildStaticItemsAsync(navigation, cancellationToken);
        Task<IEnumerable<TechniqueDTO>> customTechniquesTask =
            techniqueService.GetTechniquesListAsync(500, cancellationToken);

        await Task.WhenAll(streakTask, moodTask, staticItemsTask, customTechniquesTask);

        List<TechniqueItem> staticItems = (await staticItemsTask).ToList();
        List<TechniqueItem> customItems = listBuilder.MapCustomItems(
            await customTechniquesTask,
            navigation).ToList();
        TechniqueListLayout layout = listBuilder.BuildLayout(staticItems, customItems, myTechniquesLabel);
        TechniqueDashboardUiState uiState = TechniqueDashboardApplier.CreateUiState(layout);

        return new TechniquesInitSnapshot(await streakTask, await moodTask, uiState, staticItems);
    }
}
