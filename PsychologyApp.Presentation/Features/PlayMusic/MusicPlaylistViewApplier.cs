using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Entities.FilterChip;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Features.PlayMusic;

public static class MusicPlaylistViewApplier
{
    public static void ApplyInitState(
        MusicPlaylistState state,
        RangeObservableCollection<Audio> allItems,
        ObservableCollection<FilterChipTabItem> categoryFilters,
        RangeObservableCollection<Audio> filteredItems,
        Action<string> setSelectedCategoryKey)
    {
        allItems.ReplaceAll(state.AllItems);

        categoryFilters.Clear();
        foreach (FilterChipTabItem filter in state.CategoryFilters)
        {
            categoryFilters.Add(filter);
        }

        setSelectedCategoryKey(state.SelectedCategoryKey);
        ReplaceFilteredItems(filteredItems, state.FilteredItems);
    }

    public static void ReplaceFilteredItems(RangeObservableCollection<Audio> filteredItems, IEnumerable<Audio> items)
    {
        filteredItems.ReplaceAll(items);
    }

    public static void SelectCategory(
        string? key,
        ObservableCollection<FilterChipTabItem> categoryFilters,
        Action<string> setSelectedCategoryKey,
        Action applyFilter)
    {
        setSelectedCategoryKey(key ?? string.Empty);
        foreach (FilterChipTabItem filter in categoryFilters)
        {
            filter.IsSelected = filter.Key == (key ?? string.Empty);
        }

        applyFilter();
    }
}
