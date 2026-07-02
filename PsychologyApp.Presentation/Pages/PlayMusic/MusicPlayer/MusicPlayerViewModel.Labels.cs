using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;

public partial class MusicPlayerViewModel
{
    public string PageTitle => AppStrings.CleanerPrayersPage;
    public string SearchPlaceholder => AppStrings.CleanerSearchPlaceholder;
    public string LoadingText => AppStrings.CleanerSearchingPrayers;
    public string NoPrayersFoundText => AppStrings.CleanerNoPrayersFound;
    public string NowPlayingLabel => AppStrings.CleanerNowPlaying;
    public string BufferingText => AppStrings.CleanerPreparingAudio;
    public string OfflineBadgeText => AppStrings.CleanerOfflineBadge;
    public string LoadFailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SearchPlaceholder),
            nameof(LoadingText),
            nameof(NoPrayersFoundText),
            nameof(NowPlayingLabel),
            nameof(BufferingText),
            nameof(OfflineBadgeText),
            nameof(ProgressDisplay),
            nameof(ActiveTrackCategory));

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
