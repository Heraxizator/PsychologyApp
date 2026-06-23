using PsychologyApp.Presentation.Entities.Technique;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class TechniqueDashboardUiState
{
    public required bool IsGrouped { get; init; }
    public required ObservableCollection<TechniqueGroup> Groups { get; init; }
    public required ObservableCollection<TechniqueItem> CatalogTechniques { get; init; }
    public required object ItemsSource { get; init; }
}

public static class TechniqueDashboardApplier
{
    public static TechniqueDashboardUiState CreateUiState(TechniqueListLayout layout) =>
        new()
        {
            IsGrouped = layout.IsGrouped,
            Groups = new ObservableCollection<TechniqueGroup>(layout.Groups),
            CatalogTechniques = layout.IsGrouped
                ? []
                : new ObservableCollection<TechniqueItem>(layout.StaticItems),
            ItemsSource = layout.ItemsSource
        };
}
