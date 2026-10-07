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

        RebuildSections();
    }

    /// <summary>
    /// The catalog is the built-in practices in sections by group (only the chosen one with a filter) and then the person's own practices. The cards are never
    /// hidden, only the sections are replaced as a whole (hiding cards left a blank page on Android). Run after every refresh, which brings every practice back.
    /// </summary>
    private void RebuildSections()
    {
        List<Entities.Technique.TechniqueGroup> groups =
        [
            .. TechniqueSections.Build(_staticItemsAll ?? [], _flavorFilter, FlavorSectionTitle)
        ];
        if (_customGroup is not null)
        {
            groups.Add(_customGroup);
        }

        TechniqueGroups.ReplaceAll(groups);
    }

    private static string FlavorSectionTitle(TechniqueFlavor flavor) => FlavorFilterTitle(flavor.ToString());

    private Entities.Technique.TechniqueGroup? _customGroup;
}
