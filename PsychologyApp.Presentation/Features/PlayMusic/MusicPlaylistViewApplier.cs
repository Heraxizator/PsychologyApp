using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Entities.FilterChip;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Features.PlayMusic;

public static class MusicPlaylistViewApplier
{
    public static void ApplyInitState(
        MusicPlaylistState state,
        ObservableCollection<Audio> allItems,
        ObservableCollection<FilterChipTabItem> categoryFilters,
        ObservableCollection<Audio> filteredItems,
        Action<string> setSelectedCategoryKey)
    {
        allItems.Clear();
        foreach (Audio item in state.AllItems)
        {
            allItems.Add(item);
        }

        categoryFilters.Clear();
        foreach (FilterChipTabItem filter in state.CategoryFilters)
        {
            categoryFilters.Add(filter);
        }

        setSelectedCategoryKey(state.SelectedCategoryKey);
        ReplaceFilteredItems(filteredItems, state.FilteredItems);
    }

    public static void ReplaceFilteredItems(ObservableCollection<Audio> filteredItems, IEnumerable<Audio> items)
    {
        filteredItems.Clear();
        foreach (Audio item in items)
        {
            filteredItems.Add(item);
        }
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
