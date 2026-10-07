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

    /// <summary>Every built-in practice, whatever the filter shows; paging and inserting a new practice work from this, not from what is on screen.</summary>
    private List<Entities.Technique.TechniqueItem>? _staticItemsAll;

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

    /// <summary>
    /// Shows only the built-in practices of the chosen group by swapping the contents of the built-in part of the list. Hiding the cards instead left
    /// the Android list with rows it never measured again, and the whole page went blank. Run after every refresh too, which brings every practice back.
    /// </summary>
    private void ApplyFlavorFilter()
    {
        if (_staticItemsAll is not { Count: > 0 } all)
        {
            return;
        }

        List<Entities.Technique.TechniqueItem> shown = [.. all.Where(item => TechniqueFlavors.Matches(item.Flavor, _flavorFilter))];
        if (shown.Count == 0)
        {
            shown = [.. all];
        }

        if (IsTechniquesGrouped && TechniqueGroups.Count > 0)
        {
            Entities.Technique.TechniqueGroup group = TechniqueGroups[0];
            if (!group.SequenceEqual(shown))
            {
                group.Clear();
                foreach (Entities.Technique.TechniqueItem item in shown)
                {
                    group.Add(item);
                }
            }
        }
        else if (!CatalogTechniques.SequenceEqual(shown))
        {
            CatalogTechniques.ReplaceAll(shown);
        }
    }
}
