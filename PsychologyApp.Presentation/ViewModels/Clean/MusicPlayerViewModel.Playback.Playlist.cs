using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Clean;
using PsychologyApp.Presentation.Services.Clean;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.ViewModels.Clean;

public partial class MusicPlayerViewModel
{
    public void InitializePlaylist()
    {
        try
        {
            InitItems(MusicPlaylist.CreateDefault());
            SetDone();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize music playlist.");
            SetFail();
        }
    }

    public void RefreshCacheFlags()
    {
        foreach (Audio item in AllItems)
        {
            item.IsCached = !string.IsNullOrWhiteSpace(item.URL) && MusicAudioCache.IsCached(item.URL);
        }
    }

    private void InitItems(ObservableCollection<Audio> collection)
    {
        MusicPlaylistState state = _playlistPresenter.Initialize(
            collection,
            _selectedCategoryKey,
            item => new AsyncCommand(() => PlayTrackAsync(item)));

        MusicPlaylistViewApplier.ApplyInitState(
            state,
            AllItems,
            CategoryFilters,
            FilteredItems,
            key => _selectedCategoryKey = key);

        OnPropertyChanged(nameof(HasFilteredItems));
        OnPropertyChanged(nameof(IsSearchEmptyVisible));
    }

    private void SelectCategory(string? key) =>
        MusicPlaylistViewApplier.SelectCategory(key, CategoryFilters, value => _selectedCategoryKey = value, ApplyFilter);

    private void ApplyFilter()
    {
        MusicPlaylistViewApplier.ReplaceFilteredItems(
            FilteredItems,
            _playlistPresenter.Filter(AllItems, _selectedCategoryKey, SearchText));
        OnPropertyChanged(nameof(HasFilteredItems));
        OnPropertyChanged(nameof(IsSearchEmptyVisible));
    }
}
