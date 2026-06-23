using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Entities.FilterChip;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.PlayMusic;

public sealed class MusicPlaylistState
{
    public required ObservableCollection<Audio> AllItems { get; init; }
    public required ObservableCollection<Audio> FilteredItems { get; init; }
    public required ObservableCollection<FilterChipTabItem> CategoryFilters { get; init; }
    public required string SelectedCategoryKey { get; init; }
}

public sealed class MusicPlaylistPresenter
{
    public MusicPlaylistState Initialize(
        ObservableCollection<Audio> source,
        string selectedCategoryKey,
        Func<Audio, ICommand> clickCommandFactory)
    {
        ObservableCollection<Audio> allItems = [];
        foreach (Audio item in source)
        {
            item.ClickCommand = clickCommandFactory(item);
            item.IsCached = !string.IsNullOrWhiteSpace(item.URL) && MusicAudioCache.IsCached(item.URL);
            allItems.Add(item);
        }

        ObservableCollection<FilterChipTabItem> categoryFilters = BuildCategoryFilters(allItems, selectedCategoryKey);
        ObservableCollection<Audio> filteredItems = Filter(allItems, selectedCategoryKey, query: string.Empty);

        return new MusicPlaylistState
        {
            AllItems = allItems,
            FilteredItems = filteredItems,
            CategoryFilters = categoryFilters,
            SelectedCategoryKey = selectedCategoryKey
        };
    }

    public ObservableCollection<FilterChipTabItem> BuildCategoryFilters(
        IReadOnlyList<Audio> allItems,
        string selectedCategoryKey)
    {
        ObservableCollection<FilterChipTabItem> filters = [];

        filters.Add(new FilterChipTabItem
        {
            Key = string.Empty,
            Title = AppStrings.CleanerCategoryAll,
            IsSelected = string.IsNullOrEmpty(selectedCategoryKey)
        });

        foreach (string category in allItems
                     .Select(item => item.Category)
                     .Where(category => !string.IsNullOrWhiteSpace(category))
                     .Distinct(StringComparer.Ordinal))
        {
            filters.Add(new FilterChipTabItem
            {
                Key = category!,
                Title = category!,
                IsSelected = category == selectedCategoryKey
            });
        }

        return filters;
    }

    public ObservableCollection<Audio> Filter(IReadOnlyList<Audio> allItems, string selectedCategoryKey, string query)
    {
        IEnumerable<Audio> source = allItems;

        if (!string.IsNullOrWhiteSpace(selectedCategoryKey))
        {
            source = source.Where(item => item.Category == selectedCategoryKey);
        }

        string trimmed = query.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            source = source.Where(item =>
                (item.Name?.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Description?.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Category?.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return new ObservableCollection<Audio>(source);
    }

    public Audio? ResolveAdjacentTrack(Audio? activeTrack, IReadOnlyList<Audio> filteredItems, IReadOnlyList<Audio> allItems, int step)
    {
        if (activeTrack is null || filteredItems.Count == 0)
        {
            return null;
        }

        int index = FindIndex(filteredItems, activeTrack);
        if (index < 0)
        {
            index = FindIndex(allItems, activeTrack);
            if (index < 0)
            {
                return filteredItems[0];
            }

            return allItems[Math.Clamp(index + step, 0, allItems.Count - 1)];
        }

        int nextIndex = (index + step + filteredItems.Count) % filteredItems.Count;
        return filteredItems[nextIndex];
    }

    private static int FindIndex(IReadOnlyList<Audio> items, Audio? target)
    {
        if (target is null)
        {
            return -1;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (ReferenceEquals(items[i], target))
            {
                return i;
            }
        }

        return -1;
    }
}
