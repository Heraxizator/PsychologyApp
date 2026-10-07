using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

/// <summary>
/// The group chips above the catalog ("Body", "Thoughts", ...). The list itself is never rebuilt: cards of other groups are only switched off,
/// so loading more of the person's own practices and the "new practice" insert keep working as before.
/// </summary>
public partial class TechniquesViewModel
{
    private string _flavorFilter = TechniqueFlavors.FilterAll;

    public ObservableCollection<FilterChipTabItem> FlavorFilters { get; } = [];

    public ICommand SelectFlavorCommand { get; private set; } = default!;

    public string FlavorFilterLabel => AppStrings.PracticeFilterLabel;

    private void WireFlavorFilters()
    {
        SelectFlavorCommand = new Command<string?>(SelectFlavor);
        EnsureFlavorFilters();
    }

    private void EnsureFlavorFilters()
    {
        FlavorFilters.Clear();
        foreach (string key in TechniqueFlavors.FilterKeys)
        {
            FlavorFilters.Add(new FilterChipTabItem { Key = key, Title = FlavorFilterTitle(key), IsSelected = key == _flavorFilter });
        }
    }

    private static string FlavorFilterTitle(string key) => key switch
    {
        nameof(TechniqueFlavor.Body) => AppStrings.PracticeFilterBody,
        nameof(TechniqueFlavor.Mind) => AppStrings.PracticeFilterMind,
        nameof(TechniqueFlavor.Heart) => AppStrings.PracticeFilterHeart,
        nameof(TechniqueFlavor.Action) => AppStrings.PracticeFilterAction,
        _ => AppStrings.PracticeFilterAll
    };

    private void SelectFlavor(string? key)
    {
        _flavorFilter = string.IsNullOrEmpty(key) ? TechniqueFlavors.FilterAll : key;
        foreach (FilterChipTabItem chip in FlavorFilters)
        {
            chip.IsSelected = chip.Key == _flavorFilter;
        }

        ApplyFlavorFilter();
    }

    /// <summary>Switches the cards on or off for the chosen group; also run after every refresh of the list, which brings every card back on.</summary>
    private void ApplyFlavorFilter()
    {
        foreach (Entities.Technique.TechniqueItem item in AllCatalogItems())
        {
            item.Active = TechniqueFlavors.Matches(item.Flavor, _flavorFilter);
        }
    }

    private IEnumerable<Entities.Technique.TechniqueItem> AllCatalogItems() =>
        IsTechniquesGrouped ? TechniqueGroups.SelectMany(group => group) : CatalogTechniques;
}
