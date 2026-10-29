using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.UI.Components;

namespace PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;

public partial class MusicPlayerViewModel
{
    public string PageTitle => AppStrings.CleanerPrayersPage;
    public string SearchPlaceholder => AppStrings.CleanerSearchPlaceholder;
    public string LoadingText => AppStrings.CleanerSearchingPrayers;
    public string NoPrayersFoundText => AppStrings.CleanerNoPrayersFound;
    public string CatalogEmptyText => AppStrings.CleanerCatalogEmpty;
    public string BufferingText => AppStrings.CleanerPreparingAudio;
    public string NeedsInternetText => AppStrings.CleanerNeedsInternet;
    public string LoadFailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SearchPlaceholder),
            nameof(LoadingText),
            nameof(NoPrayersFoundText),
            nameof(CatalogEmptyText),
            nameof(ListEmptyBodyText),
            nameof(BufferingText),
            nameof(NeedsInternetText),
            nameof(ProgressDisplay));

        if (IsDone)
        {
            string? activeUrl = ActiveTrack?.URL;
            string categoryKey = _selectedCategoryKey;
            InitializePlaylist();
            _selectedCategoryKey = categoryKey;
            ApplyFilter();
            if (!string.IsNullOrWhiteSpace(activeUrl))
            {
                ActiveTrack = AllItems.FirstOrDefault(item => item.URL == activeUrl);
            }
        }
    }
}
